using MABBossChallenge.Projectiles.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.EchDestroyer
{
    [AutoloadBossHead]
    public class EchDestroyerBody : ModNPC          //使用了：全部ai，LocalAI0,localai1,localai2
    {
        public readonly float SegDistance = 44;
        public readonly float TPDistance = 30;


        /// <summary>
        /// 穿过的虫洞数，-1默认不处于跃迁中
        /// </summary>
        public int HolePassing
        {
            get
            {
                return (int)npc.localAI[0];
            }
            set
            {
                npc.localAI[0] = value;
            }
        }

        /// <summary>
        /// 体节颜色，-1默认为无色
        /// </summary>
        public float SegColor
        {
            get => npc.localAI[2];
            set => npc.localAI[2] = value;
        }

        /// <summary>
        /// 头部是否脱战，无头直接true
        /// </summary>
        public bool IsDespawning
        {
            get
            {
                int head = NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerHead>());
                if (head == -1) return true;
                if (!Main.npc[head].active) return true;
                return Main.npc[head].localAI[3] != 0;
            }
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Destroyer");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁毁灭者");
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
            NPCID.Sets.TrailingMode[npc.type] = 3;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return new bool?(false);
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.npcSlots = 1f;
            npc.width = 38;
            npc.height = 38;
            npc.aiStyle = -1;
            npc.defense = 25;
            npc.damage = 180;
            npc.lifeMax = 300000;
            if (!Main.expertMode)
            {
                npc.damage = 120;
                npc.defense = 0;
                npc.lifeMax = 240000;
            }
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            //npc.behindTiles = true;
            npc.netAlways = true;
            npc.scale = 1f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            npc.dontCountMe = true;
            npc.alpha = 255;
        }

        #region Core
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage /= 2;
            npc.lifeMax /= 2;
        }
        public override void AI()
        {
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }

            bool dead = false;
            if (npc.ai[1] <= 0f)
            {
                dead = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0 && !Main.npc[(int)npc.ai[1]].active)
            {
                dead = true;
            }
            if (dead)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
                return;
            }

            NPC head = Main.npc[(int)npc.ai[3]];
            NPC PreviousSeg = Main.npc[(int)npc.ai[1]];

            if (PreviousSeg.alpha < 128)
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 229, 0f, 0f, 100, default, 2f);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.color = Color.LightBlue;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }

            if (PreviousSeg.localAI[0] == HolePassing || HolePassing == -1)  
            {
                if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
                {
                    if (npc.Distance(PreviousSeg.Center) > 5)
                    {
                        Vector2 SegVect = Vector2.Zero;
                        try
                        {
                            SegVect = PreviousSeg.Center - npc.Center;
                        }
                        catch
                        {
                        }
                        if (SegVect == Vector2.Zero || SegVect.HasNaNs()) SegVect = new Vector2(0, 1f);
                        npc.rotation = SegVect.ToRotation();
                        int dist = (int)(SegDistance * npc.scale);
                        SegVect -= dist * Vector2.Normalize(SegVect);
                        npc.velocity = Vector2.Zero;
                        npc.position += SegVect;
                    }

                    if (!IsDespawning)               //体节信号的传导
                    {
                        if (SegColor > -1)
                        {
                            SegColor++;
                            if (SegColor > 14)
                            {
                                SegColor = -1;
                            }
                            if (SegColor == 7)
                            {
                                PreviousSeg.localAI[2] = 0;
                            }
                        }
                    }
                    else
                    {
                        SegColor = -1;
                    }
                }
            }
            else
            {
                Projectile Hole1 = Main.projectile[PortalUtils.FindHoleByNum(HolePassing)];
                this.DirectMovementSeg(Hole1.Center, head.velocity.Length());
                if (npc.Distance(Hole1.Center) <= TPDistance)
                {
                    HolePassing++;
                    Projectile Hole2 = Main.projectile[PortalUtils.FindHoleByNum(HolePassing)];
                    npc.Center = Hole2.Center;
                    HolePassing++;
                }
                if (HolePassing >= PortalUtils.HoleCount())
                {
                    HolePassing = -1;
                    if (npc.type == ModContent.NPCType<EchDestroyerTail>())
                    {
                        //head.ai[2] = 0;
                        head.ai[3] = 0;

                        if (!IsDespawning)
                        {
                            (head.modNPC as EchDestroyerHead).EndTeleporting();
                        }

                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.active && proj.type == ModContent.ProjectileType<WormHole>())
                            {
                                if (proj.timeLeft > 120)
                                {
                                    proj.timeLeft = 120;
                                }
                            }
                        }
                        if (IsDespawning) 
                        {
                            foreach(NPC allsegs in Main.npc)
                            {
                                if(allsegs.active && PortalUtils.NPCList.Contains(allsegs.type))
                                {
                                    allsegs.active = false;
                                }
                            }
                        }
                    }
                }
            }

        }


        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                string SelectedGore = Main.rand.NextBool() ? "Gores/EchGore2" : "Gores/EchGore3";
                if (npc.type == ModContent.NPCType<EchDestroyerTail>())
                {
                    SelectedGore = "Gores/EchGore4";
                }
                int goretmp = Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot(SelectedGore), 1f);
                Main.gore[goretmp].scale = npc.scale;
                if (Main.rand.Next(2) == 0)
                {
                    for (int num662 = 0; num662 < 10; num662++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default, 1.5f);
                        dust.velocity *= 1.4f;
                    }
                    for (int num664 = 0; num664 < 5; num664++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 2.5f);
                        dust.noGravity = true;
                        dust.velocity *= 5f;
                        dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 1.5f);
                        dust.velocity *= 3f;
                    }
                    Gore gore = Gore.NewGoreDirect(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                    gore.velocity *= 0.4f;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    gore = Gore.NewGoreDirect(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                    gore.velocity *= 0.4f;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    gore = Gore.NewGoreDirect(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                    gore.velocity *= 0.4f;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    gore = Gore.NewGoreDirect(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                    gore.velocity *= 0.4f;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                    return;
                }
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 6;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D DestTexture = (npc.type == ModContent.NPCType<EchDestroyerBody>()) ? mod.GetTexture("NPCs/EchDestroyer/Ech2") : mod.GetTexture("NPCs/EchDestroyer/Ech3");
            Texture2D texture2D = Main.npcTexture[npc.type];
            Color color = SegColor < 0 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * SegColor));
            SpriteEffects effects = (npc.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (npc.ai[3] > 0f)
            {
                NPC head = Main.npc[(int)npc.ai[3]];
                if (head.active)
                {
                    //TrailTimer = (TrailTimer + 1) % 50;
                    float TrailTimer = EchDestroyerHead.TrailTimer;
                    for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
                    {
                        Vector2 r = (i + npc.rotation).ToRotationVector2() * (float)Math.Sin(MathHelper.Pi * TrailTimer / 50) * 8;
                        spriteBatch.Draw(texture2D, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                        spriteBatch.Draw(DestTexture, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    }
                }
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 6;
            if (projectile.penetrate == -1 && !projectile.minion && projectile.aiStyle != 99) 
            {
                projectile.maxPenetrate = 4;
                projectile.penetrate = 4;
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if (IsDespawning)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            if (Main.player[Player.FindClosest(head.Center, 1, 1)].Distance(head.Center) > EchDestroyerHead.RageDistance)
            {
                return false;
            }
            return null;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if (IsDespawning)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            if (Main.player[Player.FindClosest(head.Center, 1, 1)].Distance(head.Center) > EchDestroyerHead.RageDistance)
            {
                return false;
            }
            return null;
        }

        public override bool? CanHitNPC(NPC target)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if ((head.modNPC as EchDestroyerHead).AIState == (int)EchDestroyerHead.WrapAIState.TransP2)
            {
                return false;
            }
            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if ((head.modNPC as EchDestroyerHead).AIState == (int)EchDestroyerHead.WrapAIState.TransP2)
            {
                return false;
            }
            return true;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation + MathHelper.Pi / 2;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (IsDespawning) 
            {
                index = -1;
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        
        public override bool PreNPCLoot()
        {
            return false;
        }
        #endregion
    }
}