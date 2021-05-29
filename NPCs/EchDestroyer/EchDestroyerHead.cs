using MABBossChallenge.Buffs;
using MABBossChallenge.Projectiles;
using MABBossChallenge.Projectiles.EchDestroyer;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.EchDestroyer
{
    [AutoloadBossHead]
    public class EchDestroyerHead : ModNPC            //使用了：全部ai，LocalAI0 2 3
    {
        public const int Length = 25;
        public readonly int TPDistance = 30;
        public readonly static int RageDistance = 3500;
        public readonly int CoilingRadius = 200;


        public static bool TransBG = false;
        public static float TrailTimer = 0;

        public int CurrentStage = 0;
        public bool ShouldTransform = false;


        public float[] ExtraAI = new float[4];

        /// <summary>
        /// 是否启用脱战AI
        /// </summary>
        public bool IsDespawning
        {
            get
            {
                return npc.localAI[3] != 0;
            }
            set
            {
                npc.localAI[3] = value ? 1 : 0;
            }
        }

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
        /// AI阶段
        /// </summary>
        public int AIState
        {
            get
            {
                return (int)npc.ai[1];
            }
            set
            {
                npc.ai[1] = value;
            }
        }

        /// <summary>
        /// 计时器
        /// </summary>
        public float Timer
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }


        /// <summary>
        /// 是否在穿梭
        /// </summary>
        public bool IsWarping
        {
            get
            {
                return npc.ai[3] != 0;
            }
            set
            {
                npc.ai[3] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// 额外计数器，瞎jb用
        /// </summary>
        public float ExtraCounter
        {
            get => npc.localAI[1];
            set => npc.localAI[1] = value;
        }

        public enum WrapAIState
        {
            LaserNormal,          //激光扫射
            LaserRewind,          //激光回放
            RamReady,             //准备碎片冲刺
            Raming,               //碎片冲刺
            TpAndRam,              //TP冲刺
            TpAndRam2,            //TP冲刺2
            CircleRam,            //TP绕圈冲刺
            TpLaser,              //TP激光
            DSRay,                 //死星射线
            TransP2,              //进入二阶段
            Mirror,                //镜像AI
            DSRay2,                //终末螺旋
            FlashBack,             //闪回AI
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("WARP Destroyer");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁毁灭者");
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.MustAlwaysDraw[npc.type] = true;
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
            npc.value = 0;
            npc.scale = 1f;
            npc.netAlways = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            npc.alpha = 255;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Running Out of Nights");
            musicPriority = MusicPriority.BossMedium;
        }

        #region Core

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.damage /= 2;
        }
        
        public override void AI()
        {
            if (NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerHead>()) != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            Player Target = Main.player[npc.target];

            if (Target.immuneTime > 60)
            {
                Target.immuneTime = 60;
                for (int i = 0; i < Target.hurtCooldowns.Length; i++)
                {
                    if (Target.hurtCooldowns[i] > 60)
                    {
                        Target.hurtCooldowns[i] = 60;
                    }
                }
            }

            if (Target.Distance(npc.Center) >= RageDistance)
            {
                Target.AddBuff(ModContent.BuffType<PhaseAnchorBuff>(), 2);
            }

            if (npc.alpha != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 229, 0f, 0f, 100, default, 2f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                    Main.dust[num].color = Color.LightBlue;
                }
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

            if (npc.ai[0] == 0f)
            {
                HolePassing = -1;
                SegColor = -1;
                int PreviousSeg = npc.whoAmI;
                for (int j = 0; j <= Length; j++)
                {
                    int type = ModContent.NPCType<EchDestroyerBody>();
                    if (j == Length)
                    {
                        type = ModContent.NPCType<EchDestroyerTail>();
                    }
                    int CurrentSeg = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[CurrentSeg].ai[3] = npc.whoAmI;
                    Main.npc[CurrentSeg].realLife = npc.whoAmI;
                    Main.npc[CurrentSeg].ai[1] = PreviousSeg;
                    Main.npc[CurrentSeg].localAI[0] = -1;             //初始化传送门
                    Main.npc[CurrentSeg].localAI[2] = -1;
                    Main.npc[PreviousSeg].ai[0] = CurrentSeg;
                    PreviousSeg = CurrentSeg;
                }

                if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerTail>()))
                {
                    npc.active = false;
                    return;
                }
            }         //生成身体



            if (!IsWarping)              //普通阶段
            {
                HolePassing = -1;

                if (Target.dead || !Target.active)         //脱战
                {
                    IsDespawning = true;
                    Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 500, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(new Vector2(100, 100), Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    InitTeleporting();
                }
                else
                {
                    NormalMoveAI(Target);
                }
            }

            if (IsWarping)             //传送阶段
            {
                if (HolePassing < PortalUtils.HoleCount() && HolePassing != -1)
                {
                    Projectile Hole1 = Main.projectile[PortalUtils.FindHoleByNum(HolePassing)];
                    float speed = 27;
                    WarpingSpeed(ref speed);
                    this.DirectMovement(Hole1.Center, speed);
                    if (npc.Distance(Hole1.Center) <= TPDistance)
                    {
                        Main.PlaySound(SoundID.Item8, npc.Center);
                        HolePassing++;
                        Projectile Hole2 = Main.projectile[PortalUtils.FindHoleByNum(HolePassing)];
                        npc.Center = Hole2.Center;
                        if (!IsDespawning)
                        {
                            npc.velocity = Vector2.Normalize(Target.Center - npc.Center);
                        }
                        HolePassing++;
                        SpecialAIForCircle();
                    }
                }
                else
                {
                    HolePassing = -1;
                    if (!IsDespawning)
                    {
                        TelePortingAI(Target);
                    }
                }
            }
            npc.rotation = npc.velocity.ToRotation();

        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D DestTexture = mod.GetTexture("NPCs/EchDestroyer/Ech1");
            Texture2D texture2D = Main.npcTexture[npc.type];
            Color color = SegColor < 0 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * SegColor));
            SpriteEffects effects = (npc.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (AIState == (int)WrapAIState.Raming && !IsWarping)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    float k = 1 - (float)i / NPCID.Sets.TrailCacheLength[npc.type];
                    spriteBatch.Draw(texture2D, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    spriteBatch.Draw(DestTexture, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                }
            }

            TrailTimer = (TrailTimer + 1) % 50;
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
            {
                Vector2 r = (i + npc.rotation).ToRotationVector2() * (float)Math.Sin(MathHelper.Pi * TrailTimer / 50) * 8;
                spriteBatch.Draw(texture2D, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                spriteBatch.Draw(DestTexture, r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            }

            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
           
            if (AIState == (int)WrapAIState.RamReady)
            {
                Vector2 TargetSelectedPos = new Vector2(ExtraAI[0], ExtraAI[1]);
                if (TargetSelectedPos != Vector2.Zero)
                {
                    Vector2 DrawVel = Vector2.Normalize(TargetSelectedPos - npc.Center);
                    Terraria.Utils.DrawLine(Main.spriteBatch, npc.Center + DrawVel * 40, npc.Center + DrawVel * 6000, Color.Cyan * 0.8f, Color.Cyan * 0.8f, 3);
                }
                else
                {
                    Vector2 DrawVel = npc.rotation.ToRotationVector2();
                    Terraria.Utils.DrawLine(Main.spriteBatch, npc.Center + DrawVel * 40, npc.Center + DrawVel * 6000, Color.Cyan * 0.8f, Color.Cyan * 0.8f, 3);
                }
            }

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                int goretmp = Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EchGore1"), 1f);
                Main.gore[goretmp].scale = npc.scale;

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

        public override void BossHeadSlot(ref int index)
        {
            if (IsDespawning)
            {
                index = -1;
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (IsDespawning)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            if (Main.player[Player.FindClosest(npc.Center, 1, 1)].Distance(npc.Center) > RageDistance)
            {
                return false;
            }
            return null;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (IsDespawning)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            if (Main.player[Player.FindClosest(npc.Center, 1, 1)].Distance(npc.Center) > RageDistance)
            {
                return false;
            }
            return null;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (AIState == (int)WrapAIState.TransP2)
            {
                return false;
            }
            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (AIState == (int)WrapAIState.TransP2)
            {
                return false;
            }
            return true;
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 6;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 6;
        }

        public override bool PreNPCLoot()
        {
            if (npc.life > 0)
            {
                return false;
            }
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.RedPotion;
        }
        public override void NPCLoot()
        {
            if (npc.life <= 0)
            {
                Item.NewItem(npc.Hitbox, ItemID.RodofDiscord);
                Item.NewItem(npc.Hitbox, ItemID.PortalGun);
                Item.NewItem(npc.Hitbox, ItemID.Nanites, 999);
                MABWorld.DownedEchDestroyer = true;
            }
            Projectile.NewProjectile(npc.Center - Vector2.Normalize(npc.velocity) * 500 + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * 300, Vector2.Zero, ModContent.ProjectileType<WormHoleEnd>(), 0, 0);
        }
        
        public override bool CheckActive()
        {
            return false;
        }
        public override bool CheckDead()
        {
            return true;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation + MathHelper.Pi / 2;
        }

        #endregion

        /// <summary>
        /// 正常移动时的AI
        /// </summary>
        /// <param name="Target"></param>
        public void NormalMoveAI(Player Target)
        {
            if (CurrentStage == 0 && npc.life < npc.lifeMax * 0.8f)
            {
                ShouldTransform = true;
                SwitchImmume(true);
            }
            if (AIState == (int)WrapAIState.LaserNormal)                     //激光攻击
            {
                Timer++;
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 200;
                WormMovement(TargetPos, 12, 0.2f, 0.3f);

                if (Timer > 100 && Timer % 90 == 0 && Timer < 600)
                {
                    Projectile.NewProjectile(Target.Center + PortalUtils.GetRandomUnit() * 400, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry1>(), npc.damage / 4, 0);
                }
                if (Timer == 640)
                {
                    if (CurrentStage == 0)
                    {
                        SwitchAIState((int)WrapAIState.RamReady);
                    }
                    else
                    {
                        SwitchAIState((int)WrapAIState.CircleRam);
                    }
                }
            }
            else if (AIState == (int)WrapAIState.RamReady)  //冲刺准备
            {
                Timer++;
                if (npc.velocity.Length() > 10)
                {
                    npc.velocity *= 0.94f;
                }
                if (Target.Distance(npc.Center) > 1000)
                {
                    WormMovement(Target.Center, 12, 0.15f, 0.3f);
                }
                else
                {
                    WormMovement(Target.Center, 7, 0.1f, 0.2f);
                }

                if (Timer == 1)
                {
                    NPC tail = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerTail>())];
                    (tail.modNPC as EchDestroyerBody).SegColor = 0;
                }
                if (SegColor > -1)
                {
                    SegColor++;
                }
                if (SegColor <= 11)
                {
                    if (CurrentStage == 1)
                    {
                        Vector2 AimPos = Target.Center + Target.velocity * 25;
                        ExtraAI[0] = AimPos.X;
                        ExtraAI[1] = AimPos.Y;
                    }
                    else
                    {
                        Vector2 AimPos = Target.Center + Target.velocity * 30;
                        ExtraAI[0] = AimPos.X;
                        ExtraAI[1] = AimPos.Y;
                    }
                }
                if (SegColor > 14)
                {
                    SwitchAIState((int)WrapAIState.Raming, false);
                    SegColor = -1;
                }
            }
            else if (AIState == (int)WrapAIState.Raming)         //冲刺期
            {
                Timer++;
                if (Timer == 1)
                {
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    npc.velocity = Vector2.Normalize(Target.Center - npc.Center) * 25;
                    Projectile.NewProjectile(InScreenPos(npc.Center), Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(), 0, 0);
                }
                if (Timer == 5)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity) / 1000, ModContent.ProjectileType<WarpCrack>(), npc.damage / 4, 0);

                    if (CurrentStage == 1)
                    {
                        Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() + MathHelper.Pi / 10).ToRotationVector2() / 1000, ModContent.ProjectileType<WarpCrack>(), npc.damage / 4, 0);
                        Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() - MathHelper.Pi / 10).ToRotationVector2() / 1000, ModContent.ProjectileType<WarpCrack>(), npc.damage / 4, 0);
                    }
                }

                Vector2 TargetPos = new Vector2(ExtraAI[0], ExtraAI[1]);
                //if (Target.Distance(npc.Center) < 500)
                //{
                //    TargetPos = npc.Center + Vector2.Normalize(TargetPos - npc.Center) * 500;
                //}
                WormMovement(TargetPos, 80, 1f, 14.4f);

                if (Timer >= 25)
                {
                    if (++ExtraCounter > 3)
                    {
                        npc.velocity *= 0.6f;
                        ExtraCounter = 0;

                        Projectile.NewProjectile(npc.Center + npc.rotation.ToRotationVector2() * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                        Projectile.NewProjectile(Target.Center + PortalUtils.GetRandomUnit() * 1000, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                        InitTeleporting();

                    }
                    else
                    {
                        SwitchAIState((int)WrapAIState.RamReady, false);
                    }
                    ExtraAI[0] = 0;
                    ExtraAI[1] = 0;
                    Timer = 0;
                }
            }
            else if (AIState == (int)WrapAIState.TpAndRam)               //多次传送冲撞
            {
                Timer++;
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 500;
                WormMovement(TargetPos, 24, 0.2f, 0.3f);

                if (Timer == 180)
                {
                    int protmp = Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<WormHoleMinion>(), npc.damage / 4, 0);
                    Main.projectile[protmp].rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                    ExtraAI[0] = Main.rand.Next(2) * 2 - 1;
                }
                if (Timer > 180)
                {
                    if (WormHoleMinion.FindFirstMinion() != -1)
                    {
                        if (Timer < 210)
                        {
                            (Main.projectile[WormHoleMinion.FindFirstMinion()].modProjectile as WormHoleMinion).RotSpeed = MathHelper.Pi / 40 * (Timer - 180) / 30 * ExtraAI[0];
                        }
                        else if (Timer < 240)
                        {
                            (Main.projectile[WormHoleMinion.FindFirstMinion()].modProjectile as WormHoleMinion).RotSpeed = MathHelper.Pi / 40 * ExtraAI[0];
                        }
                        else if (Timer < 360)
                        {
                            (Main.projectile[WormHoleMinion.FindFirstMinion()].modProjectile as WormHoleMinion).RotSpeed = MathHelper.Pi / 40 * (360 - Timer) / 120 * ExtraAI[0];
                        }
                        if (Timer == 360)
                        {
                            Vector2 Pos = Target.Center + Main.projectile[WormHoleMinion.FindFirstMinion()].rotation.ToRotationVector2() * WormHoleMinion.Radius;
                            Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                            int protmp = Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                            Main.projectile[protmp].ai[0] = 26;

                            Pos = Target.Center + (Main.projectile[WormHoleMinion.FindFirstMinion()].rotation + MathHelper.TwoPi / 3).ToRotationVector2() * WormHoleMinion.Radius;
                            protmp = Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry1>(), 0, 0);
                            Main.projectile[protmp].timeLeft = 120;

                            Pos = Target.Center + (Main.projectile[WormHoleMinion.FindFirstMinion()].rotation + MathHelper.TwoPi / 3 * 2).ToRotationVector2() * WormHoleMinion.Radius;
                            protmp = Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry1>(), 0, 0);
                            Main.projectile[protmp].timeLeft = 120;
                            InitTeleporting();

                            Main.projectile[WormHoleMinion.FindFirstMinion()].Kill();
                        }
                    }
                }


            }
            else if (AIState == (int)WrapAIState.TpAndRam2)             //多次传送W冲刺
            {
                Timer++;
                WormMovement(Target.Center, 24, 0.2f, 0.3f);
                if (Timer == 180)
                {
                    Vector2 RandomPos = Target.Center + Target.velocity * 20 + PortalUtils.GetRandomUnit() * 800;
                    ExtraAI[0] = RandomPos.X;
                    ExtraAI[1] = RandomPos.Y;

                    Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(RandomPos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);

                    float Rot = (Target.Center - RandomPos).ToRotation();
                    float Width = 150;
                    float Height = 600;
                    for (int i = 1; i < 10; i++)
                    {
                        float dir = 1;
                        if (i % 2 == 1)
                        {
                            dir = -1;
                        }
                        Vector2 VertUnit = dir * (Rot + MathHelper.Pi / 2).ToRotationVector2();
                        Vector2 Unit = Rot.ToRotationVector2();
                        Vector2 Pos = RandomPos + Unit * Width * i + VertUnit * Height;
                        Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                        Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                        //RamMarker.SummonMark(Pos);
                    }
                    InitTeleporting();
                }

                if (Timer == 280)
                {
                    SwitchAIState((int)WrapAIState.TpAndRam);
                    ExtraAI[0] = 0;
                    ExtraAI[1] = 0;
                }

                /*
                if (Timer == 240)
                {
                    Vector2 RandomPos = new Vector2(ExtraAI[0], ExtraAI[1]);
                    Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(RandomPos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    InitTeleporting();
                }

                /*
                if (Timer > 240)
                {
                    int vel = 50;
                    if (RamMarker.GetMarkPos() != Vector2.Zero)
                    {
                        PortalUtils.DirectMovement(this, RamMarker.GetMarkPos(), vel);
                        if(npc.Distance(RamMarker.GetMarkPos()) < vel)
                        {
                            RamMarker.KillMark();
                        }
                    }
                    else
                    {
                        SwitchAIState((int)WrapAIState.TpAndRam2);
                        ExtraAI[0] = 0;
                        ExtraAI[1] = 0;
                    }
                }
                */
            }
            else if (AIState == (int)WrapAIState.CircleRam)             //TP乱窜（bushi
            {
                Timer++;
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 300;
                WormMovement(TargetPos, 10, 0.15f, 0.3f);

                if (Timer == 200)
                {
                    Timer = 0;
                    Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(), 0, 0);
                    Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<WarpArena>(), 0, 0, default);
                    Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    float r = PortalUtils.GetRandomUnit().ToRotation();
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 Point = Target.Center + r.ToRotationVector2() * 600;
                        r += MathHelper.Pi + MathHelper.Pi / 5 * 4 * Main.rand.NextFloat() - MathHelper.Pi / 5 * 2;
                        Projectile.NewProjectile(Point, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    }
                    InitTeleporting();
                }

            }
            else if (AIState == (int)WrapAIState.TpLaser)            //tp弹幕攻击
            {
                Timer++;
                if (ExtraCounter != 0)
                {
                    npc.velocity *= 0.96f;
                }
                if (Timer == 101)
                {

                    ExtraAI[0] = Main.rand.NextFloat() * MathHelper.TwoPi;
                    
                    ExtraAI[1] = Main.rand.Next(2) * 2 - 1;
                    int cap = 3;
                    if (CurrentStage == 1)
                    {
                        cap = 2;
                    }
                    ExtraAI[2] = Main.rand.Next(16 - cap) + 3;           //total 18  Rand(20-cap)
                    ExtraAI[3] = 0;
                    Vector2 TpPos = ExtraAI[0].ToRotationVector2() * 500 + (ExtraAI[0] + ExtraAI[1] * MathHelper.Pi / 2).ToRotationVector2() * 600;
                    Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(Target.Center + TpPos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    InitTeleporting();
                }
                if (Timer > 103)
                {
                    if (++ExtraCounter > 3)
                    {
                        if (CurrentStage == 0)
                        {
                            SwitchAIState((int)WrapAIState.DSRay);
                        }
                        else
                        {
                            SwitchAIState((int)WrapAIState.FlashBack);
                        }
                        ExtraCounter = 0;
                    }
                    else
                    {
                        Timer = 40;
                    }
                    ExtraAI[0] = 0;
                    ExtraAI[1] = 0;
                }
            }
            else if (AIState == (int)WrapAIState.DSRay)           //死星镭射（？
            {
                Timer++;
                if (Timer < 100)
                {
                    WormMovement(Target.Center, 12, 0.15f, 0.3f);
                }
                if (Timer == 100)
                {
                    Vector2 TpPos = Target.Center + PortalUtils.GetRandomUnit() * 400;
                    ExtraAI[0] = TpPos.X;
                    ExtraAI[1] = TpPos.Y;
                    ExtraAI[2] = Main.rand.Next(2) * 2 - 1;
                    TpPos -= new Vector2(0, CoilingRadius);
                    Vector2 TpPos2 = npc.Center + Vector2.Normalize(npc.velocity) * 400;
                    if (Vector2.Distance(TpPos2, TpPos) <= 300)
                    {
                        TpPos2 = TpPos + Vector2.Normalize(TpPos2 - TpPos) * 300;
                    }
                    Projectile.NewProjectile(InScreenPos(new Vector2(ExtraAI[0], ExtraAI[1])), Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(), 0, 0);
                    Projectile.NewProjectile(TpPos2, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(TpPos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    InitTeleporting();
                }
                if (Timer >= 102 && Timer < 400)
                {
                    Vector2 Center = new Vector2(ExtraAI[0], ExtraAI[1]);
                    npc.velocity = ((Center - npc.Center).ToRotation() - MathHelper.Pi / 2 * ExtraAI[2]).ToRotationVector2() * 20;
                    if (npc.Distance(Center) > CoilingRadius)
                    {
                        npc.Center = Center + Vector2.Normalize(npc.Center - Center) * CoilingRadius;
                    }
                    foreach (NPC segs in Main.npc)
                    {
                        if (segs.active && PortalUtils.NPCList.Contains(segs.type) && segs != npc)
                        {
                            if (segs.Distance(Center) < CoilingRadius)
                            {
                                segs.Center = Center + Vector2.Normalize(segs.Center - Center) * CoilingRadius;
                            }
                        }
                    }

                    if (Timer == 140)
                    {
                        int dir = Math.Sign(Target.Center.X - npc.Center.X);
                        Projectile.NewProjectile(Center, new Vector2(dir, 0), ModContent.ProjectileType<KyberCrystal>(), npc.damage / 4, 0, default, npc.whoAmI);
                    }
                    if (Timer == 350)
                    {
                        foreach (Projectile kyber in Main.projectile)
                        {
                            if (kyber.active && kyber.type == ModContent.ProjectileType<KyberCrystal>() && kyber.ai[0] == npc.whoAmI)
                            {
                                kyber.ai[1] = 1;
                                kyber.extraUpdates = 1;
                            }
                        }
                    }

                }

                if (Timer >= 370)
                {
                    WormMovement(Target.Center, 12, 0.2f, 0.3f);
                    if (Timer == 390)
                    {
                        SwitchAIState((int)WrapAIState.LaserNormal);
                        ExtraAI[0] = 0;
                        ExtraAI[1] = 0;
                        ExtraAI[2] = 0;
                    }
                }

            }
            else if (AIState == (int)WrapAIState.TransP2)
            {
                Timer++;
                Vector2 MovePos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 600;
                WormMovement(MovePos, 18, 0.2f, 0.3f);   //80-130
                if (Timer == 80) 
                {
                    Main.PlaySound(SoundID.Item113, Target.Center);
                    TransBG = true; 
                }
                float progress = Terraria.Utils.Clamp((Timer - 80) / 50f, 0, 1);
                progress = (float)Math.Sin(progress * MathHelper.Pi);
                if (!Filters.Scene["MABBossChallenge:WarpEffect"].IsActive())
                {
                    Filters.Scene.Activate("MABBossChallenge:WarpEffect");
                }
                Filters.Scene["MABBossChallenge:WarpEffect"].GetShader().UseProgress(progress);
                if (Timer > 200)
                {
                    SwitchImmume(false);
                    CurrentStage = 1;
                    Filters.Scene["MABBossChallenge:WarpEffect"].Deactivate();
                    SwitchAIState((int)WrapAIState.LaserRewind);                  //WrapAIState.LaserRewind
                }
            }
            else if (AIState == (int)WrapAIState.Mirror)              //镜像AI
            {
                Timer++;
                if (Timer == 1)
                {
                    Vector2 SummonPos = Target.Center + (Target.Center - npc.Center);
                    NPC.NewNPC((int)SummonPos.X, (int)SummonPos.Y, ModContent.NPCType<EchPhantom>());
                }
                Vector2 MovePos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 200;
                WormMovement(MovePos, 20, 0.3f, 0.4f);
                /*
                if (Timer == 360)
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<EchPhantom>()))
                    {
                        (Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchPhantom>())].modNPC as EchPhantom).SwitchPos();
                    }
                }
                */
                if (Timer == 720)
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<EchPhantom>()))
                    {
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchPhantom>())].ai[3] = 1;
                    }
                }
                if (Timer > 760)
                {
                    SwitchAIState((int)WrapAIState.DSRay2);
                }
            }
            else if (AIState == (int)WrapAIState.DSRay2)           //终末螺旋
            {
                Timer++;
                if (Timer == 100)
                {
                    Vector2 TpPos = Target.Center - new Vector2(0, 300);
                    ExtraAI[0] = TpPos.X;
                    ExtraAI[1] = TpPos.Y;
                    ExtraAI[2] = Main.rand.Next(2) * 2 - 1;
                    TpPos -= new Vector2(0, CoilingRadius);

                    Vector2 TpPos2 = npc.Center + Vector2.Normalize(npc.velocity) * 400;
                    if (Vector2.Distance(TpPos2, TpPos) <= 300)
                    {
                        TpPos2 = TpPos + Vector2.Normalize(TpPos2 - TpPos) * 300;
                    }
                    Projectile.NewProjectile(InScreenPos(new Vector2(ExtraAI[0], ExtraAI[1])), Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(), 0, 0);
                    Projectile.NewProjectile(TpPos2, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    Projectile.NewProjectile(TpPos, Vector2.Zero, ModContent.ProjectileType<WormHole>(), 0, 0);
                    InitTeleporting();
                }

                if (Timer >= 102 && Timer < 770)           //210
                {
                    Vector2 Center = new Vector2(ExtraAI[0], ExtraAI[1]);
                    npc.velocity = ((Center - npc.Center).ToRotation() - MathHelper.Pi / 2 * ExtraAI[2]).ToRotationVector2() * 20;
                    if (npc.Distance(Center) > CoilingRadius)
                    {
                        npc.Center = Center + Vector2.Normalize(npc.Center - Center) * CoilingRadius;
                    }
                    foreach (NPC segs in Main.npc)
                    {
                        if (segs.active && PortalUtils.NPCList.Contains(segs.type) && segs != npc)
                        {
                            if (segs.Distance(Center) < CoilingRadius)
                            {
                                segs.Center = Center + Vector2.Normalize(segs.Center - Center) * CoilingRadius;
                            }
                        }
                    }
                }
                else if (Timer >= 770)
                {
                    WormMovement(Target.Center, 14f, 0.25f, 0.4f);
                }
                if (Timer == 140)         //210
                {
                    Vector2 Center = new Vector2(ExtraAI[0], ExtraAI[1]);
                    int dir = Math.Sign(Target.Center.X - npc.Center.X);
                    Projectile.NewProjectile(Center, new Vector2(dir, 0), ModContent.ProjectileType<KyberCrystal2>(), npc.damage / 4, 0, default, npc.whoAmI);
                    Projectile.NewProjectile(Center, Vector2.Zero, ModContent.ProjectileType<WarpArena3>(), 0, 0);
                }
                if (Timer == 690)            //750
                {
                    foreach (Projectile kyber in Main.projectile)
                    {
                        if (kyber.active && kyber.type == ModContent.ProjectileType<KyberCrystal2>() && kyber.ai[0] == npc.whoAmI)
                        {
                            kyber.ai[1] = 1;
                            kyber.extraUpdates = 1;
                        }
                    }
                }
                if (Timer == 710)
                {
                    foreach (Projectile sphere in Main.projectile)
                    {
                        if (sphere.active && sphere.type == ModContent.ProjectileType<WarpSphere2>())
                        {
                            if (sphere.localAI[0] < 241)
                            {
                                sphere.localAI[0] = 241;
                            }
                        }
                    }
                }
                if (Timer >= 730)            //790
                {
                    SwitchAIState((int)WrapAIState.LaserRewind);       //WrapAIState.LaserRewind
                    ExtraAI[0] = 0;
                    ExtraAI[1] = 0;
                    ExtraAI[2] = 0;
                }

            }
            else if (AIState == (int)WrapAIState.FlashBack)
            {
                Timer++;
                if (Timer == 10)
                {
                    Projectile.NewProjectile(InScreenPos(new Vector2(ExtraAI[0], ExtraAI[1])), Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(), 0, 0);
                    Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<WarpArena2>(), 0, 0);
                }
                Vector2 MoveTarget = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 200;
                WormMovement(MoveTarget, 16f, 0.45f, 0.45f);
                if (Timer == 100 || Timer == 140 || Timer == 180 || Timer == 220 || Timer == 260)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        float r = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Dust dust = Dust.NewDustDirect(Target.Center, 1, 1, MyDustId.ElectricCyan);
                        dust.noLight = false;
                        dust.noGravity = true;
                        dust.velocity = r.ToRotationVector2() * 6;
                        dust.scale = 1.0f;
                    }
                    Main.PlaySound(SoundID.Item44, Target.Center);
                    PlayerMark.SummonMark(Target.Center);
                }
                if (Timer == 320 || Timer == 360 || Timer == 400 || Timer == 440 || Timer == 480)
                {
                    Vector2 OldPos = PlayerMark.GetMarkPos();
                    Target.Teleport(OldPos);
                    PlayerMark.KillMark();
                }

                if (Timer > 660)
                {
                    Timer = 0;
                    SwitchAIState((int)WrapAIState.TpAndRam);
                }
            }
            else if (AIState == (int)WrapAIState.LaserRewind)
            {
                Timer++;
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 200;
                WormMovement(TargetPos, 12, 0.2f, 0.3f);
                if (Timer > 100 && Timer % 110 == 0 && Timer < 600)
                {
                    Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry2>(), npc.damage / 4, 0);
                }
                if (Timer >= 640)
                {
                    SwitchAIState((int)WrapAIState.RamReady);         //RamReady
                }
            }

        }


        

        /// <summary>
        /// 传送时头部的AI
        /// </summary>
        /// <param name="Target"></param>
        public void TelePortingAI(Player Target)
        {
            if (CurrentStage == 0 && npc.life < npc.lifeMax * 0.8f)
            {
                SwitchImmume(true);
                ShouldTransform = true;
            }
            if (AIState == (int)WrapAIState.LaserNormal)
            {
                WormMovement(Target.Center, 12, 0.1f, 0.3f);
            }
            else if (AIState == (int)WrapAIState.Raming)
            {
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 400;
                WormMovement(TargetPos, 12, 0.15f, 0.3f);
            }
            else if (AIState == (int)WrapAIState.TpAndRam)
            {
                WormMovement(Target.Center, 20, 0.2f, 0.3f);
            }
            else if (AIState == (int)WrapAIState.TpAndRam2)
            {
                WormMovement(Target.Center, 24, 0.2f, 0.3f);
                /*
                int vel = 50;
                if (RamMarker.GetMarkPos() != Vector2.Zero)
                {
                    PortalUtils.DirectMovement(this, RamMarker.GetMarkPos(), vel);
                    if (npc.Distance(RamMarker.GetMarkPos()) < vel)
                    {
                        RamMarker.KillMark();
                    }
                }
                else
                {
                    SwitchAIState((int)WrapAIState.TpAndRam2);
                    ExtraAI[0] = 0;
                    ExtraAI[1] = 0;
                }
                */
            }
            else if (AIState == (int)WrapAIState.CircleRam)
            {
                Vector2 TargetPos = Target.Center + Vector2.Normalize(npc.Center - Target.Center) * 400;
                WormMovement(TargetPos, 20, 0.1f, 0.4f);
            }
            else if (AIState == (int)WrapAIState.TpLaser)
            {
                Vector2 MoveDir = (ExtraAI[0] - ExtraAI[1] * MathHelper.Pi / 2).ToRotationVector2();
                npc.velocity = MoveDir * 30;
                Timer++;
                int cap = 2;
                if (CurrentStage == 1)
                {
                    cap = 1;
                }
                if (Timer % 2 == 1)
                {
                    ExtraAI[3]++;
                    if (ExtraAI[3] < ExtraAI[2] || ExtraAI[3] > ExtraAI[2] + cap)
                    {
                        Projectile.NewProjectile(npc.Center, -ExtraAI[0].ToRotationVector2(), ModContent.ProjectileType<WarpBolt>(), npc.damage / 4, 0);
                    }
                }
            }
            else if (AIState == (int)WrapAIState.DSRay)
            {
                Vector2 Center = new Vector2(ExtraAI[0], ExtraAI[1]);
                npc.velocity = ((Center - npc.Center).ToRotation() - MathHelper.Pi / 2 * ExtraAI[2]).ToRotationVector2() * 20;
                if (npc.Distance(Center) > CoilingRadius)
                {
                    npc.Center = Center + Vector2.Normalize(npc.Center - Center) * CoilingRadius;
                }
                foreach (NPC segs in Main.npc)
                {
                    if (segs.active && PortalUtils.NPCList.Contains(segs.type) && segs != npc)
                    {
                        if (segs.Distance(Center) < CoilingRadius)
                        {
                            segs.Center = Center + Vector2.Normalize(segs.Center - Center) * CoilingRadius;
                        }
                    }
                }
            }
            else if (AIState == (int)WrapAIState.DSRay2)
            {
                Vector2 Center = new Vector2(ExtraAI[0], ExtraAI[1]);
                npc.velocity = ((Center - npc.Center).ToRotation() - MathHelper.Pi / 2 * ExtraAI[2]).ToRotationVector2() * 20;
                if (npc.Distance(Center) > CoilingRadius)
                {
                    npc.Center = Center + Vector2.Normalize(npc.Center - Center) * CoilingRadius;
                }
                foreach (NPC segs in Main.npc)
                {
                    if (segs.active && PortalUtils.NPCList.Contains(segs.type) && segs != npc)
                    {
                        if (segs.Distance(Center) < CoilingRadius)
                        {
                            segs.Center = Center + Vector2.Normalize(segs.Center - Center) * CoilingRadius;
                        }
                    }
                }
            }
            

        }

        /// <summary>
        /// 停止传送的瞬间
        /// </summary>
        public void EndTeleporting()
        {
            if (AIState == (int)WrapAIState.Raming)
            {
                SwitchAIState((int)WrapAIState.TpLaser);
            }
            else if (AIState == (int)WrapAIState.TpLaser)
            {
                ExtraAI[2] = 0;
                ExtraAI[3] = 0;
            }
            else if (AIState == (int)WrapAIState.TpAndRam)
            {
                if (++ExtraCounter > 3)
                {
                    ExtraAI[0] = 0;
                    ExtraCounter = 0;
                    SwitchAIState((int)WrapAIState.LaserNormal);             //WrapAIState.LaserNormal
                }
                else
                {
                    ExtraAI[0] = 0;
                    Timer = 120;
                }
            }
            else if (AIState == (int)WrapAIState.CircleRam)
            {
                SwitchAIState((int)WrapAIState.Mirror);
                Timer = 0;
            }
        }

        public void WarpingSpeed(ref float speed)
        {
            if (AIState == (int)WrapAIState.TpLaser)
            {
                speed = 30;
            }
            else if (AIState == (int)WrapAIState.TpAndRam2)
            {
                speed = 80;
            }
        }

        public void SpecialAIForCircle()
        {
            if (AIState == (int)WrapAIState.CircleRam)
            {
                if (PortalUtils.FindHoleByNum(HolePassing + 2) != -1)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TPCircleIndicator>(), 0, 0, default, HolePassing + 1);
                }
            }
        }


        #region Utils
        /// <summary>
        /// 开始传送
        /// </summary>
        public void InitTeleporting()
        {
            if (IsWarping) return;
            IsWarping = true;
            HolePassing = 0;
            foreach (NPC seg in Main.npc)
            {
                if (seg.active)
                {
                    if (PortalUtils.NPCList.Contains(seg.type))
                    {
                        if (seg.ai[3] == npc.whoAmI)
                        {
                            seg.localAI[0] = 0;
                        }
                    }
                }
            }

        }

        public void SwitchImmume(bool immune)
        {
            foreach(NPC segs in Main.npc)
            {
                if(segs.active && PortalUtils.NPCList.Contains(segs.type))
                {
                    segs.dontTakeDamage = immune;
                }
            }
        }

        public void SwitchAIState(int P, bool CanSwitchPhase = true)
        {
            AIState = P;
            Timer = 0;
            if (CanSwitchPhase)
            {
                if (ShouldTransform)
                {
                    ShouldTransform = false;
                    if (CurrentStage == 0)
                    {
                        AIState = (int)WrapAIState.TransP2;
                    }
                }
            }
        }



        public void WormMovement(Vector2 Pos, float MaxVelocity = 24f, float TurnAcc = 0.1f, float RamAcc = 0.15f)//最大速度，转向速度，加速度
        {
            Vector2 TargetVector = Pos - npc.Center;
            if (TargetVector == Vector2.Zero || TargetVector.HasNaNs()) return;
            TargetVector = Vector2.Normalize(TargetVector) * MaxVelocity;
            if ((npc.velocity.X * TargetVector.X > 0f) && (npc.velocity.Y * TargetVector.Y > 0f))    //加速
            {
                npc.velocity.X += Math.Sign(TargetVector.X - npc.velocity.X) * RamAcc;
                npc.velocity.Y += Math.Sign(TargetVector.Y - npc.velocity.Y) * RamAcc;
            }
            if ((npc.velocity.X * TargetVector.X > 0f) || (npc.velocity.Y * TargetVector.Y > 0f))       //转向
            {
                npc.velocity.X += Math.Sign(TargetVector.X - npc.velocity.X) * TurnAcc;
                npc.velocity.Y += Math.Sign(TargetVector.Y - npc.velocity.Y) * TurnAcc;
                if (Math.Abs(TargetVector.Y) < MaxVelocity * 0.2 && (npc.velocity.X * TargetVector.X < 0f))
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * TurnAcc * 2;
                }
                if (Math.Abs(TargetVector.X) < MaxVelocity * 0.2 && (npc.velocity.Y * TargetVector.Y < 0f))
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * TurnAcc * 2;
                }
            }
            else if (Math.Abs(TargetVector.X) > Math.Abs(TargetVector.Y))
            {
                npc.velocity.X += Math.Sign(TargetVector.X - npc.velocity.X) * TurnAcc * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < MaxVelocity * 0.5)
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * TurnAcc;
                }
            }
            else
            {
                npc.velocity.Y += Math.Sign(TargetVector.Y - npc.velocity.Y) * TurnAcc * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < MaxVelocity * 0.5)
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * TurnAcc;
                }
            }

        }

        public float PointMulti(Vector2 v1,Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        private Vector2 InScreenPos(Vector2 Pos)
        {
            Vector2 ScreenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 RelativePos = Pos - ScreenCenter;
            if (Math.Abs(RelativePos.X) > Main.screenWidth / 2)
            {
                float k = (float)Main.screenWidth / 2 / Math.Abs(RelativePos.X);
                RelativePos.Y *= k;
                RelativePos.X = Math.Sign(RelativePos.X) * (float)Main.screenWidth / 2;
            }
            if (Math.Abs(RelativePos.Y) > Main.screenHeight / 2)
            {
                float k = (float)Main.screenHeight / 2 / Math.Abs(RelativePos.Y);
                RelativePos.X *= k;
                RelativePos.Y = Math.Sign(RelativePos.Y) * (float)Main.screenHeight / 2;
            }
            return ScreenCenter + RelativePos;
        }

        #endregion

    }
}