using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer;
using MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer;
using MABBossChallenge.Utils;
using Terraria.Localization;

namespace MABBossChallenge.NPCs.MiniPlayerBoss
{
    [AutoloadBossHead]
    public class CrimsonPlayerBoss : ModNPC
    {
        private int MoveAnimiantionMode = 0;
        private Vector2 SpawnPos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Guardian");
            DisplayName.AddTranslation(GameCulture.Chinese, "猩红守护者");
            Main.npcFrameCount[npc.type] = 20;
            TranslationUtils.AddTranslation("CrimsonGuardian", "Crimson Guardian", "猩红守护者");
            TranslationUtils.AddTranslation("CrimsonGuardianDescription", "The demonized knight soaked in crimson power", "被血腥之力浸染的魔剑士");
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 36;
            npc.height = 52;
            npc.damage = 80;
            npc.defense = 15;
            npc.lifeMax = 12000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.noGravity = false;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/The First Guardian");
            npc.dontTakeDamage = true;
            npc.value = 20000;

            if (!Main.expertMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 0.75f);
                npc.damage = (int)(npc.damage * 0.75f);
                npc.life = npc.lifeMax;
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.damage /= 2;
        }
        public override void AI()
        {
            npc.TargetClosest();
            bool Raged = npc.localAI[2] == 1 && NPC.AnyNPCs(ModContent.NPCType<ShadowPlayerBoss>());
            if (SpawnPos == Vector2.Zero && npc.collideY)
            {
                SpawnPos = npc.Center;
            }

            Player player = Main.player[npc.target];

            if (player.dead || (player.Center - npc.Center).Length() > 4000 || !Utils.NPCUtils.InWall(player.Center,ModContent.WallType<Walls.ArenaWall>()))
            {
                npc.ai[0] = 2;
            }
            if (npc.ai[0] != 2)
            {
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.Center = new Vector2(npc.Center.X, SpawnPos.Y);
                    npc.position.X = Terraria.Utils.Clamp(npc.Center.X, SpawnPos.X - 780, SpawnPos.X + 780) - npc.width / 2;
                }

                if (!DetectBorder())
                {
                    npc.velocity.X = 0;
                    npc.position.X = Terraria.Utils.Clamp(npc.Center.X, SpawnPos.X - 780, SpawnPos.X + 780) - npc.width / 2;
                }
            }
            npc.spriteDirection = npc.direction = Math.Sign(player.Center.X - npc.Center.X);

            if (npc.velocity.Y != 0) MoveAnimiantionMode = 2;
            if (npc.velocity.Y == 0 && Math.Abs(npc.velocity.X) > 1f) MoveAnimiantionMode = 3;
            if (npc.velocity.Y == 0 && Math.Abs(npc.velocity.X) <= 1f) MoveAnimiantionMode = 0;
            Animination();

