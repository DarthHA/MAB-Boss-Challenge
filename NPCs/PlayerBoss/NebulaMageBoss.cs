using MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj;
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
    public class NebulaMageBoss : ModNPC
    {
        private int WingsFrame = 0;
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
            npc.defense = 40;
            npc.lifeMax = 50000;
            npc.HitSound = SoundID.NPCHit9;
            npc.DeathSound = SoundID.NPCDeath11;
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
                                    Main.PlaySound(SoundID.Item32, npc.Center);
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
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/The Last Guardian");
                    npc.lifeMax = 300000;
                    npc.damage = 200;
                    npc.defense = 60;
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

            if (npc.ai[0] == 3)             //二阶段
            {

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
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        //dust11.customData = npc;
                                    }
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
                                if (npc.ai[2] % 17 == 5)
                                {
                                    Main.PlaySound(SoundID.Item32, npc.Center);
                                    Vector2 ShootVel = player.Center - npc.Center;
                                    float ShootVelR = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                                    Projectile.NewProjectile(npc.Center, (ShootVelR + Main.rand.NextFloat() * MathHelper.Pi / 6 - MathHelper.Pi / 12).ToRotationVector2() * 10, ModContent.ProjectileType<NebulaBlazeHostile>(), npc.damage / 3, 0, npc.target, 1);


                                    //Projectile.NewProjectile(npc.Center, ShootVelR.ToRotationVector2()*5, ModContent.ProjectileType<NebulaArcanumHostile>(), 10, 10, player.whoAmI);
                                }
                            }
                            if (npc.ai[2] > 400)
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                            }
                        }
                        break;
                    case 1:
                        break;
                    case 2:
                        break;

                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:

                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:

                        break;
                    case 9:
                        break;
                    default:
                        break;
                }
            }

            if (npc.ai[0] == 4)
            {
                npc.life = 1;
                npc.dontTakeDamage = true;
                npc.ai[0] = 1;
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
                    Texture2D tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/ManaBoosterHostile");
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

            return false;
        }

        /*
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (npc.alpha == 255) return false;
            return base.DrawHealthBar(hbPosition, ref scale, ref position);
        }
        */
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
            if (npc.ai[0] == 3)
            {
                MABWorld.DownedNebulaPlayerEX = true;
                int LootNum = Main.rand.Next(10) + 30;
                for (int i = 0; i < LootNum; i++)
                {
                    Item.NewItem(npc.Hitbox, ItemID.FragmentNebula);
                }
                switch (Main.rand.Next(4))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaArcanum);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaBlaze);
                        break;
                    case 2:
                        Item.NewItem(npc.Hitbox, ItemID.LastPrism);
                        break;
                    case 3:
                        Item.NewItem(npc.Hitbox, ItemID.LunarFlareBook);
                        break;
                    default:
                        break;
                }

                switch (Main.rand.Next(3))
                {
                    case 0:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaBreastplate);
                        break;
                    case 1:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaHelmet);
                        break;
                    case 2:
                        Item.NewItem(npc.Hitbox, ItemID.NebulaLeggings);
                        break;
                }

            }

        }

        public override bool CheckDead()
        {
            /*
            if (npc.ai[0] == 1 && MABWorld.DownedDestroyerEX && MABWorld.DownedNebulaPlayer && NPC.downedMoonlord)
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
                return false;
            }
                        */
            return true;

        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            name = npc.GivenName;
        }

    }
}