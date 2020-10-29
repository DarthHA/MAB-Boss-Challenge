using MABBossChallenge.Projectiles.PlayerBoss;
using MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.PlayerBoss
{
    [AutoloadBossHead]
    public class SolarFighterBoss : ModNPC
    {
        private int WingsFrame = 0;
        //private int Arena = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Defender");
            DisplayName.AddTranslation(GameCulture.Chinese, "日耀守护者");
            TranslationUtils.AddTranslation("SolarDefender", "Solar Defender", "日耀守护者");
            TranslationUtils.AddTranslation("SolarDefenderDescription", "The powerful warrior harnessing the power of the solar", "驾驭日耀之力的强大勇士");
            TranslationUtils.AddTranslation("SolarWarrior", "Solar Warrior", "日耀勇士");
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 38;
            npc.height = 50;
            npc.damage = 100;
            npc.defense = 40;
            npc.lifeMax = 50000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.buffImmune[BuffID.OnFire] = true;
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
        public override void AI()
        {
            if (!MABWorld.DownedSolarPlayerEX)
            {
                NPC.ShieldStrengthTowerSolar = 5;
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
            if (npc.ai[0] > 0 && ModLoader.GetMod("FargowiltasSouls") != null)
            {
                npc.buffImmune[ModLoader.GetMod("FargowiltasSouls").BuffType("GodEater")] = true;
                npc.buffImmune[ModLoader.GetMod("FargowiltasSouls").BuffType("OceanicMaul")] = true;
                npc.buffImmune[ModLoader.GetMod("FargowiltasSouls").BuffType("TimeFrozen")] = true;
                npc.buffImmune[ModLoader.GetMod("FargowiltasSouls").BuffType("MutantNibble")] = true;
            }
            if (player.Center.X - npc.Center.X >= 0) { npc.spriteDirection = 1; npc.direction = 1; }
            if (player.Center.X - npc.Center.X < 0) { npc.spriteDirection = -1; npc.direction = -1; }
            npc.frameCounter++;
            if (npc.frameCounter > 5)
            {
                npc.frameCounter = 0;
                WingsFrame++;
                if (WingsFrame > 3) WingsFrame = 0;
            }
            if (npc.velocity.Length() > 9)
            {
                for (int num15 = 0; num15 < 1; num15++)
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default, 1f)];
                    dust.noGravity = true;
                    dust.scale = 1.7f;
                    dust.fadeIn = 0.5f;
                    dust.velocity *= 5f;
                    //dust.shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.ArmorSetDye(), Main.LocalPlayer);
                }
            }

            if (npc.ai[0] == 0)               //开局
            {
                if (!MABWorld.DownedSolarPlayer)
                {
                    npc.ai[2]++;
                    if (npc.ai[2] == 1)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("SolarDefender"), TranslationUtils.GetTranslation("SolarDefenderDescription"));
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
                    case 0:                 //破晓
                        {
                            Vector2 Pos = player.Center - new Vector2(400, 0) * npc.direction;
                            Movement(Pos, 0.5f, true);
                            npc.ai[2]++;

                            if (npc.ai[2] % 40 == 39 && npc.ai[2] > 40)
                            {
                                Main.PlaySound(SoundID.Item1, npc.Center);
                                Vector2 ShootVel = Vector2.Normalize(player.Center - npc.Center);
                                Projectile.NewProjectile(npc.Center, ShootVel * 20 + player.velocity * 0.33f, ModContent.ProjectileType<DayBreakHostile>(), npc.damage / 4, 0, Main.myPlayer);
                            }
                            if (npc.ai[2] > 240)
                            {
                                npc.ai[1] = 1;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 1:                  //日耀喷发剑
                        {
                            Movement(player.Center, 0.2f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] > 50 && npc.ai[2] % 50 == 0)
                            {
                                Main.PlaySound(SoundID.Item116, npc.Center);
                                Vector2 ShootVel = player.Center - npc.Center;
                                float r = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                                int protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarEruptionHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI, r);
                                Main.projectile[protmp].localAI[0] = Main.rand.Next(2) * 2 - 1;
                            }
                            if (npc.ai[2] > 50 + 50 * 5)
                            {
                                npc.ai[1] = 2;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:             //冲刺
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] < 100)
                            {
                                Vector2 Pos = player.Center - new Vector2(500, 0) * npc.direction;
                                Movement(Pos, 0.5f, true);
                                if (npc.ai[2] == 30) Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarShieldHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 100)
                            {

                                if (npc.ai[2] % 50 == 10)
                                {
                                    Main.PlaySound(SoundID.Item109, npc.Center);
                                    npc.localAI[3] = npc.spriteDirection;
                                    Vector2 RamPos = Vector2.Normalize(player.Center - npc.Center + player.velocity * 15);
                                    RamPos.Normalize();
                                    npc.localAI[0] = RamPos.X * 35;
                                    npc.localAI[1] = RamPos.Y * 35;

                                }
                                if (npc.ai[2] % 50 > 10)
                                {
                                    npc.direction = (int)npc.localAI[3];
                                    npc.spriteDirection = (int)npc.localAI[3];
                                    npc.velocity = new Vector2(npc.localAI[0], npc.localAI[1]);
                                }
                                if (npc.ai[2] % 50 < 10)
                                {
                                    npc.velocity *= 0.9f;
                                }
                                if (npc.ai[2] > 10 + 50 * 6 + 100)
                                {
                                    npc.ai[1] = 0;
                                    npc.ai[2] = 0;
                                    npc.ai[3] = 0;
                                }

                            }
                        }
                        break;

                    default:
                        break;

                }
            }


            if (npc.ai[0] == 2)         //变换形态
            {
                ClearAll();
                npc.velocity *= 0.8f;
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/The Last Guardian");
                    npc.lifeMax = 300000;
                    npc.damage = 200;
                    npc.defense = 60;
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarRitual>(), 0, 0, default, npc.whoAmI);
                }
                if (npc.ai[2] == 120)
                {
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    //Arena = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarArena>(), 0, 0, Main.myPlayer, npc.whoAmI);
                    Main.NewText(TranslationUtils.GetTranslation("TrueFight"), Color.Orange);
                    npc.GivenName = TranslationUtils.GetTranslation("SolarWarrior");
                    //Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<HelFireHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);

                }
                if (npc.ai[2] >= 120 && npc.ai[2] <= 240)
                {
                    npc.HealEffect(500);
                    if (npc.life < npc.lifeMax - 500)
                    {
                        npc.life += 500;
                    }
                    else
                    {
                        npc.life = npc.lifeMax;
                    }
                }
                if (npc.ai[2] > 240)                    //4秒
                {
                    npc.dontTakeDamage = false;
                    npc.life = npc.lifeMax;
                    npc.ai[0] = 3;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;              //
                }
            }




            if (npc.ai[0] == 3)
            {
                switch (npc.ai[1])
                {
                    case 0:                 //破晓
                        {

                            Vector2 Pos = player.Center - new Vector2(400, 0) * npc.direction;
                            Movement(Pos, 0.5f, true);
                            npc.ai[2]++;

                            if (npc.ai[2] % 35 == 34 && npc.ai[2] > 35 && npc.ai[2] <= 245)
                            {
                                Main.PlaySound(SoundID.Item1, npc.Center);
                                Vector2 ShootVel = Vector2.Normalize(player.Center - npc.Center);
                                Projectile.NewProjectile(npc.Center, ShootVel * 23f + player.velocity * 0.1f, ModContent.ProjectileType<DayBreakHostile>(), npc.damage / 4, 0, Main.myPlayer);
                                ShootVel = Vector2.Normalize(player.Center - npc.Center + new Vector2(0, -800));
                                Projectile.NewProjectile(npc.Center + new Vector2(0, 800), ShootVel * 23f + player.velocity * 0.3f, ModContent.ProjectileType<DayBreakHostile>(), npc.damage / 4, 0, Main.myPlayer);
                                ShootVel = Vector2.Normalize(player.Center - npc.Center + new Vector2(0, 800));
                                Projectile.NewProjectile(npc.Center - new Vector2(0, 800), ShootVel * 23f + player.velocity * 0.3f, ModContent.ProjectileType<DayBreakHostile>(), npc.damage / 4, 0, Main.myPlayer);
                            }
                            if (npc.ai[2] > 275)
                            {
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 1 : 6;
                                //npc.ai[1] = 1;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 1:                  //日耀喷发剑
                        {

                            Movement(player.Center, 0.2f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] > 50 && npc.ai[2] % 50 == 0 && npc.ai[2] <= 50 + 50 * 5)
                            {
                                Main.PlaySound(SoundID.Item116, npc.Center);
                                Vector2 ShootVel = player.Center - npc.Center;
                                float r = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                                int protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarEruptionHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI, r);
                                Main.projectile[protmp].localAI[0] = Main.rand.Next(2) * 2 - 1;
                            }
                            if (npc.ai[2] > 50 + 50 * 6)
                            {
                                //npc.ai[1] = 2;
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 2 : 4;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:             //冲刺
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] < 100)
                            {
                                Vector2 Pos = player.Center - new Vector2(500, 0) * npc.direction;
                                Movement(Pos, 0.5f, true);
                                if (npc.ai[2] == 30) Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarShieldHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 100)
                            {

                                if (npc.ai[2] % 50 == 10)
                                {
                                    Main.PlaySound(SoundID.Item109, npc.Center);
                                    npc.localAI[3] = npc.spriteDirection;
                                    Vector2 RamPos = Vector2.Normalize(player.Center - npc.Center + player.velocity * 15);
                                    RamPos.Normalize();
                                    npc.localAI[0] = RamPos.X * 35;
                                    npc.localAI[1] = RamPos.Y * 35;
                                    for (float i = -MathHelper.Pi / 2; i <= MathHelper.Pi / 2; i += MathHelper.Pi / 4)
                                    {
                                        float r = (float)Math.Atan2(RamPos.Y, RamPos.X);
                                        int protmp = Projectile.NewProjectile(npc.Center, (i + r).ToRotationVector2() * 15, ProjectileID.DD2FlameBurstTowerT3Shot, npc.damage / 4, 0, player.whoAmI);
                                        Main.projectile[protmp].hostile = true;
                                        Main.projectile[protmp].friendly = false;
                                        Main.projectile[protmp].scale = 2.0f;
                                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                                    }

                                }
                                if (npc.ai[2] % 50 > 10)
                                {
                                    npc.direction = (int)npc.localAI[3];
                                    npc.spriteDirection = (int)npc.localAI[3];
                                    npc.velocity = new Vector2(npc.localAI[0], npc.localAI[1]);
                                    if (npc.ai[2] % 5 == 3)
                                    {
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarRamFireball>(), npc.damage / 4, 0);
                                    }
                                }
                                if (npc.ai[2] % 50 < 10)
                                {
                                    npc.velocity *= 0.9f;
                                }
                                if (npc.ai[2] > 10 + 50 * 6 + 100)
                                {
                                    //npc.ai[1] = 3;
                                    npc.ai[1] = (Main.rand.Next(2) == 0) ? 3 : 7;
                                    npc.ai[2] = 0;
                                    npc.ai[3] = 0;
                                }

                            }
                        }
                        break;
                    case 3:              //泰拉狱火球
                        {

                            Vector2 Pos = player.Center - new Vector2(500, 0) * npc.direction;
                            if (npc.ai[2] % 140 <= 70) Pos += new Vector2(0, 100);
                            else Pos -= new Vector2(0, 100);
                            Movement(Pos, 0.5f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 100)
                            {
                                Main.PlaySound(SoundID.Item1, npc.Center);
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<HelFireHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI);
                            }
                            if (npc.ai[2] > 540)
                            {
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 0 : 5;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                            }
                        }
                        break;
                    case 4:               //星怒
                        {


                            npc.ai[2]++;
                            if (npc.ai[2] < 90)
                            {
                                Movement(player.Center - new Vector2(0, 200), 0.5f, true);
                            }
                            if (npc.ai[2] >= 90)
                            {

                                if (npc.ai[2] % 40 == 10)
                                {
                                    npc.localAI[3] = npc.spriteDirection;
                                    Vector2 RamPos = Vector2.Normalize(player.Center - npc.Center);
                                    RamPos.Normalize();
                                    npc.localAI[0] = RamPos.X * 35;
                                    npc.localAI[1] = RamPos.Y * 35;
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StarWrathHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                                }
                                if (npc.ai[2] % 40 > 10)
                                {
                                    npc.direction = (int)npc.localAI[3];
                                    npc.spriteDirection = (int)npc.localAI[3];
                                    npc.velocity = new Vector2(npc.localAI[0], npc.localAI[1]);
                                }
                                if (npc.ai[2] % 40 < 10)
                                {
                                    npc.velocity *= 0.9f;
                                }
                                if (npc.ai[2] > 10 + 40 * 6 + 90)
                                {
                                    npc.ai[1] = Main.rand.NextBool() ? 3 : 7;
                                    npc.ai[2] = 0;
                                    npc.ai[3] = 0;
                                }

                            }
                        }
                        break;
                    case 5:              //破晓2
                        {
                            Vector2 Pos;
                            if (npc.ai[2] % 20 == 10 && npc.ai[2] > 30)
                            {
                                npc.localAI[0] = Main.rand.Next(-400, 400);
                            }
                            Pos = player.Center - new Vector2(400, 0) * npc.direction + new Vector2(0, npc.localAI[0]);

                            if (npc.ai[2] > 330)
                            {
                                Pos = player.Center - new Vector2(400, 0) * npc.direction + new Vector2(0, -500);
                            }
                            Movement(Pos, 0.4f, true);

                            npc.ai[2]++;
                            if (npc.ai[2] < 330 && npc.ai[2] > 30)
                            {
                                if (npc.ai[2] % 20 == 10)                   //普通长矛
                                {
                                    // Projectile.NewProjectile(npc.Center - new Vector2(Main.rand.Next(1000) - 500, Main.rand.Next(200)), Vector2.Zero, ModContent.ProjectileType<DayBreakHostile2>(), npc.damage / 4, 0, player.whoAmI, -1);
                                    Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * (Main.rand.Next(30) + 10), ModContent.ProjectileType<DayBreakHostile2>(), npc.damage / 4, 0, player.whoAmI, -1);
                                }
                            }
                            if (npc.ai[2] == 330)                         //大长矛
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DayBreakHostile2>(), npc.damage / 3, 0, player.whoAmI, npc.whoAmI);
                            }
                            if (npc.ai[2] > 460)
                            {
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 1 : 6;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                        break;
                    case 6:               //阳炎之怒
                        {
                            Movement(player.Center, 0.2f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SunFuryHostile>(), npc.damage / 4, 0, Main.myPlayer, npc.whoAmI, 0);
                            }

                            if (npc.ai[2] == 151)
                            {
                                Main.PlaySound(SoundID.Item1, npc.Center);
                                Vector2 ShootVel = player.Center - npc.Center;
                                ShootVel = Vector2.Normalize(ShootVel + player.velocity * 10);
                                Projectile.NewProjectile(npc.Center, ShootVel * 30, ModContent.ProjectileType<SunFuryHostile>(), npc.damage / 4, 0, player.whoAmI, npc.whoAmI, 1);
                            }
                            if (npc.ai[2] > 361)
                            {
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 2 : 4;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                        break;
                    case 7:                         //熔岩大剑1
                        {
                            if (player.velocity.Length() > 15)
                            {
                                if (player.velocity != Vector2.Zero)
                                {
                                    player.velocity = Vector2.Normalize(player.velocity) * 15;
                                }
                            }
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 100, 0.2f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FlareGSword>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] >= 180)
                            {

                                if (npc.ai[2] % 15 == 5)
                                {
                                    npc.localAI[3] = npc.spriteDirection;
                                    Vector2 RamPos = Vector2.Normalize(player.Center - npc.Center + player.velocity * 5);
                                    float RamR = (float)Math.Atan2(RamPos.Y, RamPos.X);
                                    RamR += (Main.rand.Next(2) * 2 - 1) * (Main.rand.NextFloat() * MathHelper.Pi / 10 + MathHelper.Pi / 16);
                                    RamPos = RamR.ToRotationVector2();
                                    npc.localAI[0] = RamPos.X * 60;
                                    npc.localAI[1] = RamPos.Y * 60;
                                    Main.PlaySound(SoundID.Item71, npc.Center);
                                }
                                if (npc.ai[2] % 15 > 5)
                                {
                                    npc.direction = (int)npc.localAI[3];
                                    npc.spriteDirection = (int)npc.localAI[3];
                                    npc.velocity = new Vector2(npc.localAI[0], npc.localAI[1]);
                                }
                                if (npc.ai[2] % 15 == 10)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<FlareLaser>(), npc.damage / 4, 0);
                                }
                                if (npc.ai[2] % 15 < 5)
                                {
                                    npc.velocity *= 0.96f;
                                }

                            }
                            if (npc.ai[2] > 180 + 15 * 10)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1]++;
                            }

                        }
                        break;
                    case 8:              //熔岩大剑2
                        {
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 300, 0.4f, true);
                            npc.ai[2]++;
                            if (npc.ai[2] > 120)
                            {
                                npc.ai[1] = (Main.rand.Next(2) == 0) ? 0 : 5;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
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

            SpriteEffects SP = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle WingFrame = new Rectangle(0, 62 * WingsFrame, 86, 62);
            Texture2D Tex1 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/SolarFighterBoss");
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/SolarWings");


            for (int i = 1; i < 4; i++)
            {
                Color color27 = Color.White * npc.Opacity * 0.6f;
                color27 *= (float)(4 - i) / 4;
                Vector2 value4 = npc.position - npc.velocity * i;
                float num165 = npc.oldRot[i];
                Main.spriteBatch.Draw(Tex2, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(0, 90), WingFrame, color27, num165, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                Main.spriteBatch.Draw(Tex1, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, color27, num165, Tex1.Size() * 0.5f, npc.scale, SP, 0f);
            }
            spriteBatch.Draw(Tex2, npc.Center - Main.screenPosition + new Vector2(0, 90) + new Vector2(0, npc.gfxOffY), WingFrame, Color.White, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0);
            spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, Color.White, 0, Tex1.Size() * 0.5f, 1.0f, SP, 0);
            return false;
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage /= 3 / 2;
            if (npc.ai[0] == 2) damage /= 2;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 3 / 2;
            if (npc.ai[0] == 2) damage /= 2;
        }

        public override void NPCLoot()
        {

            MABWorld.DownedSolarPlayer = true;
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = (npc.ai[0] > 2) ? "日耀守护者" : "日耀勇士" + "被击败了，凶手是" + Main.LocalPlayer.name + "。";
            if (npc.ai[0] == 1)
            {
                int LootNum = Main.rand.Next(5) + 10;
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentSolar);
                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.DayBreak);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.SolarEruption);
                        break;
                    default:
                        break;
                }
            }
            if (npc.ai[0] == 3)
            {
                if (!MABWorld.DownedSolarPlayer)
                {
                    NPC.ShieldStrengthTowerSolar = 1;
                }
                MABWorld.DownedSolarPlayerEX = true;
                int LootNum = Main.rand.Next(10) + 30;
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentSolar);
                }
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.DayBreak);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.SolarEruption);
                        break;
                    case 2:
                        Item.NewItem(npc.Hitbox, ItemID.Meowmere);
                        break;
                    case 3:
                        Item.NewItem(npc.Hitbox, ItemID.StarWrath);
                        break;
                    case 4:
                        Item.NewItem(npc.Hitbox, ItemID.Terrarian);
                        break;
                    default:
                        break;
                }

                switch (Main.rand.Next(3))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.SolarFlareBreastplate);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.SolarFlareLeggings);
                        break;
                    case 2:
                        Item.NewItem(npc.Hitbox, ItemID.SolarFlareHelmet);
                        break;
                }
            }

        }

        public override bool CheckDead()
        {
            if (npc.ai[0] <= 1 &&  MABWorld.DownedSolarPlayer && NPC.downedMoonlord)
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
                return false;
            }

            return true;
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            name = npc.GivenName;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return (npc.ai[0] != 3 || npc.ai[1] != 7);
        }
        private void ClearAll()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.ai[0] == npc.whoAmI)
                {
                    if (proj.type == ModContent.ProjectileType<SolarShieldHostile>() || proj.type == ModContent.ProjectileType<SolarEruptionHostile>())
                    {
                        proj.Kill();
                    }
                }
            }
        }

        /*
        private int RandAttack(int a)
        {
            int result = 0;
            while (true)
            {
                result = Main.rand.Next(7);
                bool IsRight = true;
                if (result == a) IsRight = false;
                if ((result == 6 && a == 1) || (result == 1 && a == 6)) IsRight = false;
                if ((result == 2 && a == 4) || (result == 4 && a == 2)) IsRight = false;
                if ((result == 0 && a == 5) || (result == 5 && a == 0)) IsRight = false;
                if (IsRight) break;
            }
            return result;
        }
        */
    }
}