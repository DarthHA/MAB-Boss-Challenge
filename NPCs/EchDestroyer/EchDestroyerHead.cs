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
    public class EchDestroyerHead : ModNPC            //使用了：全部ai，LocalAI0 2 3
    {
        public readonly int Length = 25;
        public readonly int TPDistance = 30;
        public bool ReadyToTP = false;
        public int StateRage = 0;
        public float TrailTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("WARP Destroyer");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁毁灭者");
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
            NPCID.Sets.TrailingMode[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.npcSlots = 1f;
            npc.width = 38;
            npc.height = 38;
            npc.aiStyle = -1;
            npc.defense = 50;
            npc.damage = 200;
            npc.lifeMax = 50000;
            if (!Main.expertMode)
            {
                npc.damage = 150;
                npc.defense = 0;
                npc.lifeMax = 35000;
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
            if (Target.immuneTime > 50)
            {
                Target.immuneTime = 50;
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
                //Projectile.NewProjectile(npc.Center + new Vector2(0,100), Vector2.Zero, ModContent.ProjectileType<WormHoleEXFake>(), 0, 0);
                npc.localAI[0] = -1;
                npc.localAI[2] = -1;
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

            if (npc.ai[3] == 0)              //普通阶段
            {

                npc.localAI[0] = -1;

                npc.ai[2]++;
                if (Target.dead || !Target.active)         //脱战
                {
                    if (npc.localAI[3] == 0)
                    {
                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.active && proj.type == ModContent.ProjectileType<PortalBolt>())
                            {
                                proj.active = false;
                            }
                        }
                        ReadyToTP = false;
                        Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 500, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                        Projectile.NewProjectile(new Vector2(100, 100), Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                        npc.localAI[3] = 1;
                        npc.ai[3] = 1;
                        npc.ai[2] = 0;
                    }
                }
                else
                {
                    if (npc.ai[1] == 0)                     //激光攻击
                    {
                        WormMovement(Target.Center, 18, 0.15f, 0.4f);
                        if (npc.ai[2] > 100 && npc.ai[2] % 90 == 0 && npc.ai[2] < 600)
                        {
                            Projectile.NewProjectile(Target.Center + PortalUtils.GetRandomUnit() * 400, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry>(), npc.damage / 4, 0);
                        }
                        if (npc.ai[2] == 640)
                        {
                            npc.ai[1] = 4;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }
                    }

                    else if (npc.ai[1] == 1)             //头部弹幕
                    {
                        WormMovement(Target.Center, 25, 0.25f, 0.35f);
                        if (npc.ai[2] < 580 && npc.ai[2] > 60) 
                        {
                            if (npc.ai[2] % 10 == 2)
                            {
                                Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 5, ModContent.ProjectileType<WarpSphere>(), npc.damage / 4, 0);
                                Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 5, ModContent.ProjectileType<WarpSphere>(), npc.damage / 4, 0);
                            }
                        }
                        if (npc.ai[2] > 640)
                        {
                            npc.ai[1] = 2;
                            npc.ai[2] = 0;
                        }
                    }

                    else if (npc.ai[1] == 2)               //多次传送冲撞
                    {
                        WormMovement(Target.Center, 24, 0.3f, 0.4f);
                        if (npc.ai[2] > 60)
                        {
                            if ((npc.Distance(Target.Center) > 400 && PointMulti(npc.velocity, Target.Center - npc.Center) < 0) || npc.Distance(Target.Center) > 1500)
                            {
                                Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 300, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                                Projectile.NewProjectile(Target.Center + PortalUtils.GetRandomUnit() * 400, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                                npc.ai[3] = 1;
                                npc.ai[2] = 0;
                            }
                        }

                    }

                    else if (npc.ai[1] == 3)              //追踪弹幕
                    {
                        WormMovement(Target.Center, 16, 0.15f, 0.3f);
                        if (npc.ai[2] > 100 && npc.ai[2] % 90 == 0 && npc.ai[2] < 540)
                        {
                            Projectile.NewProjectile(Target.Center + PortalUtils.GetRandomUnit() * 500, Vector2.Zero, ModContent.ProjectileType<WormHoleSentry>(), npc.damage / 4, 0, default, default, 1);
                        }
                        if (npc.ai[2] == 640)             //TP乱窜（bushi
                        {
                            Projectile.NewProjectile(Target.Center, Vector2.Zero, ModContent.ProjectileType<WarpArena>(), 0, 0, default);
                            Projectile.NewProjectile(npc.Center + Vector2.Normalize(npc.velocity) * 400, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                            float r = PortalUtils.GetRandomUnit().ToRotation();
                            for (int i = 0; i < 15; i++)
                            {
                                Vector2 Point = Target.Center + r.ToRotationVector2() * 600;
                                r += MathHelper.Pi + MathHelper.Pi / 5 * 4 * Main.rand.NextFloat() - MathHelper.Pi / 5 * 2;
                                Projectile.NewProjectile(Point, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
                            }
                            npc.ai[2] = 0;
                            npc.ai[3] = 1;
                        }

                    }

                    else if (npc.ai[1] == 4 || npc.ai[1] == 6 || npc.ai[1] == 8 || npc.ai[1] == 10)  //冲刺准备
                    {
                        if (npc.velocity.Length() > 10)
                        {
                            npc.velocity *= 0.94f;
                        }
                        WormMovement(Target.Center, 10, 0.2f, 0.3f);
                        if (npc.ai[2] == 1)
                        {
                            NPC tail = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerTail>())];
                            tail.localAI[2] = 0;
                        }
                        if (npc.localAI[2] > -1)
                        {
                            npc.localAI[2]++;
                        }
                        if (npc.localAI[2] > 14)
                        {
                            npc.ai[1]++;
                            npc.ai[2] = 0;
                            npc.localAI[2] = -1;
                        }
                    }
                    else if (npc.ai[1] == 5 || npc.ai[1] == 7 || npc.ai[1] == 9 || npc.ai[1] == 11)         //冲刺期
                    {
                        if (npc.ai[2] == 1)
                        {
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                            npc.velocity = Vector2.Normalize(Target.Center - npc.Center) * 25;
                        }
                        Vector2 TargetPos = Target.Center;
                        if (Target.Distance(npc.Center) < 500)
                        {
                            TargetPos = npc.Center + Vector2.Normalize(Target.Center - npc.Center) * 500;
                        }
                        WormMovement(TargetPos, 50, 0.4f, 0.9f);
                        if (npc.ai[2] == 10)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity) / 1000, ModContent.ProjectileType<WarpDeathray>(), npc.damage / 4, 0);
                            Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() + MathHelper.Pi / 12).ToRotationVector2() / 1000, ModContent.ProjectileType<WarpDeathray>(), npc.damage / 4, 0);
                            Projectile.NewProjectile(npc.Center, (npc.velocity.ToRotation() - MathHelper.Pi / 12).ToRotationVector2() / 1000, ModContent.ProjectileType<WarpDeathray>(), npc.damage / 4, 0);
                        }
                        if (npc.ai[2] >= 60)
                        {
                            npc.ai[1]++;
                            if (npc.ai[1] > 11)
                            {
                                npc.velocity *= 0.6f;
                                npc.ai[1] = 1;
                            }
                            npc.ai[2] = 0;
                        }
                    }
                }
                /*
                if (npc.ai[2] > 300)
                {
                    if (!ReadyToTP)
                    {
                        if (npc.Distance(Target.Center) > 400 && PointMulti(npc.velocity, Target.Center - npc.Center) < 0)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity) * 40, ModContent.ProjectileType<PortalBolt>(), 0, 0, default);
                            ReadyToTP = true;
                        }
                    }
                    else
                    {
                        if (PortalUtils.HoleCount() > 1)
                        {
                            npc.ai[3] = 1;
                            npc.ai[2] = 0;
                            ReadyToTP = false;
                        }
                    }
                }
                */
            }

            if (npc.ai[3] == 1)             //传送阶段
            {
                if (npc.ai[2] == 0)
                {
                    npc.localAI[0] = 0;
                    foreach(NPC seg in Main.npc)
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

                if (npc.localAI[0] < PortalUtils.HoleCount() && npc.localAI[0] != -1)
                {
                    Projectile Hole1 = Main.projectile[PortalUtils.FindHoleByNum((int)npc.localAI[0])];
                    this.DirectMovement(Hole1.Center, 27);
                    if (npc.Distance(Hole1.Center) <= TPDistance)
                    {
                        Main.PlaySound(SoundID.Item8, npc.Center);
                        npc.localAI[0]++;
                        Projectile Hole2 = Main.projectile[PortalUtils.FindHoleByNum((int)npc.localAI[0])];
                        npc.Center = Hole2.Center;
                        if (npc.localAI[3] != 1)
                        {
                            npc.velocity = Vector2.Normalize(Target.Center - npc.Center);
                            if (npc.ai[1] == 2)
                            {
                                npc.velocity *= 10;
                            }
                        }
                        npc.localAI[0]++;
                    }
                }
                else
                {
                    npc.localAI[0] = -1;
                    if (npc.localAI[3] != 1)
                    {
                        if (npc.ai[1] == 2)
                        {
                            WormMovement(Target.Center, 24, 0.3f, 0.4f);
                        }
                        if (npc.ai[1] == 3)
                        {
                            WormMovement(Target.Center, 20, 0.1f, 0.4f);
                        }

                    }
                }
                npc.ai[2]++;
            }
            npc.rotation = npc.velocity.ToRotation();

        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D DestTexture = mod.GetTexture("NPCs/EchDestroyer/Ech1");
            Texture2D texture2D = Main.npcTexture[npc.type];
            Color color = npc.localAI[2] < 0 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * npc.localAI[2]));
            SpriteEffects effects = (npc.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if(npc.ai[1] == 5 || npc.ai[1] == 7|| npc.ai[1] == 9|| npc.ai[1] == 11)
            {
                for(int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    float k = 1 - (float)i / NPCID.Sets.TrailCacheLength[npc.type];
                    spriteBatch.Draw(texture2D, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    spriteBatch.Draw(DestTexture, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                }
            }
            if ((npc.ai[3] == 1 && npc.ai[1] == 3) || npc.ai[1] == 2 || npc.ai[1] == 4 || npc.ai[1] == 6 || npc.ai[1] == 8 || npc.ai[1] == 10)
            {
                TrailTimer = (TrailTimer + 1) % 50;
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
                {
                    Vector2 r = (i + npc.rotation).ToRotationVector2() * (float)Math.Sin(MathHelper.Pi * TrailTimer / 50) * 5;
                    spriteBatch.Draw(texture2D,r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    spriteBatch.Draw(DestTexture,r + npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * 0.4f, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                }
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            return false;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (npc.localAI[3] == 1)
            {
                index = -1;
            }
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (npc.localAI[3] == 1)
            {
                return false;
            }
            return null;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.localAI[3] == 1)
            {
                return false;
            }
            if (npc.alpha > 0)
            {
                return false;
            }
            return null;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 4;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 4;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<TimeDisort>(), 180);
        }
        public override bool PreNPCLoot()
        {
            return true;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.RedPotion;
        }
        public override void NPCLoot()
        {
            MABWorld.DownedEchDestroyer = true;
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






        public void WormMovement(Vector2 Pos, float MaxVelocity = 24f, float TurnAcc = 0.1f, float RamAcc = 0.15f)//最大速度，转向速度，加速度
        {
            Vector2 TargetVector = Pos - npc.Center;

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

        
    }
}