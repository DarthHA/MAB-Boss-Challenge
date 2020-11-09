using MABBossChallenge.Buffs;
using MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.PlayerBoss
{
    [AutoloadBossHead]
    public class VortexRangerBoss : ModNPC
    {
        private int WingsFrame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex Defender");
            DisplayName.AddTranslation(GameCulture.Chinese, "星璇守护者");
            TranslationUtils.AddTranslation("VortexDefender", "Vortex Defender", "星璇守护者");
            TranslationUtils.AddTranslation("VortexRangerDescription", "The agile ranger mastering the power of Vortex", "掌握星璇之力的敏捷游侠");
            TranslationUtils.AddTranslation("VortexRanger", "Vortex Ranger", "星璇游侠");
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 38;
            npc.height = 50;
            npc.damage = 100;
            npc.defense = 20;
            npc.lifeMax = 40000;
            if (!Main.expertMode)
            {
                npc.damage = 60;
                npc.lifeMax = 30000;
            }
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[ModContent.BuffType<SolarFlareBuff>()] = true;
            npc.buffImmune[ModContent.BuffType<JusticeJudegmentBuff>()] = true;
            npc.buffImmune[ModContent.BuffType<ManaFlare>()] = true;
            npc.buffImmune[ModContent.BuffType<LifeFlare>()] = true;
            npc.buffImmune[ModContent.BuffType<DamageFlare>()] = true;
            npc.buffImmune[ModContent.BuffType<ImprovedCelledBuff>()] = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Heroic");
            musicPriority = MusicPriority.BossHigh;
            //NPCID.Sets.TrailCacheLength[npc.type] = 6;
            //NPCID.Sets.TrailingMode[npc.type] = 2;
            if (ModLoader.GetMod("CalamityMod") != null)
            {
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("ExoFreeze")] = true;
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("GlacialState")] = true;
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TemporalSadness")] = true;
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("SilvaStun")] = true;
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TimeSlow")] = true;
                npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("PearlAura")] = true;
            }
            npc.dontTakeDamage = true;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.life /= 2;
            npc.damage /= 2;
        }

        private void Movement(Vector2 targetPos, float speedModifier, bool fastX = true)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * (fastX ? 2 : 1);
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > 24)
                npc.velocity.X = 24 * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > 24)
                npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);


        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }


        public override void AI()
        {
            if (!MABWorld.DownedVortexPlayer)
            {
                NPC.ShieldStrengthTowerVortex = 5;
            }
            npc.TargetClosest();
            Player player = Main.player[npc.target];
            if (player.dead || (player.Center - npc.Center).Length() > 4000)
            {
                npc.velocity -= new Vector2(0, 1);
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                return;
            }

            if (ModLoader.GetMod("CalamityMod") != null)
            {
                player.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("RageMode")] = true;
                player.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("AdrenalineMode")] = true;
            }
            if (npc.ai[0] != 2 || (npc.ai[1] != 6 && npc.ai[1] != 7))
            {
                if (player.Center.X - npc.Center.X >= 0) { npc.spriteDirection = 1; npc.direction = 1; }
                if (player.Center.X - npc.Center.X < 0) { npc.spriteDirection = -1; npc.direction = -1; }
            }
            npc.frameCounter++;
            if (npc.frameCounter > 2)
            {
                npc.frameCounter = 0;
                WingsFrame++;
                if (WingsFrame > 3) WingsFrame = 0;
            }


            if (npc.ai[3] == 1)        //隐形
            {
                if (npc.alpha < 255)
                {
                    npc.alpha += 6;
                    int dust = Dust.NewDust(npc.Center, 2, 2, 229, default, default, default, default, 1);
                    Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5f;
                    Main.dust[dust].noGravity = true;
                    dust = Dust.NewDust(npc.Center, 2, 2, 240, default, default, default, default, 1);
                    Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5f;
                    Main.dust[dust].noGravity = true;
                }
                if (npc.alpha > 255)
                {
                    npc.alpha = 255;
                }
            }
            if (npc.ai[3] == 0)          //显形
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= 6;
                    int dust = Dust.NewDust(npc.Center, 2, 2, 229, default, default, default, default, 1);
                    Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5f;
                    Main.dust[dust].noGravity = true;
                    dust = Dust.NewDust(npc.Center, 2, 2, 240, default, default, default, default, 1);
                    Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5f;
                    Main.dust[dust].noGravity = true;
                }
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }


            if (npc.ai[0] == 0)               //开局
            {
                npc.GivenName = TranslationUtils.GetTranslation("VortexDefender");
                if (!MABWorld.DownedVortexPlayer)
                {
                    npc.ai[2]++;
                    if (npc.ai[2] == 1)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("VortexDefender"), TranslationUtils.GetTranslation("VortexRangerDescription"));
                    }

                    if (npc.ai[2] >= 330)
                    {
                        npc.dontTakeDamage = false;
                        npc.ai[0] = 1;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                }
                else
                {
                    npc.ai[0] = 1;
                    npc.dontTakeDamage = false;
                }
            }


            if (npc.ai[0] == 1)
            {


                switch (npc.ai[1])
                {
                    case 0:             //星璇机枪1
                        {
                            Vector2 Pos = player.Center - new Vector2(400, 0) * npc.direction;
                            Movement(Pos, 0.3f - 0.1f * npc.alpha / 255, true);
                            npc.ai[2]++;

                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<VortexBeaterHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 180)  //270
                            {
                                npc.ai[1] = 1;
                                npc.ai[2] = 0;
                                npc.localAI[0] = npc.direction;
                            }
                        }
                        break;
                    case 1:             //换位
                        {
                            Vector2 Pos = player.Center + new Vector2(400, 0) * npc.localAI[0];
                            Movement(Pos, 0.3f - 0.1f * npc.alpha / 255, true);
                            npc.ai[2]++;
                            if (npc.ai[2] >= 50)
                            {
                                npc.ai[1] = 2;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:             //星璇机枪2
                        {
                            Vector2 Pos = player.Center + new Vector2(400, 0) * npc.localAI[0];
                            Movement(Pos, 0.3f - 0.1f * npc.alpha / 255, true);
                            npc.ai[2]++;

                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<VortexBeaterHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 180)
                            {
                                npc.ai[1] = 3;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                            }
                        }
                        break;
                    case 3:                  //幻象弓
                        {
                            Vector2 Pos = player.Center - new Vector2(0, 300) + new Vector2(200, 0) * Math.Sign(player.velocity.X);// - new Vector2(100, 0) * npc.direction;
                            Movement(Pos, 0.3f - 0.1f * npc.alpha / 255, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] == 250)
                            {
                                npc.ai[3] = (npc.ai[3] + 1) % 2;        //切换
                            }
                            if (npc.ai[2] > 270)
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;

                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (npc.ai[0] == 2)         //变换形态
            {
                npc.ai[3] = 0;
                npc.velocity *= 0.8f;
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Ex Mechina");
                    npc.lifeMax = 300000;
                    npc.damage = 180;
                    npc.defense = 40;
                    if (!Main.expertMode)
                    {
                        npc.damage = 120;
                        npc.lifeMax = 200000;
                        npc.defense = 20;
                    }
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<VortexRitual>(), 0, 0, default, npc.whoAmI);
                }
                if (npc.ai[2] == 120)
                {
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    //Arena = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarArena>(), 0, 0, Main.myPlayer, npc.whoAmI);
                    Main.NewText(TranslationUtils.GetTranslation("TrueFight"), Color.LightGreen);
                    npc.GivenName = TranslationUtils.GetTranslation("VortexRanger");
                    //Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<HelFireHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);

                }
                if (npc.ai[2] >= 120 && npc.ai[2] <= 240)
                {
                    if (npc.life < npc.lifeMax - 500)
                    {
                        npc.HealEffect(500);
                        npc.life += 500;
                    }
                    else
                    {
                        npc.life = npc.lifeMax;
                    }
                }
                if (npc.ai[2] > 240)
                {
                    npc.dontTakeDamage = false;
                    npc.life = npc.lifeMax;
                    npc.ai[0] = 3;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                }
            }




            if (npc.ai[0] == 3)             //二阶段
            {

                switch (npc.ai[1])
                {
                    case 0:             //星璇机枪1
                        {
                            Vector2 Pos = player.Center - new Vector2(400, 0) * npc.direction;
                            Movement(Pos, 0.4f, true);
                            npc.ai[2]++;

                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<VortexBeaterHostile2>(), npc.damage / 4 + 15 * npc.alpha / 255, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 180)  //270
                            {
                                npc.ai[1] = 1;
                                npc.ai[2] = 0;
                                npc.localAI[0] = npc.direction;
                            }
                        }
                        break;
                    case 1:             //换位加外星霰弹枪
                        {

                            npc.ai[2]++;
                            if (npc.ai[2] < 30)
                            {
                                Vector2 Pos = player.Center - new Vector2(400, 0) * npc.localAI[0] + new Vector2(0, 150);
                                Movement(Pos, 0.3f, true);
                            }
                            if (npc.ai[2] == 30)
                            {
                                Vector2 Pos = player.Center + new Vector2(400, 0) * npc.localAI[0];
                                npc.velocity = new Vector2(Math.Sign(Pos.X - npc.Center.X), 0) * 22.5f;
                            }
                            if (npc.ai[2] >= 60)
                            {
                                Vector2 Pos = player.Center + new Vector2(400, 0) * npc.localAI[0] + new Vector2(0, 50);
                                Movement(Pos, 0.6f, true);
                            }
                            if (npc.ai[2] == 10)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<XenopopperHostile>(), npc.damage / 4 + 15 * npc.alpha / 255, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] >= 100)
                            {
                                npc.ai[1] = 2;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:             //星璇机枪2
                        {
                            Vector2 Pos = player.Center + new Vector2(400, 0) * npc.localAI[0];
                            Movement(Pos, 0.4f, true);
                            npc.ai[2]++;

                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<VortexBeaterHostile2>(), npc.damage / 4 + 15 * npc.alpha / 255, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 180)
                            {
                                npc.ai[1] = (Main.rand.NextBool()) ? 3 : 5;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                            }
                        }
                        break;

                    case 3:               //代弓
                        {
                            Movement(player.Center - new Vector2(0, 300), 0.4f, false);
                            npc.spriteDirection = npc.direction = player.direction;
                            npc.ai[2]++;
                            if (npc.ai[2] == 10)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DaedalusStormbowHostile2>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 520)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = (Main.rand.NextBool()) ? 6 : 8;
                                foreach(Projectile proj in Main.projectile)
                                {
                                    if (proj.active && (proj.type == ProjectileID.HallowStar || proj.type == ModContent.ProjectileType<HolyArrowHostile>()) && proj.hostile) 
                                    {
                                        proj.Kill();
                                    }
                                }
                            }
                        }
                        break;

                    case 4:                  //玛瑙
                        {
                            int moved = (npc.ai[3] == 1) ? 600 : 350;
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * moved, 0.3f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 40)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<OnyxBlasterHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 400)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = (Main.rand.NextBool()) ? 3 : 5;
                            }
                        }
                        break;
                    case 5:                  //幻象弓
                        {
                            Vector2 Pos = player.Center - new Vector2(0, 425) + new Vector2(300, 0) * Math.Sign(player.velocity.X + 0.00114514);// - new Vector2(100, 0) * npc.direction;
                            Movement(Pos, 0.3f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 60)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmHostile2>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 500)
                            {
                                npc.ai[1] = (Main.rand.NextBool()) ? 6 : 8;

                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 6:            //喷火器准备
                        {

                            npc.ai[3] = 0;
                            Arena();
                            Vector2 Pos = player.Center - new Vector2(400, 0) * npc.direction - new Vector2(0, 250);
                            Movement(Pos, 0.3f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ArenaCenter>(), 0, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] == 40)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FlameThrowerHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 60)
                            {
                                npc.ai[1]++;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 7:             //喷火器扫射
                        {
                            Arena();
                            npc.velocity *= 0.96f;
                            npc.ai[2]++;
                            if (npc.ai[2] > 380)
                            {
                                switch (Main.rand.Next(3))
                                {
                                    case 0:
                                        npc.ai[1] = 0;
                                        break;
                                    case 1:
                                        npc.ai[1] = 4;
                                        break;
                                    case 2:
                                        npc.ai[1] = 9;
                                        break;
                                    default:
                                        break;
                                }
                                npc.ai[2] = 0;
                                if (Main.rand.NextBool()) npc.ai[3] = 1;              //隐形
                                npc.localAI[3] = 1;              //去场地
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active && proj.type == ProjectileID.GreekFire2)
                                    {
                                        proj.Kill();
                                    }
                                }
                            }
                        }
                        break;
                    case 8:              //脉冲
                        {
                            npc.ai[3] = 0;
                            Arena();
                            npc.velocity *= 0.96f;
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ArenaCenter>(), 0, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] == 20)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PulseBowHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 360)
                            {
                                foreach (Projectile proj in Main.projectile) if (proj.active && proj.type == ModContent.ProjectileType<PulseProjHostile>()) proj.Kill();
                                switch (Main.rand.Next(3))
                                {
                                    case 0:
                                        npc.ai[1] = 0;
                                        break;
                                    case 1:
                                        npc.ai[1] = 4;
                                        break;
                                    case 2:
                                        npc.ai[1] = 9;
                                        break;
                                    default:
                                        break;
                                }
                                npc.ai[2] = 0;
                                if (Main.rand.NextBool()) npc.ai[3] = 1;        //隐形
                                npc.localAI[3] = 1;               //去场地
                            }
                        }
                        break;
                    case 9:                       //电圈
                        {
                            Vector2 Pos = player.Center - new Vector2(500, 0) * npc.direction;
                            Movement(Pos, 0.3f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 40)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ElectrosphereLauncherHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 360)
                            {
                                npc.ai[1] = (Main.rand.NextBool()) ? 3 : 5;
                                npc.ai[2] = 0;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            if (npc.ai[0] == 4)
            {
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.ai[0] = 2;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.localAI[3] = 0;
            }


        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //if (npc.alpha != 255)
            {
                SpriteEffects SP = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Rectangle WingFrame = new Rectangle(0, 62 * WingsFrame, 86, 62);
                Texture2D Tex1 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/VortexRangerBoss");
                Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/VortexWings");
                Texture2D Tex3 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/VortexWingsGlow");

                for (int i = 1; i < 4; i++)
                {
                    Color color27 = Color.White * npc.Opacity * 0.6f;
                    color27 *= (float)(4 - i) / 4;
                    Vector2 value4 = npc.position - npc.velocity * i;
                    float num165 = npc.oldRot[i];
                    Main.spriteBatch.Draw(Tex2, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 9, 92), WingFrame, color27, num165, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                    Main.spriteBatch.Draw(Tex3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 9, 92), WingFrame, color27, num165, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                    Main.spriteBatch.Draw(Tex1, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, color27, num165, Tex1.Size() * 0.5f, npc.scale, SP, 0f);
                }
                Color color27A = Color.White * npc.Opacity;
                spriteBatch.Draw(Tex2, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 9, 92) + new Vector2(0, npc.gfxOffY), WingFrame, color27A, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0);
                spriteBatch.Draw(Tex3, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 9, 92) + new Vector2(0, npc.gfxOffY), WingFrame, color27A, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0);
                spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, color27A, 0, Tex1.Size() * 0.5f, 1.0f, SP, 0);

            }
            return false;
        }


        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }


        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 3 / 2;
            if (npc.ai[0] == 3) damage /= 2;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 3 / 2;
            if (npc.ai[0] == 3) damage /= 3 / 2;
        }

        public override void NPCLoot()
        {
            if (!MABWorld.DownedVortexPlayer)
            {
                NPC.ShieldStrengthTowerVortex = 1;
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.TowerDamageBolt, 0, 0f, Main.myPlayer, NPC.FindFirstNPC(NPCID.LunarTowerVortex), 0f);
            }
            MABWorld.DownedVortexPlayer = true;
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = (npc.ai[0] > 2) ? "星璇守护者" : "星璇游侠" + "被击败了，凶手是" + Main.LocalPlayer.name + "。";
            if (npc.ai[0] == 1)
            {
                int LootNum = Main.rand.Next(5) + 10;
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentVortex);
                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.VortexBeater);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.Phantasm);
                        break;
                    default:
                        break;
                }
            }
            if (npc.ai[0] == 3)
            {
                MABWorld.DownedVortexPlayerEX = true;
                int LootNum = 114 + Main.rand.Next(514);
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentVortex);
                }


                List<int> list1 = new List<int>();
                list1.Add(ItemID.VortexBreastplate);
                list1.Add(ItemID.VortexHelmet);
                list1.Add(ItemID.VortexLeggings);
                list1.Add(ItemID.Phantasm);
                list1.Add(ItemID.VortexBeater);
                list1.Add(ItemID.SDMG);
                list1.Add(ItemID.FireworksLauncher);
                for (int i = 0; i < 4; i++)
                {
                    int type = list1[Main.rand.Next(list1.Count)];
                    list1.Remove(type);
                    Item.NewItem(npc.Hitbox, type);
                }
                int r = Main.rand.Next(4);
                if (r == 1)
                {
                    Item.NewItem(npc.Hitbox, ItemID.MoonlordBullet, 999);
                }
                if (r == 2)
                {
                    Item.NewItem(npc.Hitbox, ItemID.MoonlordArrow, 999);
                }
                
            }

        }
        public override bool CheckDead()
        {
            if (npc.ai[0] == 1 && MABWorld.DownedVortexPlayer && NPC.downedMoonlord)
            {
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.ai[0] = 2;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                AllClear();
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.localAI[3] = 0;
                for (int i = 0; i < 15; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.Heart);
                }
                return false;
            }

            return true;
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            name = npc.GivenName;

        }

        private void Arena()
        {
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && (player.Center - npc.Center).Length() > 1500)
                {
                    player.velocity = Vector2.Normalize(npc.Center - player.Center) * 15;
                    player.position += Vector2.Normalize(npc.Center - player.Center) * 15;
                    player.controlDown = false;
                    player.controlHook = false;
                    player.controlJump = false;
                    player.controlLeft = false;
                    player.controlMount = false;
                    player.controlRight = false;
                    player.controlUp = false;
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (npc.ai[0] == 3 && npc.ai[3] == 1) return false;
            return null;
        }

        public override bool CheckActive()
        {
            if (npc.ai[0] == 3)
            {
                if (npc.ai[1] == 6 || npc.ai[1] == 7 || npc.ai[0] == 8)
                {
                    return false;
                }
            }
            return true;
        }
        private void AllClear()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<VortexBeaterHostile>() || proj.type == ModContent.ProjectileType<PhantasmHostile>())
                    {
                        proj.Kill();
                    }
                }
            }

            if (npc.ai[3] >= 660 || npc.ai[3] < 20)
            {
                npc.ai[3] = 0;
            }
            else
            {
                npc.ai[3] = 100;
            }


        }
    }
}