using MABBossChallenge.Buffs;
using MABBossChallenge.Projectiles.PlayerBoss;
using MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj;
using MABBossChallenge.Utils;
using MentalAIBoost.Projectiles.DestroyerEXProj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.PlayerBoss
{
    [AutoloadBossHead]
    public class NebulaMageBoss : ModNPC
    {
        private int WingsFrame = 0;
        private int StateRage = 0;
        private float CurrentAI = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Defender");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云守护者");
            TranslationUtils.AddTranslation("NebulaDefender", "Nebula Defender", "星云守护者");
            TranslationUtils.AddTranslation("NebulaDefenderDescription", "The mysterious warlock manipulating the power of the nebula", "操纵星云之力的神秘术士");
            TranslationUtils.AddTranslation("NebulaMage", "Nebula Mage", "星云术士");
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
            npc.HitSound = SoundID.NPCHit9;
            //npc.DeathSound = SoundID.NPCDeath11;
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

        private void TP(Vector2 Pos)
        {
            for (float i = 0; i < 48; i++)
            {
                var dust = Dust.NewDustDirect(npc.Center, 0, 0, MyDustId.PurpleHighFx, 0, 0, 100, Color.White, 2f);
                dust.noGravity = true;
                dust.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5;
                dust.noLight = false;
            }

            npc.Center = Pos;

            for (float i = 0; i < 48; i++)
            {
                var dust = Dust.NewDustDirect(npc.Center, 0, 0, MyDustId.PurpleHighFx, 0, 0, 100, Color.White, 2f);
                dust.noGravity = true;
                dust.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5;
                dust.noLight = false;
            }
        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (npc.ai[0] == 3)
            {
                if (npc.ai[1] == 7 || npc.ai[1] == 9) 
                {
                    return true;
                }
            }
            return false;
        }

        public override void AI()
        {
            if (!MABWorld.DownedNebulaPlayer)
            {
                NPC.ShieldStrengthTowerNebula = 5;
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
            if (player.Center.X - npc.Center.X >= 0) { npc.spriteDirection = 1; npc.direction = 1; }
            if (player.Center.X - npc.Center.X < 0) { npc.spriteDirection = -1; npc.direction = -1; }
            npc.frameCounter++;
            if (npc.frameCounter > 4)
            {
                npc.frameCounter = 0;
                WingsFrame++;
                if (WingsFrame > 3) WingsFrame = 0;
            }

            if (npc.ai[0] == 0)
            {
                npc.GivenName = TranslationUtils.GetTranslation("NebulaDefender");
                if (!MABWorld.DownedNebulaPlayer)
                {
                    npc.ai[2]++;
                    if (npc.ai[2] == 1)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("NebulaDefender"), TranslationUtils.GetTranslation("NebulaDefenderDescription"));
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
                    case 0:           //星云烈焰
                        {
                            Vector2 Pos = player.Center - Vector2.Normalize(player.velocity) * 300;
                            if (player.velocity == Vector2.Zero)
                            {
                                Pos = npc.Center;
                                npc.velocity = Vector2.Zero;
                            }
                            Movement(Pos, 0.3f, true);

                            for (int i = 0; i < 4; i++)
                            {
                                Dust dust11 = Main.dust[Dust.NewDust(npc.Center + new Vector2(npc.direction * 5, 0), 0, 0, 242, npc.direction * 2, 0f, 150, default, 1.3f)];
                                Dust dust2 = dust11;
                                dust2.velocity *= 0f;
                                dust11.noGravity = true;
                                dust11.fadeIn = 1f;
                                dust2 = dust11;
                                dust2.velocity += npc.velocity;
                                if (Main.rand.Next(2) == 0)
                                {
                                    dust2 = dust11;
                                    dust2.scale += Main.rand.NextFloat();
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        //dust11.customData = npc;
                                    }
                                }
                            }

                            npc.ai[2]++;
                            if (npc.ai[2] % 100 == 1)
                            {
                                Vector2 MovePos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 300;
                                npc.localAI[0] = MovePos.X;
                                npc.localAI[1] = MovePos.Y;
                            }
                            if (npc.ai[2] > 100 && npc.ai[2] % 100 == 2)
                            {
                                TP(new Vector2(npc.localAI[0], npc.localAI[1]));
                            }

                            if (npc.ai[2] > 100 && npc.ai[2] % 100 > 10 && npc.ai[2] % 100 < 80)
                            {
                                if (npc.ai[2] % 17 == 5)
                                {
                                    Main.PlaySound(SoundID.Item20, npc.Center);
                                    Vector2 ShootVel = player.Center - npc.Center;
                                    float ShootVelR = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                                    if (Main.rand.Next(3) == 0)
                                    {
                                        Projectile.NewProjectile(npc.Center, (ShootVelR + Main.rand.NextFloat() * MathHelper.Pi / 6 - MathHelper.Pi / 12).ToRotationVector2() * 10, ModContent.ProjectileType<NebulaBlazeHostile>(), npc.damage / 3, 0, npc.target, 1);
                                    }
                                    else
                                    {
                                        Projectile.NewProjectile(npc.Center, (ShootVelR + Main.rand.NextFloat() * MathHelper.Pi / 6 - MathHelper.Pi / 12).ToRotationVector2() * 7, ModContent.ProjectileType<NebulaBlazeHostile>(), npc.damage / 4, 0, npc.target, 0);
                                    }

                                    //Projectile.NewProjectile(npc.Center, ShootVelR.ToRotationVector2()*5, ModContent.ProjectileType<NebulaArcanumHostile>(), 10, 10, player.whoAmI);
                                }
                            }
                            if (npc.ai[2] > 400)
                            {
                                npc.ai[1] = 1;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                            }
                        }
                        break;
                    case 1:              //星云强化焰
                        {
                            npc.ai[2]++;
                            Movement(player.Center + new Vector2(0, -300), 0.3f, false);
                            if (npc.ai[2] % 9 == 2 && npc.ai[2] <= 300)
                            {
                                int FlareType = 0;
                                switch (Main.rand.Next(3))
                                {
                                    case 0:
                                        FlareType = ModContent.ProjectileType<LifeBoosterHostile>();
                                        break;
                                    case 1:
                                        FlareType = ModContent.ProjectileType<ManaBoosterHostile>();
                                        break;
                                    case 2:
                                        FlareType = ModContent.ProjectileType<DamageBoosterHostile>();
                                        break;
                                    default:
                                        break;
                                }
                                Projectile.NewProjectile(player.Center + player.velocity * 30 + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * (300 + Main.rand.Next(200)), Vector2.Zero, FlareType, npc.damage / 4, 0, player.whoAmI);

                            }
                            if (npc.ai[2] > 500)
                            {
                                npc.ai[1] = 2;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:                //星云奥秘
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] % 100 == 1)
                            {
                                Vector2 PrePos = player.Center + Vector2.Normalize(player.velocity) * 500;
                                Vector2 PrePosV = Vector2.Normalize(new Vector2(-player.velocity.Y, player.velocity.X)) * 600;
                                if (player.velocity == Vector2.Zero)
                                {
                                    switch (Main.rand.Next(4))
                                    {
                                        case 0:
                                            PrePos = player.Center + new Vector2(0, -300);
                                            PrePosV = new Vector2(-600, 0);
                                            break;
                                        case 1:
                                            PrePos = player.Center + new Vector2(0, 300);
                                            PrePosV = new Vector2(600, 0);
                                            break;
                                        case 2:
                                            PrePos = player.Center + new Vector2(300, 0);
                                            PrePosV = new Vector2(0, -600);
                                            break;
                                        case 3:
                                            PrePos = player.Center + new Vector2(-300, 0);
                                            PrePosV = new Vector2(0, 600);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                TP(PrePos + PrePosV);
                                npc.velocity = -PrePosV / 30;
                            }
                            if (npc.ai[2] % 100 > 1 && npc.ai[2] % 100 < 60)
                            {
                                if (npc.ai[2] % 15 == 0)
                                {
                                    Main.PlaySound(SoundID.Item117, npc.Center);
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<NebulaArcanumHostile>(), (int)(npc.damage * 0.3), 0, player.whoAmI);
                                }
                            }
                            if (npc.ai[2] % 100 > 60)
                            {
                                npc.velocity *= 0.95f;
                            }



                            if (npc.ai[2] > 100 * 3)
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
                npc.velocity *= 0.8f;
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    AllClear();
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/The Last Guardian");
                    npc.lifeMax = 300000;
                    npc.damage = 180;
                    npc.defense = 40;
                    if (!Main.expertMode)
                    {
                        npc.damage = 120;
                        npc.lifeMax = 200000;
                        npc.defense = 20;
                    }
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<NebulaRitual>(), 0, 0, default, npc.whoAmI);
                }
                if (npc.ai[2] == 120)
                {
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    //Arena = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SolarArena>(), 0, 0, Main.myPlayer, npc.whoAmI);
                    Main.NewText(TranslationUtils.GetTranslation("TrueFight"), Color.MediumPurple);
                    npc.GivenName = TranslationUtils.GetTranslation("NebulaMage");
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
                if (npc.ai[2] > 240)                    //4秒
                {
                    npc.dontTakeDamage = false;
                    npc.life = npc.lifeMax;
                    npc.ai[0] = 3;
                    npc.ai[1] = 0;              //
                    npc.ai[2] = 0;
                }
            }

            if (npc.ai[0] == 3)             //二阶段
            {
                if (StateRage <= 0 && npc.life < npc.lifeMax / 4 * 3) 
                {
                    CurrentAI = npc.ai[1];
                    AllClear();
                    StateRage = 1;
                    npc.ai[1] = 8;           //磁球
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.localAI[0] = 0;
                    npc.localAI[1] = 0;
                    npc.localAI[2] = 0;
                    npc.localAI[3] = 0;
                }
                if (StateRage <= 1 && npc.life < npc.lifeMax / 2)
                {
                    CurrentAI = npc.ai[1];
                    AllClear();
                    StateRage = 2;
                    npc.ai[1] = 5;           //水刃
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.localAI[0] = 0;
                    npc.localAI[1] = 0;
                    npc.localAI[2] = 0;
                    npc.localAI[3] = 0;
                }
                if (StateRage <= 2 && npc.life < npc.lifeMax / 4)
                {
                    CurrentAI = npc.ai[1];
                    AllClear();
                    StateRage = 3;
                    npc.ai[1] = 3;              //月曜
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.localAI[0] = 0;
                    npc.localAI[1] = 0;
                    npc.localAI[2] = 0;
                    npc.localAI[3] = 0;
                }
                switch (npc.ai[1])
                {
                    case 0:                //星云烈焰
                        {
                            Vector2 Pos = player.Center - Vector2.Normalize(player.velocity) * 250;
                            if (player.velocity == Vector2.Zero)
                            {
                                Pos = npc.Center;
                                npc.velocity = Vector2.Zero;
                            }
                            Movement(Pos, 0.3f, true);

                            for (int i = 0; i < 4; i++)
                            {
                                Dust dust11 = Main.dust[Dust.NewDust(npc.Center + new Vector2(npc.direction * 5, 0), 0, 0, 242, npc.direction * 2, 0f, 150, default, 1.3f)];
                                Dust dust2 = dust11;
                                dust2.velocity *= 0f;
                                dust11.noGravity = true;
                                dust11.fadeIn = 1f;
                                dust2 = dust11;
                                dust2.velocity += npc.velocity;
                                if (Main.rand.Next(2) == 0)
                                {
                                    dust2 = dust11;
                                    dust2.scale += Main.rand.NextFloat();
                                }
                            }

                            npc.ai[2]++;
                            if (npc.ai[2] % 100 == 1)
                            {
                                Vector2 MovePos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 250;
                                npc.localAI[0] = MovePos.X;
                                npc.localAI[1] = MovePos.Y;
                            }
                            if (npc.ai[2] > 100 && npc.ai[2] % 100 == 2)
                            {
                                TP(new Vector2(npc.localAI[0], npc.localAI[1]));
                            }

                            if (npc.ai[2] > 100 && npc.ai[2] % 100 > 10 && npc.ai[2] % 100 < 80)
                            {
                                if (npc.ai[2] % 15 == 5)
                                {
                                    Main.PlaySound(SoundID.Item20, npc.Center);
                                    Vector2 ShootVel = player.Center + player.velocity * 5 - npc.Center;
                                    float ShootVelR = ShootVel.ToRotation();
                                    Projectile.NewProjectile(npc.Center, (ShootVelR + Main.rand.NextFloat() * MathHelper.Pi / 3 - MathHelper.Pi / 6).ToRotationVector2() * 10, ModContent.ProjectileType<NebulaBlazeHostileEX>(), npc.damage / 4, 0, npc.target, 1);


                                    //Projectile.NewProjectile(npc.Center, ShootVelR.ToRotationVector2()*5, ModContent.ProjectileType<NebulaArcanumHostile>(), 10, 10, player.whoAmI);
                                }
                            }
                            if (npc.ai[2] > 400)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 1 : 6;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                            }
                        }
                        break;
                    case 1:                //星云强化焰
                        {
                            npc.ai[2]++;
                            Movement(player.Center + new Vector2(0, -300), 0.3f, false);
                            if (npc.ai[2] < 300)
                            {
                                if (npc.ai[2] % 60 == 0)
                                {
                                    int FlareType = 0;
                                    switch (Main.rand.Next(3))
                                    {
                                        case 0:
                                            FlareType = ModContent.ProjectileType<LifeBoosterHostile>();
                                            break;
                                        case 1:
                                            FlareType = ModContent.ProjectileType<ManaBoosterHostile>();
                                            break;
                                        case 2:
                                            FlareType = ModContent.ProjectileType<DamageBoosterHostile>();
                                            break;
                                        default:
                                            break;
                                    }
                                    npc.localAI[3] = FlareType;
                                    npc.localAI[0] = player.Center.X + ((player.velocity.Length() > 5) ? player.velocity.X * 15 : player.velocity.X * 5);
                                    npc.localAI[1] = player.Center.Y + ((player.velocity.Length() > 5) ? player.velocity.Y * 15 : player.velocity.Y * 5);
                                    npc.localAI[2] = Main.rand.NextFloat() * MathHelper.TwoPi;
                                }

                                if (npc.ai[2] % 60 < 34)
                                {
                                    float r = npc.ai[2] / 34 * MathHelper.TwoPi;
                                    Vector2 Center = new Vector2(npc.localAI[0], npc.localAI[1]);
                                    int protmp = Projectile.NewProjectile(Center + (r + npc.localAI[2]).ToRotationVector2() * 600, Vector2.Zero, (int)npc.localAI[3], npc.damage / 5, 0, player.whoAmI, 1);
                                    Main.projectile[protmp].localAI[0] = Center.X;
                                    Main.projectile[protmp].localAI[1] = Center.Y;
                                }
                            }
                            if (npc.ai[2] > 400)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 2 : 7;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                                npc.localAI[3] = 0;
                            }
                        }
                        break;
                    case 2:                    //星云奥秘
                        {
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, false);

                            npc.ai[2]++;
                            if (npc.ai[2] % 100 == 51 && npc.ai[2] < 400) 
                            {
                                Vector2 OldPos = npc.Center;
                                Vector2 PrePos;
                                if (PointMulti(((player.Center - npc.Center).ToRotation() + MathHelper.Pi / 2).ToRotationVector2(), player.velocity) > 0)
                                {
                                    PrePos = player.Center + ((player.Center - npc.Center).ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 300;
                                }
                                else
                                {
                                    PrePos = player.Center + ((player.Center - npc.Center).ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 300;
                                }
                                
                                Vector2 TelePos = PrePos * 2 - npc.Center;
                                if (player.Distance(TelePos) > 1000)
                                {
                                    TelePos = player.Center + Vector2.Normalize(TelePos - player.Center) * 1000;
                                }
                                TP(TelePos);
                                int dustamount = (int)((npc.Center - OldPos).Length() / 8);
                                int projamount = Terraria.Utils.Clamp((int)((npc.Center - OldPos).Length() / 150), 1, 5);
                                for (int i = 0; i < dustamount; i++)
                                {
                                    Vector2 DustPos = OldPos + (npc.Center - OldPos) * Main.rand.NextFloat();
                                    Dust dust = Dust.NewDustDirect(DustPos, 1, 1, MyDustId.PurpleHighFx);
                                    dust.noGravity = true;
                                }
                                Main.PlaySound(SoundID.Item117, npc.Center);
                                for (int i = 0; i < projamount; i++)
                                {
                                    Vector2 ProjPos = OldPos + (npc.Center - OldPos) * i / projamount;
                                    if (player.Distance(ProjPos) >= 150)
                                    {
                                        Projectile.NewProjectile(ProjPos, Vector2.Zero, ModContent.ProjectileType<NebulaArcanumHostile>(), (int)(npc.damage * 0.25), 0, player.whoAmI);
                                    }
                                }
                            }
   
                            if (npc.ai[2] > 420)
                            {
                                foreach(Projectile proj in Main.projectile)
                                {
                                    if(proj.active && proj.type == ModContent.ProjectileType<NebulaArcanumHostile>())
                                    {
                                        proj.timeLeft = 30;
                                        proj.ai[0] = 470;
                                    }
                                }
                                npc.ai[1] = Main.rand.NextBool() ? 0 : 4;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 3:                   //仪式1
                        {
                            npc.velocity *= 0.8f;
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                npc.dontTakeDamage = true;
                                TP(player.Center + new Vector2(0, -300));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Ritual>(), 1, 0, default, npc.whoAmI);
                            }

                            if (npc.ai[2] == 30)
                            {
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active && proj.type == ModContent.ProjectileType<Ritual>())
                                    {
                                        if ((int)proj.ai[0] == npc.whoAmI)
                                        {
                                            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 3)
                                            {
                                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LunarFlareBookHostile>(), 1, 0, default, proj.whoAmI, i);
                                            }
                                        }
                                    }
                                }

                            }
                            
                            if (npc.ai[2] > 120)
                            {
                                if (npc.ai[2] < 160)
                                {
                                    int count = (Main.rand.Next(4) <= 2) ? 1 : 2;
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (Main.rand.Next(3) < 2)
                                        {
                                            Vector2 Pos = npc.Center + (MathHelper.Pi * Main.rand.NextFloat()).ToRotationVector2() * Main.rand.Next(1100);
                                            Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<LunarFlareMark>(), npc.damage / 4, 0);
                                        }
                                        else
                                        {
                                            Vector2 Pos = npc.Center + (-MathHelper.Pi * Main.rand.NextFloat()).ToRotationVector2() * Main.rand.Next(1100);
                                            Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<LunarFlareMark>(), npc.damage / 4, 0);
                                        }
                                    }
                                }

                                if (npc.ai[2] > 240)
                                {
                                    npc.ai[2] = 100;
                                    npc.ai[3]++;
                                    if (npc.ai[3] > 4)
                                    {
                                        npc.ai[1] = CurrentAI;
                                        npc.ai[2] = 0;
                                        npc.ai[3] = 0;
                                        foreach(Projectile proj in Main.projectile)
                                        {
                                            if(proj.active && proj.type == ModContent.ProjectileType<Ritual>())
                                            {
                                                if ((int)proj.ai[0] == npc.whoAmI)
                                                {
                                                    proj.ai[1] = 1;
                                                }
                                            }
                                        }
                                        npc.dontTakeDamage = false;
                                    }
                                }
                            }
                        }
                        break;
                    case 4:           //水晶碎片
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] < 100)
                            {
                                Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, true);
                            }
                            if (npc.ai[2] > 100)
                            {
                                if (npc.ai[2] <= 310)
                                {
                                    npc.velocity *= 0.9f;
                                    if (npc.ai[2] % 40 == 30)
                                    {
                                        Vector2 Pos = player.Center + player.velocity * 15 + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * 250;
                                        TP(Pos);
                                    }
                                    if (npc.ai[2] % 40 == 10 && npc.ai[2] > 110)
                                    {
                                        Main.PlaySound(SoundID.Item101, npc.Center);
                                        bool flag = (int)(npc.ai[2] / 40) % 2 == 1;
                                        float r = flag ? 0 : MathHelper.Pi / 6;
                                        for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
                                        {
                                            Projectile.NewProjectile(npc.Center, (i + r).ToRotationVector2() * 32, ModContent.ProjectileType<CrystalThornBase>(), npc.damage / 4, 0, default);
                                        }
                                    }

                                }
                                else
                                {
                                    Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, true);
                                    if (npc.ai[2] > 340)
                                    {
                                        npc.ai[1] = Main.rand.NextBool() ? 1 : 6;
                                        npc.ai[2] = 0;
                                    }
                                }
                            }
                        }
                        break;
                    case 5:                  //仪式2
                        {
                            npc.velocity *= 0.8f;
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                npc.dontTakeDamage = true;
                                TP(player.Center + new Vector2(0, -300));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Ritual>(), 1, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] == 30)
                            {
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active && proj.type == ModContent.ProjectileType<Ritual>())
                                    {
                                        if ((int)proj.ai[0] == npc.whoAmI)
                                        {
                                            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 3)
                                            {
                                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<RazorbladeTyphoonHostile>(), (int)(npc.damage * 0.22), 0, default, proj.whoAmI, i);
                                            }
                                        }
                                    }
                                }

                            }

                            if (npc.ai[2] > 650)
                            {
                                npc.ai[1] = CurrentAI;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active)
                                    {
                                        if (proj.type == ModContent.ProjectileType<Ritual>())
                                        {
                                            if ((int)proj.ai[0] == npc.whoAmI)
                                            {
                                                proj.ai[1] = 1;
                                            }
                                        }

                                        if (proj.type == ModContent.ProjectileType<RazorbladeTyphoonProj>())
                                        {
                                            proj.Kill();
                                        }
                                    }
                                }
                                npc.dontTakeDamage = false;
                            }
                        }
                        break;
                    case 6:                 //裂天剑
                        {
                            if (npc.ai[2] > 100 && npc.ai[2] % 120 == 0)
                            {
                                npc.localAI[0] = Main.rand.NextFloat() * 400 - 200;
                                npc.localAI[1] = Main.rand.NextFloat() * 200 - 100;
                            }
                            npc.ai[2]++;
                            Movement(player.Center - new Vector2((300 + npc.localAI[1]) * npc.direction, npc.localAI[0]), 0.4f, true);
                            if (npc.ai[2] > 100)
                            {
                                if (npc.ai[2] <= 360)
                                {
                                    if (npc.ai[2] % 60 == 0)
                                    {
                                        int k = (int)(npc.ai[2] / 60) - 2;
                                        if (k == 0 || k == 2)
                                        {
                                            k = 1;
                                        }
                                        else if (k == 1 || k == 3)
                                        {
                                            k = 2;
                                        }
                                        else if (k == 4)
                                        {
                                            k = 3;
                                        }
                                        Vector2 Facing = Vector2.Normalize(player.Center - npc.Center);
                                        int protmp = Projectile.NewProjectile(npc.Center - Facing * (60 + Main.rand.Next(120)), Facing * 25, ProjectileID.SkyFracture, npc.damage / 5, 0);
                                        Main.projectile[protmp].friendly = false;
                                        Main.projectile[protmp].hostile = true;
                                        Main.projectile[protmp].tileCollide = false;
                                        Main.projectile[protmp].localAI[0] = k;
                                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                                        SpawnSFDust();
                                    }
                                    
                                }

                                if (npc.ai[2] > 530)
                                {
                                    npc.ai[1] = Main.rand.NextBool() ? 2 : 7;
                                    npc.ai[2] = 0;
                                    npc.ai[3] = 0;
                                    npc.localAI[0] = 0;
                                    npc.localAI[1] = 0;
                                }
                            }
                        }
                        break;
                    case 7:              //三法杖冲刺
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] < 100)
                            {
                                Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 700, 0.4f, true);
                            }
                            if (npc.ai[2] == 60 || npc.ai[2] == 90 || npc.ai[2] == 120) 
                            {
                                for(int i = 0; i < 20; i++)
                                {
                                    Dust dust = Dust.NewDustDirect(npc.Center, 1, 1, MyDustId.PurpleHighFx, default, default, default, default, 2);
                                    dust.velocity = (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * 20;
                                    dust.noGravity = true;
                                }
                            }
                            if (npc.ai[2] > 100 && npc.ai[2] <= 300)
                            {
                                int t = (int)(npc.ai[2] / 60) - 1;
                                
                                if (npc.ai[2] % 60 == 0)
                                {
                                    Vector2 RamVel = Vector2.Normalize(player.Center + player.velocity * (25 - t * 5) - npc.Center + new Vector2(0.01f, 0));
                                    
                                    npc.velocity = RamVel * 45;
                                    if (t == 1)
                                    {
                                        Main.PlaySound(SoundID.Item72, npc.Center);

                                        int protmp = Projectile.NewProjectile(npc.Center + (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 15, 16 * npc.velocity.ToRotation().ToRotationVector2(), ProjectileID.ShadowBeamHostile, npc.damage / 4, 0f);
                                        Main.projectile[protmp].timeLeft = 300;
                                        Main.projectile[protmp].tileCollide = false;

                                        protmp = Projectile.NewProjectile(npc.Center + (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 15, 16 * npc.velocity.ToRotation().ToRotationVector2(), ProjectileID.ShadowBeamHostile, npc.damage / 4, 0f);
                                        Main.projectile[protmp].timeLeft = 300;
                                        Main.projectile[protmp].tileCollide = false;
                                    }
                                }
                                if (npc.ai[2] % 60 <= 40)
                                {
                                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                                    if (npc.ai[2] % 60 == 6)
                                    {
                                        if (t == 1)
                                        {

                                            int protmp = Projectile.NewProjectile(npc.Center + (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 15, 16 * npc.velocity.ToRotation().ToRotationVector2(), ProjectileID.ShadowBeamHostile, npc.damage / 4, 0f);
                                            Main.projectile[protmp].timeLeft = 500;
                                            Main.projectile[protmp].tileCollide = false;

                                            protmp = Projectile.NewProjectile(npc.Center + (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 15, 16 * npc.velocity.ToRotation().ToRotationVector2(), ProjectileID.ShadowBeamHostile, npc.damage / 4, 0f);
                                            Main.projectile[protmp].timeLeft = 500;
                                            Main.projectile[protmp].tileCollide = false;
                                        }
                                    }
                                    if (t == 2)
                                    {
                                        if (npc.ai[2] % 6 == 2)
                                        {
                                            Vector2 blastPos = npc.Center + (200 + Main.rand.Next(200)) * (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2();
                                            int protmp = Projectile.NewProjectile(npc.Center, 16 * (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2(), ProjectileID.InfernoHostileBolt, npc.damage / 4, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                                            Main.projectile[protmp].timeLeft = 300;
                                            Main.projectile[protmp].tileCollide = false;

                                            blastPos = npc.Center + (200 + Main.rand.Next(200)) * (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2();
                                            protmp = Projectile.NewProjectile(npc.Center, 16 * (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2(), ProjectileID.InfernoHostileBolt, npc.damage / 4, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                                            Main.projectile[protmp].timeLeft = 300;
                                            Main.projectile[protmp].tileCollide = false;
                                        }
                                    }
                                    if (t == 3)
                                    {
                                        if (npc.ai[2] % 3 == 1)
                                        {
                                            Main.PlaySound(SoundID.Item43, npc.Center);
                                            int protmp = Projectile.NewProjectile(npc.Center, 6 * (npc.velocity.ToRotation() + MathHelper.Pi / 4).ToRotationVector2(),ModContent.ProjectileType<LostSoulHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                                            Main.projectile[protmp].timeLeft = 300;

                                            protmp = Projectile.NewProjectile(npc.Center, 6 * (npc.velocity.ToRotation() - MathHelper.Pi / 4).ToRotationVector2(), ModContent.ProjectileType<LostSoulHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                                            Main.projectile[protmp].timeLeft = 300;
                                        }
                                    }
                                }
                                if (npc.ai[2] % 60 > 40)
                                {
                                    if (npc.ai[2] % 60 < 50)
                                    {
                                        npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                                    }
                                    npc.velocity *= 0.96f;
                                    if (npc.ai[2] % 60 > 58)
                                    {
                                        npc.velocity = Vector2.Zero;
                                    }
                                    else
                                    {
                                        if (t == 2)
                                        {
                                            if (npc.ai[2] % 10 == 2)
                                            {
                                                Vector2 blastPos = npc.Center + 300 * (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2();
                                                int protmp = Projectile.NewProjectile(npc.Center, 16 * (npc.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2(), ProjectileID.InfernoHostileBolt, npc.damage / 4, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                                                Main.projectile[protmp].timeLeft = 300;
                                                Main.projectile[protmp].tileCollide = false;

                                                blastPos = npc.Center + 300 * (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2();
                                                protmp = Projectile.NewProjectile(npc.Center, 16 * (npc.velocity.ToRotation() - MathHelper.Pi / 2).ToRotationVector2(), ProjectileID.InfernoHostileBolt, npc.damage / 4, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                                                Main.projectile[protmp].timeLeft = 300;
                                                Main.projectile[protmp].tileCollide = false;
                                            }
                                        }
                                        if (t == 3)
                                        {
                                            if (npc.ai[2] % 4 == 2)
                                            {
                                                Main.PlaySound(SoundID.Item73, npc.Center);
                                                int protmp = Projectile.NewProjectile(npc.Center, 6 * (npc.velocity.ToRotation() + MathHelper.Pi / 4).ToRotationVector2(), ModContent.ProjectileType<LostSoulHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                                                Main.projectile[protmp].timeLeft = 300;

                                                protmp = Projectile.NewProjectile(npc.Center, 6 * (npc.velocity.ToRotation() - MathHelper.Pi / 4).ToRotationVector2(), ModContent.ProjectileType<LostSoulHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                                                Main.projectile[protmp].timeLeft = 300;
                                            }
                                        }
                                    }

                                }
                            }
                            if (npc.ai[2] > 300)
                            {
                                Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, true);
                                if (npc.ai[2] > 360)
                                {
                                    npc.ai[1] = Main.rand.NextBool() ? 0 : 4;
                                    npc.ai[2] = 0;
                                }
                            }

                        }
                        break;
                    case 8:              //仪式3
                        {
                            npc.velocity *= 0.8f;
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                npc.dontTakeDamage = true;
                                TP(player.Center + new Vector2(0, -300));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Ritual>(), 1, 0, default, npc.whoAmI);
                            }

                            if (npc.ai[2] > 160)
                            {
                                if (npc.ai[2] % 60 == 20)
                                {
                                    Main.PlaySound(SoundID.Item20, npc.Center);
                                    int count = 4 + Main.rand.Next(4);
                                    for (int i = 0; i < count; i++)
                                    {
                                        Vector2 RandomVelocity = (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * (15 + Main.rand.Next(50));
                                        Projectile.NewProjectile(npc.Center, RandomVelocity, ModContent.ProjectileType<MagnetSphereBallHostile>(), npc.damage / 4, 0);
                                    }
                                }
                            }

                            if (npc.ai[2] > 800)
                            {
                                npc.ai[1] = CurrentAI;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active)
                                    {
                                        if (proj.type == ModContent.ProjectileType<Ritual>())
                                        {
                                            if ((int)proj.ai[0] == npc.whoAmI)
                                            {
                                                proj.ai[1] = 1;
                                            }
                                        }

                                        if (proj.type == ModContent.ProjectileType<MagnetSphereBallHostile>())
                                        {
                                            proj.Kill();
                                        }
                                    }
                                }
                                npc.dontTakeDamage = false;
                            }
                        }
                        break;
                    case 9:             //最终仪式
                        {
                            npc.velocity *= 0.8f;
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                npc.dontTakeDamage = true;
                                TP(player.Center + new Vector2(0, -300));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Ritual>(), 1, 0, default, npc.whoAmI);
                            }

                            if (npc.ai[2] == 70)
                            {
                                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LastPrismHostile>(), npc.damage / 4, 0, default, npc.whoAmI, i);
                                }
                            }

                            if (npc.ai[2] > 170 && npc.ai[2] < 840)  
                            {
                                //player.wingTime = player.wingTimeMax;
                                if (npc.ai[2] % 60 == 20)
                                {
                                    int count = 1;
                                    if (npc.ai[2] > 400 && npc.ai[2] < 620)
                                    {
                                        count = 2;
                                    }
                                    if (npc.ai[2] > 620)
                                    {
                                        count = 3;
                                    }
                                    float RandomVRot1 = Main.rand.NextFloat() * MathHelper.TwoPi;
                                    float RandomR = 20 + Main.rand.Next(480);
                                    float RandomK = Main.rand.Next(2) * 2 - 1;
                                    for (int i = 0; i < count; i++)
                                    {
                                        Vector2 RandomVec = (RandomVRot1 + MathHelper.TwoPi / count * i).ToRotationVector2() * RandomR;
                                        float RandomRot2 = RandomVec.ToRotation() + MathHelper.Pi / 2 * RandomK;
                                        Vector2 RandomPos = npc.Center + RandomVec + (RandomRot2).ToRotationVector2() * (float)Math.Sqrt(1000000 - (float)Math.Pow(RandomVec.Length(), 2));
                                        Projectile.NewProjectile(RandomPos, (RandomRot2 + MathHelper.Pi).ToRotationVector2() * 0.01f, ModContent.ProjectileType<LastPrismHostile2>(), npc.damage / 4, 0, default, npc.whoAmI);
                                    }
                                }
                            }

                            if (npc.ai[2] > 960)
                            {
                                npc.dontTakeDamage = false;
                                npc.life = 0;
                                npc.HitEffect();
                                npc.checkDead();
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
            Texture2D Tex1 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/NebulaMageBoss");
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/NebulaWings");
            Texture2D Tex3 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/NebulaWingsGlow");

            for (int i = 1; i < 4; i++)
            {
                Color color27 = Color.Black * npc.Opacity;
                color27 *= (float)(4 - i) / 4;
                Vector2 value4 = npc.position - npc.velocity * i;
                Main.spriteBatch.Draw(Tex2, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 8, 92), WingFrame, color27, 0, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                Main.spriteBatch.Draw(Tex3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 8, 92), WingFrame, color27, 0, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                Main.spriteBatch.Draw(Tex1, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, color27, 0, Tex1.Size() * 0.5f, npc.scale, SP, 0f);
            }
            spriteBatch.Draw(Tex2, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 8, 92) + new Vector2(0, npc.gfxOffY), WingFrame, Color.Purple, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0);
            spriteBatch.Draw(Tex3, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 8, 92) + new Vector2(0, npc.gfxOffY), WingFrame, Color.Purple, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0);
            spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, Color.White, 0, Tex1.Size() * 0.5f, 1.0f, SP, 0);

            if (npc.ai[0] == 1)
            {
                if (npc.ai[1] == 1 && npc.ai[2] < 300)
                {
                    npc.ai[3] = (npc.ai[3] + 1) % 15;
                    Rectangle TexFrame = new Rectangle(0, 28 * (int)(npc.ai[3] / 5), 18, 28);
                    Texture2D tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/BoosterHeld");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 15, 0) - Main.screenPosition, TexFrame, Main.DiscoColor, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                    tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/BoosterHeld2");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 15, 0) - Main.screenPosition, TexFrame, Color.White, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                }

                if ((npc.ai[1] == 1 && npc.ai[2] > 400) || (npc.ai[1] == 2 && npc.ai[2] < 100 * 3 - 20))
                {
                    npc.ai[3] = (npc.ai[3] + 1) % 45;
                    Rectangle TexFrame = new Rectangle(0, 24 * (int)(npc.ai[3] / 5), 24, 24);
                    Texture2D tex = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/NAHold");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 25, 0) - Main.screenPosition, TexFrame, Color.White, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                }
            }
            if (npc.ai[0] == 3)
            {
                if (npc.ai[1] == 1 && npc.ai[2] < 300)
                {
                    npc.ai[3] = (npc.ai[3] + 1) % 15;
                    Rectangle TexFrame = new Rectangle(0, 28 * (int)(npc.ai[3] / 5), 18, 28);
                    Texture2D tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/BoosterHeld");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 15, 0) - Main.screenPosition, TexFrame, Main.DiscoColor, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                    tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/BoosterHeld2");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 15, 0) - Main.screenPosition, TexFrame, Color.White, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                }
                if (npc.ai[1] == 2)
                {
                    npc.ai[3] = (npc.ai[3] + 1) % 45;
                    Rectangle TexFrame = new Rectangle(0, 24 * (int)(npc.ai[3] / 5), 24, 24);
                    Texture2D tex = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/NAHold");
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 25, 0) - Main.screenPosition, TexFrame, Color.White, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
                }


                if (npc.ai[1] == 4 && npc.ai[2] > 50)
                {
                    Texture2D tex = Main.itemTexture[ItemID.CrystalVileShard];

                    Vector2 Facing = Main.player[npc.target].Center - npc.Center;
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction, 0) - Main.screenPosition, null, Color.White, Facing.ToRotation() + MathHelper.Pi / 4, new Vector2(0, tex.Height), 1, SpriteEffects.None, 0);

                }


                if (npc.ai[1] == 6 && npc.ai[2] > 50)
                {
                    Texture2D tex = Main.itemTexture[ItemID.SkyFracture];

                    Vector2 Facing = Main.player[npc.target].Center - npc.Center;
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction, 0) - Main.screenPosition, null, Color.White, Facing.ToRotation() + MathHelper.Pi / 4, new Vector2(0, tex.Height), 1, SpriteEffects.None, 0);

                }

                if (npc.ai[1] == 7)
                {

                    if (npc.ai[2] > 50 && npc.ai[2] < 150) 
                    {
                        Texture2D tex = Main.itemTexture[ItemID.ShadowbeamStaff];

                        Vector2 Facing = Main.player[npc.target].Center - npc.Center;
                        if (npc.ai[2] > 120)
                        {
                            Facing = npc.velocity;
                        }
                        spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction, 0) - Main.screenPosition, null, Color.White, Facing.ToRotation() + MathHelper.Pi / 4, new Vector2(0, tex.Height), 1, SpriteEffects.None, 0);
                    }
                    if (npc.ai[2] > 170 && npc.ai[2] < 225) 
                    {
                        Texture2D tex = Main.itemTexture[ItemID.InfernoFork];

                        Vector2 Facing = npc.velocity;
                        spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction, 0) - Main.screenPosition, null, Color.White, Facing.ToRotation() + MathHelper.Pi / 4, new Vector2(0, tex.Height), 1, SpriteEffects.None, 0);
                    }
                    if (npc.ai[2] > 235 && npc.ai[2] < 300) 
                    {
                        Texture2D tex = Main.itemTexture[ItemID.SpectreStaff];

                        Vector2 Facing = npc.velocity;
                        spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction, 0) - Main.screenPosition, null, Color.White, Facing.ToRotation() + MathHelper.Pi / 4, new Vector2(0, tex.Height), 1, SpriteEffects.None, 0);
                    }
                }

                if (npc.ai[1] == 8 && npc.ai[2] > 100)
                {
                    Texture2D tex = Main.itemTexture[ItemID.MagnetSphere];
                    SpriteEffects SP1 = (npc.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    spriteBatch.Draw(tex, npc.Center + new Vector2(npc.direction * 25, 0) - Main.screenPosition, null, Color.White, 0, tex.Size() * 0.5f, 1, SP1, 0);
                }
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
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 3 / 2;
        }

        public override void NPCLoot()
        {
            if (!MABWorld.DownedNebulaPlayer)
            {
                NPC.ShieldStrengthTowerNebula = 1;
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.TowerDamageBolt, 0, 0f, Main.myPlayer, NPC.FindFirstNPC(NPCID.LunarTowerNebula), 0f);
            }
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = (npc.ai[0] > 2) ? "星云守护者" : "星云术士" + "被击败了，凶手是" + Main.LocalPlayer.name + "。";
            MABWorld.DownedNebulaPlayer = true;
            if (npc.ai[0] == 1)
            {
                int LootNum = Main.rand.Next(5) + 10;
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentNebula);
                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaArcanum);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaBlaze);
                        break;
                    default:
                        break;
                }
            }
            if (npc.ai[0] == 3)             //二阶段
            {
                MABWorld.DownedNebulaPlayerEX = true;
                int LootNum = 114 + Main.rand.Next(514);
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentNebula);
                }

                List<int> list1 = new List<int>();
                list1.Add(ItemID.NebulaBreastplate);
                list1.Add(ItemID.NebulaLeggings);
                list1.Add(ItemID.NebulaHelmet);
                list1.Add(ItemID.NebulaArcanum);
                list1.Add(ItemID.NebulaBlaze);
                list1.Add(ItemID.LunarFlareBook);
                list1.Add(ItemID.LastPrism);
                
                for(int i = 0; i < 4; i++)
                {
                    int type = list1[Main.rand.Next(list1.Count)];
                    list1.Remove(type);
                    Item.NewItem(npc.Hitbox, type);
                }
            }

        }

        public override bool CheckDead()
        {

            if (npc.ai[0] == 1 && MABWorld.DownedNebulaPlayer && NPC.downedMoonlord) 
            { 
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.ai[0] = 2;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.localAI[3] = 0;
                for(int i = 0; i < 15; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.Heart);
                }
                return false;
            }

            if (npc.ai[0] == 3 && StateRage <= 3)
            {
                npc.life = 1;
                npc.dontTakeDamage = true;
                AllClear();
                StateRage = 4;
                npc.ai[1] = 9;              //棱镜
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
        public void AllClear()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<NebulaBlazeHostile>() || proj.type == ModContent.ProjectileType<NebulaBlazeHostileEX>() ||
                        proj.type == ModContent.ProjectileType<ManaBoosterHostile>() || proj.type == ModContent.ProjectileType<LifeBoosterHostile>() ||
                        proj.type == ModContent.ProjectileType<DamageBoosterHostile>() || proj.type == ModContent.ProjectileType<NebulaArcanumHostile>() ||
                        proj.type == ModContent.ProjectileType<NASecondaryProj>() || proj.type == ModContent.ProjectileType<LunarFlareHostile>() ||
                        proj.type == ModContent.ProjectileType<LunarFlareMark>() || proj.type == ModContent.ProjectileType<RazorbladeTyphoonProj>() ||
                        proj.type == ModContent.ProjectileType<RazorbladeTyphoonHostile>() || proj.type == ModContent.ProjectileType<CrystalThornBase>() ||
                        proj.type == ModContent.ProjectileType<CrystalThornTip>() || proj.type == ModContent.ProjectileType<LostSoulHostile>() ||
                        proj.type == ModContent.ProjectileType<MagnetSphereBallHostile>() || proj.type == ModContent.ProjectileType<LunarFlareBookHostile>() ||
                        proj.type == ModContent.ProjectileType<LastPrismHostile>() || proj.type == ModContent.ProjectileType<PrismLaserHostile>() || 
                        proj.type == ModContent.ProjectileType<LastPrismHostile2>())     
                    {
                        proj.Kill();
                    }
                    if (proj.GetGlobalProjectile<PlayerBossProj>().SpecialProj)
                    {
                        if (proj.type == ProjectileID.SkyFracture || proj.type == ProjectileID.ShadowBeamHostile || 
                            proj.type == ProjectileID.InfernoHostileBlast || proj.type == ProjectileID.InfernoHostileBolt)
                        {
                            proj.Kill();
                        }
                    }
                }
            }
        }
        public override bool CheckActive()
        {
            if (npc.ai[0] == 3)
            {
                if (npc.ai[1] == 3 || npc.ai[1] == 5 || npc.ai[1] == 8 || npc.ai[1] == 9)
                {
                    return false;
                }
            }
            return true;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            name = npc.GivenName;
        }

        private float PointMulti(Vector2 v1,Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        private void SpawnSFDust()
        {
            float Facing = (Main.player[npc.target].Center - npc.Center).ToRotation();
            for (int i = 0; i <= 40; i += 10)
            {
                for (int num103 = 0; num103 < 12; num103++)
                {
                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy(num103 * (MathHelper.TwoPi / 12), default) * new Vector2(1f, 4f);
                    vector12 = vector12.RotatedBy(Facing, default);
                    int num104 = Dust.NewDust(npc.Center + Facing.ToRotationVector2() * (35 + i * 2), 0, 0, 180, 0f, 0f, 0, default, 1f) ;
                    Main.dust[num104].scale = 1.5f;
                    Main.dust[num104].noGravity = true;
                    Main.dust[num104].position = npc.Center + Facing.ToRotationVector2() * (35 + i * 2) + vector12;
                    Main.dust[num104].velocity = Facing.ToRotationVector2() * 0f + vector12.SafeNormalize(Vector2.UnitY) * 1;
                    Main.dust[num104].velocity *= (80 - i) / 40;
                }
            }
        }
    }
}