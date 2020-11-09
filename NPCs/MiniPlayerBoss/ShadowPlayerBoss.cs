using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer;
using MABBossChallenge.Walls;
using Terraria.Localization;
using MABBossChallenge.Utils;

namespace MABBossChallenge.NPCs.MiniPlayerBoss
{
    [AutoloadBossHead]
    public class ShadowPlayerBoss : ModNPC
    {
        private int MoveAnimiantionMode = 0;
        public int OriHeadSlot = -1;
        private Vector2 SpawnPos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Guardian");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影守护者");
            Main.npcFrameCount[npc.type] = 20;
            TranslationUtils.AddTranslation("ShadowGuardian", "Shadow Guardian", "暗影守护者");
            TranslationUtils.AddTranslation("ShadowGuardianDescription", "The demonized knight corroded by the power of corruption", "被腐化之力侵蚀的魔剑士");
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 36;
            npc.height = 52;
            npc.damage = 70;
            npc.defense = 0;
            npc.lifeMax = 10000;
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
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.damage /= 2;
        }
        public override void AI()
        {
            npc.TargetClosest();
            bool Raged = npc.localAI[2] == 1 && NPC.AnyNPCs(ModContent.NPCType<CrimsonPlayerBoss>());
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
                }

                if (npc.Center.X > SpawnPos.X + 795 || npc.Center.X < SpawnPos.X - 795) 
                {
                    npc.velocity.X = 0;
                }
            }
            npc.spriteDirection = npc.direction = Math.Sign(player.Center.X - npc.Center.X);

            if (npc.velocity.Y != 0) MoveAnimiantionMode = 2;
            if (npc.velocity.Y == 0 && Math.Abs(npc.velocity.X) > 1f) MoveAnimiantionMode = 3;
            if (npc.velocity.Y == 0 && Math.Abs(npc.velocity.X) <= 1f) MoveAnimiantionMode = 0;
            Animination();

            if (npc.ai[0] == 0)               //开局
            {
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    if (!Raged)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("ShadowGuardian"),TranslationUtils.GetTranslation("ShadowGuardianDescription"));
                    }
                    else
                    {
                        npc.localAI[2] = 1;
                        //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Right on Track");
                        npc.Center -= new Vector2(40, 0);
                    }
                }

                if (npc.ai[2] >= 330)
                {
                    npc.dontTakeDamage = false;
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightsBaneHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                    npc.ai[2] = 0;
                    npc.ai[0] = 1;
                    npc.ai[1] = 0;
                }
            }

            if (npc.ai[0] == 1)
            {
                if (!HasWeapons())
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightsBaneHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                }
                switch (npc.ai[1])
                {
                    case 0:                  //闪现攻击
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.hide = true;
                                npc.dontTakeDamage = true;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow2>(), 0, 0, default, npc.direction, 0);
                            }
                            npc.ai[2]++;
                            int freq = Raged ? 13 : 10;
                            if (npc.ai[2] % freq == 5 && npc.ai[2] > 40 && npc.ai[2] < 80)
                            {
                                Main.PlaySound(SoundID.Item71, npc.Center);
                                Projectile.NewProjectile(player.Center + (player.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * Main.rand.Next(10), (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), ModContent.ProjectileType<ShadowLaser>(), npc.damage / 4, 0, default);
                            }
                            if (npc.ai[2] == 89)
                            {
                                npc.Center = new Vector2(SpawnPos.X - Math.Sign(SpawnPos.X - player.Center.X) * Main.rand.Next(100, 700), npc.Center.Y);
                            }
                            if (npc.ai[2] == 90)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow>(), 0, 0, default, npc.direction, 1);
                            }
                            if (npc.ai[2] == 105)
                            {
                                npc.hide = false;
                                npc.dontTakeDamage = false;
                            }
                            if (npc.ai[2] > 115)
                            {
                                npc.ai[2] = 0;

                                npc.ai[1] = Main.rand.NextBool() ? 1 : 9;

                            }
                        }
                        break;
                    case 1:                     //挥剑
                        {
                            npc.ai[2]++;
                            if (npc.ai[2] > 50)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1]++;
                            }
                        }
                        break;
                    case 2:                     //停顿
                        {
                            npc.ai[2]++;
                            int timer = Raged ? 60 : 30;
                            if (npc.ai[2] > timer)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = Main.rand.NextBool() ? 6 : 3;

                            }
                        }
                        break;
                    case 3:                         //跃起然后落下
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                                npc.velocity = new Vector2(10 * npc.direction, -15);
                                npc.localAI[0] = Terraria.Utils.Clamp(SpawnPos.X, SpawnPos.X - 800, SpawnPos.X + 800);
                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 300)
                                {
                                    npc.localAI[0] = Terraria.Utils.Clamp(npc.Center.X + Math.Sign(SpawnPos.X - npc.Center.X) * 300, SpawnPos.X - 800, SpawnPos.X + 800);
                                }
                                npc.localAI[1] = player.Center.Y;
                            }
                            npc.direction = npc.spriteDirection = Math.Sign(SpawnPos.X - npc.Center.X);
                            if (npc.velocity.Y != 0)
                            {
                                //if (npc.Center.Y > npc.localAI[1])
                                //{
                                //    npc.velocity.Y -= 0.5f;
                                //}

                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 16)
                                {
                                    npc.velocity.X = 0;
                                    npc.velocity.Y += 0.6f; 
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
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FallenExplosion>(), npc.damage / 4, 0);
                                    int amount = Raged ? 7 : 10;
                                    for (int i = 0; i < amount; i++)
                                    {
                                        Projectile.NewProjectile(SpawnPos + new Vector2(Main.rand.Next(-800, 800), 100), (Main.rand.NextFloat() * MathHelper.Pi / 3 - MathHelper.Pi / 6 - MathHelper.Pi / 2).ToRotationVector2() * 32, ModContent.ProjectileType<VileThornBaseHostile>(), npc.damage / 4, 0, default);
                                    }
                                }
                                if (npc.ai[2] > 80)
                                {
                                    npc.ai[2] = 0;
                                    npc.ai[1] = 4;
                                }
                            }
                        }
                        break;
                    case 4:                  //后跃
                        {
                            if (npc.ai[2] == 0)
                            { 
                                npc.ai[2]++;
                                npc.localAI[0] = Terraria.Utils.Clamp(player.Center.X, SpawnPos.X - 700, SpawnPos.X + 700);
                                if(Math.Abs(npc.localAI[0] - npc.Center.X) < 200)
                                {
                                    npc.localAI[0] = npc.Center.X + Math.Sign(npc.localAI[0] - npc.Center.X) * 200;
                                }
                                npc.direction = Math.Sign(npc.localAI[0] - npc.Center.X);
                                npc.velocity = new Vector2(-12 * npc.direction, -7);
                            }

                            if (npc.velocity.Y != 0)
                            {


                                if (Math.Abs(npc.localAI[0] - npc.Center.X) < 16)
                                {
                                    npc.velocity.X = 0;
                                    npc.velocity.Y += 0.5f;
                                }
                                /*
                                else
                                {
                                    npc.velocity.X += 0.5f * (npc.localAI[0] - npc.Center.X);
                                    if (npc.velocity.X > 18) npc.velocity.X = 18;
                                    if (npc.velocity.X < -18) npc.velocity.X = -18;
                                }
                                */
                            }
                            if (npc.velocity.Y == 0)
                            {
                                npc.velocity.X *= 0.8f;
                                npc.ai[2]++;
                                if (npc.ai[2] > 10)
                                {
                                    npc.ai[2] = 0;
                                    npc.ai[1] = 5;
                                }
                            }
                        }
                        break;
                    case 5:                //冲刺
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.localAI[0] = Math.Sign(player.Center.X - npc.Center.X);
                                npc.localAI[1] = Math.Sign(player.Center.X - npc.Center.Y);
                                npc.velocity.X = npc.localAI[0] * 45;
                            }
                            npc.ai[2]++;
                            if (npc.ai[2] < 20)
                            {
                                npc.direction = npc.spriteDirection = (int)npc.localAI[0];

                                if (npc.ai[2] == 10 || npc.ai[2] == 19) 
                                {
                                    Projectile.NewProjectile(npc.Center, new Vector2(Math.Sign(npc.velocity.X) * 35, 0), ModContent.ProjectileType<ShadowBolt>(), npc.damage / 4, 0);
                                }
                                if ((npc.Center.X > SpawnPos.X + 780 && npc.velocity.X > 0) || (npc.Center.X < SpawnPos.X - 790 && npc.velocity.X < 0)) 
                                {
                                    npc.velocity = Vector2.Zero;
                                    npc.ai[2] = 20;
                                }

                            }
                            if (npc.ai[2] < 30)
                            {
                                int freq = Raged ? 5 : 3;
                                if (npc.ai[2] % freq == 1)
                                {
                                    Main.PlaySound(SoundID.Item20, npc.Center);
                                    Projectile.NewProjectile(npc.Center, (npc.localAI[0] > 0 ? -MathHelper.Pi / 4 : -MathHelper.Pi / 4 * 3).ToRotationVector2() * 3, ModContent.ProjectileType<ShadowBolt2>(), npc.damage / 4, 0);
                                }
                            }
                            if (npc.ai[2] >= 20)
                            {
                                if ((npc.Center.X > SpawnPos.X + 780 && npc.velocity.X > 0) || (npc.Center.X < SpawnPos.X - 790 && npc.velocity.X < 0))
                                {
                                    npc.velocity = Vector2.Zero;
                                }

                                npc.velocity *= 0.9f;

                                if (npc.ai[2] < 50)
                                {
                                    npc.direction = npc.spriteDirection = (int)npc.localAI[0];
                                }
                            }
                            if (npc.ai[2] > 90)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = Main.rand.NextBool() ? 0 : 8;
                            }
                        }
                        break;
                    case 6:                     //瞬移
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.hide = true;
                                npc.dontTakeDamage = true;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow2>(), 0, 0, default, npc.direction, 0);
                                npc.localAI[0] = player.Center.X;
                            }
                            npc.ai[2]++;
                            if (npc.ai[2] == 29)
                            {
                                npc.Center = new Vector2(npc.localAI[0], SpawnPos.Y - 400);
                            }
                            if (npc.ai[2] == 30)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow>(), 0, 0, default, npc.direction, 1);
                            }
                            if (npc.ai[2] == 45)
                            {
                                npc.hide = false;
                                npc.dontTakeDamage = false;
                                npc.velocity.Y = -2f;
                            }
                            if (npc.ai[2] >= 29 && npc.ai[2] < 45)
                            {
                                npc.velocity.Y = 0;
                            }
                            if (npc.ai[2] > 45)
                            {
                                npc.velocity.X = 0;
                                if (player.Center.X != npc.Center.X)
                                {
                                    if (Math.Abs(player.Center.X - npc.Center.X) < 7)
                                    {
                                        npc.Center = new Vector2(player.Center.X, npc.Center.Y);
                                        npc.velocity.X = 0;
                                    }
                                    else
                                    {
                                        npc.velocity.X = 7 * Math.Sign(player.Center.X - npc.Center.X);
                                    }
                                }
                            }
                            if (npc.ai[2] == 60)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1]++;
                            }
                        }
                        break;
                    case 7:                 //下砸
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.velocity.Y = 15;
                                npc.ai[2]++;
                            }
                            if (!Collision.SolidCollision(npc.position + new Vector2(0, 15), npc.width, npc.height))
                            {
                                npc.position.Y += 15;
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
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FallenExplosion>(), npc.damage / 4, 0);
                                }
                                if (!Raged)
                                {
                                    if (npc.ai[2] > 5 && npc.ai[2] <= 60 && npc.ai[2] % 5 == 0)
                                    {
                                        Projectile.NewProjectile(npc.Center + new Vector2(800 * npc.ai[2] / 60, 20), (-MathHelper.Pi / 2).ToRotationVector2() * 32, ModContent.ProjectileType<VileThornBaseHostile>(), npc.damage / 4, 0, default);
                                        Projectile.NewProjectile(npc.Center + new Vector2(-800 * npc.ai[2] / 60, 20), (-MathHelper.Pi / 2).ToRotationVector2() * 32, ModContent.ProjectileType<VileThornBaseHostile>(), npc.damage / 4, 0, default);
                                    }
                                }
                                else
                                {
                                    if (npc.ai[2] > 5 && npc.ai[2] <= 60 && npc.ai[2] % 8 == 0)
                                    {
                                        Projectile.NewProjectile(npc.Center + new Vector2(800 * npc.ai[2] / 60, 20), (-MathHelper.Pi / 2).ToRotationVector2() * 32, ModContent.ProjectileType<VileThornBaseHostile>(), npc.damage / 4, 0, default);
                                        Projectile.NewProjectile(npc.Center + new Vector2(-800 * npc.ai[2] / 60, 20), (-MathHelper.Pi / 2).ToRotationVector2() * 32, ModContent.ProjectileType<VileThornBaseHostile>(), npc.damage / 4, 0, default);
                                    }
                                }
                            }
                            if (npc.ai[2] > 140)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = 4;
                            }
                        }
                
                        
                        break;
                    case 8:   //消失分身
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.hide = true;
                                npc.dontTakeDamage = true;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow2>(), 0, 0, default, npc.direction, 0);
                                npc.localAI[0] = player.Center.X;
                            }
                            npc.ai[2]++;
                            if (npc.ai[2] > 30 && npc.ai[2] < 120) 
                            { 
                                int freq = Raged ? 20: 15;
                                if (npc.ai[2] % freq == 0)
                                {
                                    Vector2 SummonPos = player.Center + (Main.rand.Next() * MathHelper.TwoPi).ToRotationVector2() * 500;
                                    int safety = 0;
                                    while (Collision.SolidCollision(SummonPos, 1, 1) || !Utils.NPCUtils.InWall(SummonPos,ModContent.WallType<ArenaWall>())) 
                                    {
                                        SummonPos = player.Center + (Main.rand.Next() * MathHelper.TwoPi).ToRotationVector2() * 500;
                                        safety++;
                                        if (safety > 1000)
                                        {
                                            break;
                                        }
                                    }
                                    Projectile.NewProjectile(SummonPos, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerClone>(), npc.damage / 4, 0, default);
                                }
                            }
                            if (npc.ai[2] == 119)
                            {
                                npc.Center = new Vector2(SpawnPos.X + Main.rand.Next(-700, 700), npc.Center.Y);
                            }
                            if (npc.ai[2] == 200)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPlayerShadow>(), 0, 0, default, npc.direction, 1);
                            }
                            if (npc.ai[2] == 215)
                            {
                                npc.hide = false;
                                npc.dontTakeDamage = false;
                            }
                            if (npc.ai[2] > 260)
                            {
                                npc.ai[2] = 0;
                                npc.ai[1] = Main.rand.NextBool() ? 1 : 9;
                            }
                        }
                        
                        break;
                    case 9:                     //平A
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
                                npc.ai[2] = 0;
                                npc.ai[1] = Main.rand.NextBool() ? 6 : 3;
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
                if (proj.active && proj.type == ModContent.ProjectileType<LightsBaneHostile>() && proj.ai[0] == npc.whoAmI)
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
        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.RedPotion;
        }
        

        public override void NPCLoot()
        {
            if (npc.localAI[2] == 1)
            {
                int npctmp = NPC.FindFirstNPC(ModContent.NPCType<CrimsonPlayerBoss>());
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
            MABWorld.DownedPreEvilFighter = true;
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = "暗影守护者 被击败了，凶手是" + Main.LocalPlayer.name + "。";
        }
        private void DP(SpriteBatch spritebatch,Vector2 Pos,Color a)
        {

            Rectangle Frame = new Rectangle(0, (int)(56 * npc.localAI[3]), 40, 56);
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D WeaponTex = Main.itemTexture[ItemID.LightsBane];
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
            Texture2D Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/ShadowPlayerBoss_Legs");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/ShadowPlayerBoss_Body");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/ShadowPlayerBoss_Face");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/ShadowPlayerBoss_Head");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);



        }
        public override void BossHeadSlot(ref int index)
        {
            if (!npc.hide)
            {
                if (OriHeadSlot == -1) 
                {
                    OriHeadSlot = index; 
                }
                else
                {
                    index = OriHeadSlot;
                }
            }
            else
            {
                index = -1;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                float a = (float)(NPCID.Sets.TrailCacheLength[npc.type] - i - 1) / NPCID.Sets.TrailCacheLength[npc.type];
                DP(spriteBatch, npc.Center - Main.screenPosition + (npc.oldPos[i] - npc.position), drawColor * a);
            }
            DP(spriteBatch, npc.Center - Main.screenPosition, drawColor);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
    }
}