using MABBossChallenge.Buffs;
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
        public float TrailTimer = 0;

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
            npc.defense = 50;
            npc.damage = 150;
            npc.lifeMax = 50000;
            if (!Main.expertMode)
            {
                npc.damage = 100;
                npc.defense = 0;
                npc.lifeMax = 35000;
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

            
            if (PreviousSeg.localAI[0] == npc.localAI[0] || npc.localAI[0] == -1)  
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

                    if (head.localAI[3] != 1)
                    {
                        if (head.ai[1] == 4 || head.ai[1] == 6 || head.ai[1] == 8 || head.ai[1] == 10)
                        {
                            if (npc.localAI[2] > -1)
                            {
                                npc.localAI[2]++;
                                if (npc.localAI[2] > 14)
                                {
                                    npc.localAI[2] = -1;
                                }
                                if (npc.localAI[2] == 7)
                                {
                                    PreviousSeg.localAI[2] = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        npc.localAI[2] = -1;
                    }
                }
            }
            else
            {
                Projectile Hole1 = Main.projectile[PortalUtils.FindHoleByNum((int)npc.localAI[0])];
                this.DirectMovementSeg(Hole1.Center, head.velocity.Length());
                if (npc.Distance(Hole1.Center) <= TPDistance)
                {
                    npc.localAI[0]++;
                    Projectile Hole2 = Main.projectile[PortalUtils.FindHoleByNum((int)npc.localAI[0])];
                    npc.Center = Hole2.Center;
                    npc.localAI[0]++;
                }
                if (npc.localAI[0] >= PortalUtils.HoleCount())
                {
                    npc.localAI[0] = -1;
                    if (npc.type == ModContent.NPCType<EchDestroyerTail>())
                    {
                        head.ai[3] = 0;
                        head.ai[2] = 0;

                        if (head.localAI[3] != 1)
                        {
                            if (head.ai[1] == 2)
                            {
                                if (++head.localAI[1] > 3)
                                {
                                    head.localAI[1] = 0;
                                    //head.ai[1] = 0;
                                    head.ai[1] = 3;
                                }
                            }
                            else if (head.ai[1] == 3)
                            {
                                head.ai[1] = 0;
                            }
                        }

                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.active && proj.type == ModContent.ProjectileType<WormHoleEX>())
                            {
                                if (proj.timeLeft > 120)
                                {
                                    proj.timeLeft = 120;
                                }
                            }
                        }
                        if (head.localAI[3] == 1) 
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

        


        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 4;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<TimeDisort>(), 120);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D DestTexture = (npc.type == ModContent.NPCType<EchDestroyerBody>()) ? mod.GetTexture("NPCs/EchDestroyer/Ech2") : mod.GetTexture("NPCs/EchDestroyer/Ech3");
            Texture2D texture2D = Main.npcTexture[npc.type];
            Color color = npc.localAI[2] < 0 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * npc.localAI[2]));
            SpriteEffects effects = (npc.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (npc.ai[3] > 0f)
            {
                NPC head = Main.npc[(int)npc.ai[3]];
                if (head.active)
                {
                    if ((head.ai[3] == 1 && head.ai[1] == 3) || head.ai[1] == 2 || head.ai[1] == 4 || head.ai[1] == 6 || head.ai[1] == 8 || head.ai[1] == 10)
                    {
                        TrailTimer = (TrailTimer + 1) % 50;
                        for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
                        {
                            Vector2 r = (i + npc.rotation).ToRotationVector2() * (float)Math.Sin(MathHelper.Pi * TrailTimer / 50) * 5;
                            spriteBatch.Draw(texture2D, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                            spriteBatch.Draw(DestTexture, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                        }
                    }
                }
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 4;
            if (projectile.penetrate == -1 && !projectile.minion && projectile.aiStyle != 99) 
            {
                projectile.maxPenetrate = 6;
                projectile.penetrate = 6;
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if (head.localAI[3] == 1)
            {
                return false;
            }
            return null;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            NPC head = Main.npc[(int)npc.ai[3]];
            if (head.localAI[3] == 1)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            return null;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation + MathHelper.Pi / 2;
        }
        public override void BossHeadSlot(ref int index)
        {
            if (Main.npc[(int)npc.ai[3]].localAI[3] == 1) 
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

    }
}