            if (Main.rand.Next(4) == 1)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, MyDustId.RedBlood, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                
            }

            if (npc.ai[0] == 0)               //开局
            {
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    if (!Raged) 
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("CrimsonGuardian"), TranslationUtils.GetTranslation("CrimsonGuardianDescription"));
                    }
                    else
                    {
                        //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Right on Track");
                        npc.Center += new Vector2(40, 0);
                    }
                }

                if (npc.ai[2] >= 330)
                {
                    npc.dontTakeDamage = false;
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<BloodLustClusterHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                    npc.ai[0] = 1;             
                    npc.ai[1] = 6;                 ///6
                    npc.ai[2] = 0;
                    //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/The Grimm Troupe");
                }
            }

            if (npc.ai[0] == 1)
            {
               
                switch (npc.ai[1])
                {

                    case 0:                 //后跃
                        {
                            npc.direction = Math.Sign(SpawnPos.X - npc.Center.X + 0.01f);
                            if (npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                                npc.localAI[0] = Terraria.Utils.Clamp(SpawnPos.X + Math.Sign(npc.Center.X - SpawnPos.X - 0.01f) * 700, SpawnPos.X - 700, SpawnPos.X + 650);

                                npc.velocity = new Vector2(-16 * npc.direction, -7);
                            }

                            if (npc.velocity.Y != 0)
                            {
                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 16)
                                {
                                    npc.velocity.X = 0;
                                    npc.velocity.Y += 1f;
                                }
                            }
                            if (npc.velocity.Y == 0)
                            {

                                npc.velocity.X *= 0.8f;
                                npc.ai[2]++;
                                if (npc.ai[2] > 10)
                                {
                                    npc.ai[1]++;
                                    npc.ai[2] = 0;
                                }
                            }
                        }
                        break;
                    case 1:                     //大剑气
                        {
                            npc.ai[2]++;
                            npc.direction = Math.Sign(SpawnPos.X - npc.Center.X);

                            if (npc.ai[2] == 45)
                            {
                                Main.PlaySound(SoundID.Item71, npc.Center);
                                float FiringY = npc.Center.Y + npc.height / 2 - 190;
                                Projectile.NewProjectile(new Vector2(npc.Center.X + 50 * npc.direction, FiringY), new Vector2(1, 0) * npc.direction, ModContent.ProjectileType<BloodBeam>(), (int)(npc.damage * 0.3), 0, default);
                            }
                            if (npc.ai[2] > 120)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 4 : 13;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:                  //血云冲刺
                        {
                            if (npc.ai[2] == 0)
                            {
                                foreach(Projectile proj in Main.projectile)
                                {
                                    if(proj.active && proj.type == ModContent.ProjectileType<CNimbusHostile2>())
                                    {
                                        proj.Kill();
                                    }
                                }
                                npc.velocity.Y = -25f;
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] == 1)
                            {
                                if (npc.Center.Y <= SpawnPos.Y - 500)
                                {
                                    npc.ai[2]++;
                                }
                            }
                            if (npc.ai[2] > 1)
                            {

                                if (npc.ai[2] < 75)             //飞行
                                {
                                    npc.velocity.Y = 0;
                                    npc.ai[2]++;
                                }
                                if (npc.ai[2] == 3)
                                {
                                    npc.ai[2] = 39;
                                }
                                if (npc.ai[2] == 40)
                                {
                                    npc.localAI[0] = Math.Sign(player.Center.X - npc.Center.X + 0.01f);
                                }
                                if (npc.ai[2] > 40 && npc.ai[2] <= 70)
                                {
                                    npc.hide = true;
                                    npc.velocity.X = 25 * npc.localAI[0];
                                    if (npc.ai[2] == 41)
                                    {
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CrimsonRam>(), npc.damage / 4, 0, default, npc.whoAmI);
                                    }
                                    int freq = Raged ? 4 : 2;
                                    if (npc.ai[2] % freq == 1)
                                    {
                                        int protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CNimbusHostile2>(), npc.damage / 4, 0);
                                        Main.projectile[protmp].timeLeft = 360;
                                    }

                                    if (Collision.SolidCollision(npc.position + new Vector2(npc.velocity.X, 0), npc.width, npc.height))
                                    {
                                        npc.ai[2] = 71;
                                    }
                                }
                                if (npc.ai[2] > 70)           //下落
                                {
                                    npc.hide = false;
                                    npc.velocity.X *= 0.9f;
                                }
                                if (npc.ai[2] == 75)
                                {
                                    npc.velocity.Y += 1f;
                                    if (!Collision.SolidCollision(npc.Center + new Vector2(0, 15), npc.width, npc.height))
                                    {
                                        npc.position.Y += 15;
                                    }
                                    if (Collision.SolidCollision(npc.position + new Vector2(0, npc.velocity.Y), npc.width, npc.height))
                                    {
                                        Main.PlaySound(SoundID.Item14, npc.position);
                                        Projectile.NewProjectile(npc.Bottom, Vector2.Zero, ModContent.ProjectileType<HitExplosion>(), npc.damage / 4, 0);
                                        npc.velocity.Y = 0;
                                        npc.ai[2]++;
                                    }
                                }
                                if (npc.ai[2] > 75)
                                {
                                    npc.ai[2]++;
                                    //player.GetModPlayer<ShakeScreenPlayer>().shake = true;
                                    if (npc.ai[2] > 105)
                                    {
                                        npc.ai[1]++;
                                        npc.ai[2] = 0;
                                    }
                                }
                            }
                        }
                        break;
                    case 3:             //停顿
                        {
                            npc.ai[2]++;
                            int timer = Raged ? 100 : 60;
                            if (npc.ai[2] > timer)
                            {
                                npc.ai[1] = 6;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 4:                     //灵液跳劈
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                                npc.velocity = new Vector2(15 * npc.direction, -15);
                                npc.localAI[0] = Terraria.Utils.Clamp(player.Center.X, SpawnPos.X - 750, SpawnPos.X + 750);
                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 200)
                                {
                                    npc.localAI[0] = npc.Center.X + Math.Sign(npc.localAI[0] - npc.Center.X) * 200;
                                }
                                npc.localAI[0] = Terraria.Utils.Clamp(npc.localAI[0], SpawnPos.X - 700, SpawnPos.X + 700);
                                
                                npc.localAI[1] = player.Center.Y;
                            }
                            npc.direction = npc.spriteDirection = Math.Sign(npc.localAI[0] - npc.Center.X);
                            if (npc.velocity.Y != 0)
                            {

                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 16)
                                {
                                    npc.velocity.X = 0;
                                    npc.velocity.Y += 2f;
                                    if (!Collision.SolidCollision(npc.position + new Vector2(0, 1f), npc.width, npc.height))
                                    {
                                        npc.position.Y += 1f;
                                    }
                                }
                                else
                                {
                                    npc.velocity.X += 0.5f * (npc.localAI[0] - npc.Center.X);
                                    if (npc.velocity.X > 10) npc.velocity.X = 10;
                                    if (npc.velocity.X < -10) npc.velocity.X = -10;
                                }
                            }
                            if (npc.velocity.Y == 0)
                            {
                                if (npc.ai[2] == 1)
                                {
                                    Main.PlaySound(SoundID.Item14, npc.position);
                                }
                                npc.velocity.X *= 0.8f;
                                npc.ai[2]++;
                                if (npc.ai[2] < 30)
                                {
                                    player.GetModPlayer<ShakeScreenPlayer>().shake = true;
                                }
                                if (npc.ai[2] == 2)
                                {
                                    Projectile.NewProjectile(npc.Bottom, Vector2.Zero, ModContent.ProjectileType<HitExplosion>(), npc.damage / 4, 0);
                                    if (!Raged)
                                    {
                                        for (float i = -MathHelper.Pi / 6; i <= MathHelper.Pi / 6; i += MathHelper.Pi / 56)
                                        {
                                            int protmp = Projectile.NewProjectile(npc.Center, (i - MathHelper.Pi / 2).ToRotationVector2() * 10, ProjectileID.GoldenShowerHostile, npc.damage / 4, 0);
                                            Main.projectile[protmp].tileCollide = false;
                                        }
                                    }
                                    else
                                    {
                                        for (float i = -MathHelper.Pi / 6; i <= MathHelper.Pi / 6; i += MathHelper.Pi / 48)
                                        {
                                            int protmp = Projectile.NewProjectile(npc.Center, (i - MathHelper.Pi / 2).ToRotationVector2() * 10, ProjectileID.GoldenShowerHostile, npc.damage / 4, 0);
                                            Main.projectile[protmp].tileCollide = false;
                                        }
                                    }
                                }
                                if (npc.ai[2] > 80)
                                {
                                    npc.ai[1]++;
                                    npc.ai[2] = 0;
                                }
                            }
                        }
                        break;
                    case 5:                       //停顿
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 30)
                            {
                                npc.ai[1] = (Main.rand.NextBool()) ? 2 : 10;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 6:                 //电锯斩
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.velocity.Y = -30f;
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] == 1)
                            {
                                if (npc.Center.Y <= SpawnPos.Y - 400)
                                {
                                    npc.ai[2]++;
                                }
                            }
                            if (npc.ai[2] > 1)
                            {
                                if (npc.ai[2] < 60)
                                {
                                    npc.velocity.Y = 0;
                                    npc.ai[2]++;
                                }
                                if (npc.ai[2] == 40)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<BladeSwing>(), npc.damage / 4, 0, default ,npc.whoAmI);
                                }
                                if (npc.ai[2] == 60)
                                {

                                    Vector2 ShootVel = Vector2.Normalize(player.Center - npc.Center);
                                    if (npc.Distance(player.Center) > 250)
                                    {
                                        ShootVel = Vector2.Normalize(player.Center - npc.Center + player.velocity * 5);
                                    }
                                    npc.localAI[0] = ShootVel.X;
                                    npc.localAI[1] = ShootVel.Y;
                                    npc.ai[2]++;
                                }
                                if (npc.ai[2] == 61) 
                                {
                                    Vector2 RamVel = new Vector2(npc.localAI[0], npc.localAI[1]);
                                    npc.velocity = RamVel;
                                    npc.direction = Math.Sign(npc.velocity.X);
                                    if (Collision.SolidCollision(npc.position + npc.velocity * 39, npc.width, npc.height) ||
                                        Collision.SolidCollision(npc.position + npc.velocity * 20, npc.width, npc.height) ||
                                        Collision.SolidCollision(npc.position + npc.velocity * 10, npc.width, npc.height) ||
                                        Collision.SolidCollision(npc.position + npc.velocity * 5, npc.width, npc.height) )
                                        //!DetectBorder())   
                                    {
                                        if (npc.Center.Y >= SpawnPos.Y - 16) 
                                        {
                                            npc.ai[2]++;
                                            Main.PlaySound(SoundID.Item14, npc.position);
                                        }
                                        else
                                        {
                                            npc.velocity.Y += 25f;
                                        }
                                    }
                                    else
                                    {
                                        npc.position += RamVel * 36;
                                    }
                                }
                                if (npc.ai[2] >= 62) 
                                {
                                    npc.ai[2]++;
                                    npc.velocity.X *= 0.98f;

                                }
                                if (npc.ai[2] == 90)
                                {
                                    float RamVel = Math.Sign(SpawnPos.X - npc.Center.X + 0.01f);
                                    npc.velocity.X = RamVel * 25;
                                }
                                if (npc.ai[2] > 120)
                                {
                                    npc.velocity.X *= 0.9f;
                                }
                                if (npc.ai[2] > 140)
                                {
                                    npc.ai[1]++;
                                    npc.ai[2] = 0;
                                }
                            }

                        }
                        break;
                    case 7:                       //停顿
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 30)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 0 : 8;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 8:             //平A
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] % 20 == 0)
                            {
                                npc.velocity.X = npc.direction * 25;
                            }

                            if ((npc.Center.X > SpawnPos.X + 790 && npc.velocity.X > 0) || (npc.Center.X < SpawnPos.X - 790 && npc.velocity.X < 0))
                            {
                                npc.velocity = Vector2.Zero;
                            }

                            if (npc.ai[2] % 20 > 0)
                            {
                                npc.velocity.X *= 0.8f;
                            }
                            if (npc.ai[2] >= 119)
                            {
                                npc.ai[1]++;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 9:                       //停顿
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 30)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 4 : 13;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 10:             //后跃
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                                npc.localAI[0] = Terraria.Utils.Clamp(player.Center.X, SpawnPos.X - 700, SpawnPos.X + 700);
                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 200)
                                {
                                    npc.localAI[0] = npc.Center.X + Math.Sign(npc.localAI[0] - npc.Center.X) * 200;
                                }
                                npc.localAI[0] = Terraria.Utils.Clamp(npc.localAI[0], SpawnPos.X - 700, SpawnPos.X + 700);
                                npc.direction = Math.Sign(player.Center.X - npc.Center.X);
                                npc.velocity = new Vector2(-12 * npc.direction, -7);
                            }

                            if (npc.velocity.Y != 0)
                            {
                                npc.direction = Math.Sign(player.Center.X - npc.Center.X);
                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 16)
                                {
                                    npc.velocity.X = 0;
                                    npc.velocity.Y += 0.5f;
                                }
                            }
                            if (npc.velocity.Y == 0)
                            {
                                npc.direction = Math.Sign(player.Center.X - npc.Center.X);
                                npc.velocity.X *= 0.8f;
                                npc.ai[2]++;
                                if (npc.ai[2] > 10)
                                {
                                    npc.ai[1]++;
                                    npc.ai[2] = 0;
                                }
                            }
                        }
                        break;
                    case 11:                      //前冲雨云
                        {

                            npc.ai[2]++;
                            if (npc.ai[2] < 30)
                            {
                                npc.direction = Math.Sign(player.Center.X - npc.Center.X);
                            }
                            if (npc.ai[2] == 30)
                            {
                                npc.localAI[0] = Math.Sign(player.Center.X - npc.Center.X);
                                npc.localAI[1] = Math.Sign(player.Center.X - npc.Center.Y);
                                npc.velocity.X = npc.localAI[0] * 45;
                            }

                            if (npc.ai[2] < 50 && npc.ai[2] >= 30) 
                            {
                                npc.direction = npc.spriteDirection = (int)npc.localAI[0];

                                if ((npc.Center.X > SpawnPos.X + 780 && npc.velocity.X > 0) || (npc.Center.X < SpawnPos.X - 780 && npc.velocity.X < 0))
                                {
                                    npc.velocity = Vector2.Zero;
                                    npc.ai[2] = 50;
                                }

                            }
                            if (npc.ai[2] < 60 && npc.ai[2] > 30)            //弹幕阶段
                            {
                                if (Math.Abs(npc.velocity.X) > 1)
                                {
                                    int freq = Raged ? 4 : 2;
                                    if (npc.ai[2] % freq == 1)
                                    {
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CNimbusHostile>(), (int)(npc.damage * 0.3), 0, default, npc.Center.X, SpawnPos.Y - 500);
                                    }
                                }
                            }
                            if (npc.ai[2] >= 50)
                            {
                                if ((npc.Center.X > SpawnPos.X + 780 && npc.velocity.X > 0) || (npc.Center.X < SpawnPos.X - 790 && npc.velocity.X < 0))
                                {
                                    npc.velocity = Vector2.Zero;
                                }

                                npc.velocity *= 0.9f;

                                if (npc.ai[2] < 80)
                                {
                                    npc.direction = npc.spriteDirection = (int)npc.localAI[0];
                                }
                            }
                            if (npc.ai[2] > 120)
                            {
                                npc.ai[1]++;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 12:          //停顿
                        {
                            npc.ai[2]++;
                            int timer = Raged ? 120 : 80;
                            if (npc.ai[2] > timer)
                            {
                                npc.ai[1] = 6;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 13:                    //大刀跳劈
                        {
                            if (npc.ai[2] >= 0 && npc.ai[2] < 30) 
                            {
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] == 10)
                            {
                                npc.localAI[0] = npc.direction;
                            }
                            if (npc.ai[2] > 10)
                            {
                                npc.direction = Math.Sign(npc.localAI[0]);
                            }
                            if (npc.ai[2] == 30)
                            {
                                npc.ai[2]++;
                                npc.velocity = new Vector2(10 * npc.direction * npc.Distance(player.Center) / 1000, -25);
                            }
                            if (npc.ai[2] > 30)
                            {
                                if (npc.velocity.Y != 0)
                                {
                                    if (!DetectBorder())
                                    {
                                        npc.velocity.X = 0.001f;
                                        npc.velocity.Y += 2f;
                                        if (!Collision.SolidCollision(npc.position + new Vector2(0, 1f), npc.width, npc.height))
                                        {
                                            npc.position.Y += 1f;
                                        }
                                    }
                                    else
                                    {
                                        npc.velocity.Y += 0.5f;
                                    }
                                }
                                if (npc.velocity.Y == 0)
                                {
                                    if (npc.ai[2] == 31)
                                    {
                                        Main.PlaySound(SoundID.Item14, npc.position);
                                    }
                                    npc.velocity.X *= 0.8f;
                                    npc.ai[2]++;
                                    if (npc.ai[2] < 60)
                                    {
                                        player.GetModPlayer<ShakeScreenPlayer>().shake = true;
                                    }
                                    if (npc.ai[2] == 32)
                                    {
                                        Main.PlaySound(SoundID.Item14, npc.position);
                                        for (int i = 0; i < 10; i++)
                                        {
                                            Projectile.NewProjectile(npc.Bottom + new Vector2(70 * npc.direction, 0) * i, Vector2.Zero, ModContent.ProjectileType<HitExplosion>(), npc.damage / 4, 0);
                                        }
                                    }
                                    if (npc.ai[2] > 60)
                                    {
                                        npc.ai[1]++;
                                        npc.ai[2] = 0;
                                    }
                                }
                            }
                        }
                        break;
                    case 14:              //停顿
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 30)
                            {
                                npc.ai[1] = Main.rand.NextBool() ? 2 : 10;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (npc.ai[0] == 2)
            {
                npc.velocity += new Vector2(0, 1);
                npc.noGravity = false;
                npc.noTileCollide = true;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                return;
            }

        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private bool DetectBorder()
        {
            if (npc.velocity.X > 0)
            {
                return npc.Center.X < SpawnPos.X + 780;
            }
            else if (npc.velocity.X < 0)
            {
                return npc.Center.X > SpawnPos.X - 780;
            }
            else
            {
                return true;
            }
        }
        public void Animination()
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                if (MoveAnimiantionMode == 0)             //静息
                {
                    npc.localAI[3] = 0;
                    return;
                }
                if (MoveAnimiantionMode == 1)               //挥手
                {
                    if (npc.localAI[3] < 1 || npc.localAI[3] > 4) 
                    {
                        npc.localAI[3] = 1;
                    }
                    else
                    {
                        npc.localAI[3]++;
                    }
                    return;
                }
                if (MoveAnimiantionMode == 2)              //空中
                {
                    npc.localAI[3] = 5;
                    return;
                }
                if (MoveAnimiantionMode == 3)               //行走
                {
                    if (npc.localAI[3] < 6 || npc.localAI[3] > 18)
                    {
                        npc.localAI[3] = 6;
                    }
                    else
                    {
                        npc.localAI[3]++;
                    }
                    return;
                }
            }


        }

        
        private bool HasWeapons()
        {
            foreach(Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<BloodLustClusterHostile>() && proj.ai[0] == npc.whoAmI && !proj.hide)
                {
                    return true;
                }
            }
            return false;
        }
        

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (npc.hide) return false;
            return null;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.RedPotion;
        }

        public override void NPCLoot()
        {
            if (npc.localAI[2] == 1)
            {
                int npctmp = NPC.FindFirstNPC(ModContent.NPCType<ShadowPlayerBoss>());
                if (npctmp != -1)
                {
                    if (Main.npc[npctmp].life < Main.npc[npctmp].lifeMax / 3 * 2)
                    {
                        Main.npc[npctmp].life += Main.npc[npctmp].lifeMax / 3;
                    }
                    else
                    {
                        Main.npc[npctmp].life = Main.npc[npctmp].lifeMax;
                    }
                    Main.npc[npctmp].damage = (int)(Main.npc[npctmp].damage * 1.3f);
                    Main.npc[npctmp].HealEffect(Main.npc[npctmp].lifeMax / 3);
                }
            }
            MABWorld.DownedPreEvilFighter2 = true;
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = "猩红守护者 被击败了，凶手是" + Main.LocalPlayer.name + "。";
        }
        private void DP(SpriteBatch spritebatch,Vector2 Pos,Color a)
        {
            Rectangle Frame = new Rectangle(0, (int)(56 * npc.localAI[3]), 40, 56);
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            
            Texture2D WeaponTex = Main.itemTexture[ItemID.BloodButcherer];
            if (!HasWeapons())
            {
                if (npc.direction > 0)
                {
                    spritebatch.Draw(WeaponTex, Pos, null, a, MathHelper.Pi, WeaponTex.Size() / 2, 1, SP, 0);
                }
                else
                {
                    spritebatch.Draw(WeaponTex, Pos, null, a, MathHelper.Pi, WeaponTex.Size() / 2, 1, SP, 0);
                }
            }
            
            Texture2D Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/CrimsonPlayerBoss_Legs");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/CrimsonPlayerBoss_Body");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/CrimsonPlayerBoss_Face");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/CrimsonPlayerBoss_Head");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);

            

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            DP(spriteBatch, npc.Center - Main.screenPosition, drawColor);
            return false;
        }

       
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
    }
}