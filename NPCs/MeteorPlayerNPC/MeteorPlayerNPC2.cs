using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MABBossChallenge.Utils;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.Projectiles.MeteorPlayerNPC;
using Terraria.Utilities;
using System.IO;
using Terraria.Localization;
using MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer;
using MABBossChallenge.Projectiles.PlayerBoss;

namespace MABBossChallenge.NPCs.MeteorPlayerNPC
{
    [AutoloadHead]
    public class MeteorPlayerNPC2 : ModNPC
    {
        private Vector2 SpawnPos = Vector2.Zero;
        private int AI0 = 0, AI1 = 0, AI2 = 0, AI3 = 0;
        private float LocalAI0 = 0, LocalAI1 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Guardian");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨石守卫");
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            TranslationUtils.AddTranslation("MeteorGuardianChat10", "THE BEHAVIOR OF THOSE MONSTERS REALLY ANGERED ME, SO I MUST FIGHT!", "那些怪物的行为激怒了我，所以我必须迎战！");
            TranslationUtils.AddTranslation("MeteorGuardianChat11", "COME ON!", "来吧！");
            TranslationUtils.AddTranslation("MeteorGuardianChat12", "I HAVE NO TIME TO SELL ANYTHING NOW!", "我现在没空去卖什么东西！");
            TranslationUtils.AddTranslation("MeteorGuardianChat13", "How about we come to kill more monsters than who?", "我们来比比谁杀的怪物多怎么样？");
            TranslationUtils.AddTranslation("MeteorGuardianChat14", "After the fight, let's go to ", "等打完了，让我们去");
            TranslationUtils.AddTranslation("MeteorGuardianChat15", "'s to drink something.", "那里喝点什么。");
            TranslationUtils.AddTranslation("MeteorGuardianChat16", "Oh, I won't help you solve the boss, you need to practice", "噢，我不会帮你解决Boss的，你需要锻炼");
        }
        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 36;
            npc.height = 52;
            npc.damage = 120;
            npc.defense = 10;
            npc.lifeMax = 4000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            //npc.dontTakeDamageFromHostiles = true;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            if (Main.hardMode && MABWorld.DownedMeteorPlayerEX)
            {
                npc.lifeMax = 10000;
                npc.defense = 50;
                npc.damage = 300;

                if (NPC.downedMoonlord)
                {
                    npc.lifeMax = 100000;
                    npc.defense = 100;
                    npc.damage = 800;
                }
            }

            if (!Main.expertMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 0.75f);
                npc.damage = (int)(npc.damage * 0.75f);
                npc.life = npc.lifeMax;
            }

            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }

            npc.value = 0;
            
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            npc.lifeRegen += 10;
        }

        public override void AI()
        {
            for (int i = 0; i < npc.buffType.Length; i++) 
            {
                if (Main.debuff[npc.buffType[i]])
                {
                    npc.DelBuff(i);
                }
            }



            if (MABBossChallenge.mabconfig.NPCAttackBGM)
            {
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Pure Tension");
            }
            else
            {
                music = -1;
            }
            npc.GivenName = "无名";
            if (SpawnPos == Vector2.Zero)
            {
                SpawnPos = npc.Center;
            }

            npc.ai[3] = HomeOnTarget();

            if (npc.ai[3] < 0 || npc.ai[3] > 200)
            {
                AI0 = 2;
            }
            if (AI0 == 2)
            {
                MoveToVector2(SpawnPos, 30);
                npc.spriteDirection = npc.direction = Math.Sign(npc.velocity.X);
                if (npc.Distance(SpawnPos) < 16)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC1>()))
                    {
                        NPC npctmp = (NPC)npc.Clone();
                        npc.Transform(ModContent.NPCType<MeteorPlayerNPC1>());
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeless = npctmp.homeless;
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeTileX = npctmp.homeTileX;
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeTileY = npctmp.homeTileY;
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeless = npctmp.oldHomeless;
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeTileX = npctmp.oldHomeTileX;
                        Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeTileY = npctmp.oldHomeTileY;
                    }
                    else
                    {
                        npc.active = false;
                    }
                    return;
                }
                return;
            }

            npc.spriteDirection = npc.direction = Math.Sign(Main.npc[(int)npc.ai[3]].Center.X - npc.Center.X);
            NPC target = Main.npc[(int)npc.ai[3]];
            if (npc.velocity.Length() > 2)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
            }
            if (Main.rand.Next(2) == 0)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position + new Vector2(0, npc.height), npc.width, 10, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity.Y += 5f;
            }

            if (NPC.downedMoonlord)
            {
                if ((target.life > 10000 || TargetCount() >= 10) && !NPC.AnyNPCs(ModContent.NPCType<MeteorHeadFriendly>()))
                {
                    for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 3)
                    {
                        int npct = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MeteorHeadFriendly>());
                        Main.npc[npct].velocity = i.ToRotationVector2() * 10;
                    }
                }
            }


            if (AI0 == 0)
            {
                AI0 = 1;
            }
            if (AI0 == 1)
            {

                switch (AI1)
                {
                    case 0:                   //空间枪攻击
                        {
                            ChangeRot(target.Center);
                            if (AI2 == 0)
                            {
                                npc.velocity = Vector2.Zero;
                                //Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SpaceGunFriendly>(), npc.damage, 0, default, npc.whoAmI);
                                Vector2 Pos = target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 200;
                                LocalAI0 = Pos.X;
                                LocalAI1 = Pos.Y;
                                AI2++;
                            }
                            MoveToVector2(new Vector2(LocalAI0, LocalAI1), 20);

                            if (Vector2.Distance(new Vector2(LocalAI0, LocalAI1), npc.Center) < 16)
                            {
                                AI2++;
                                npc.velocity *= 0.8f;
                                if (NPC.downedMoonlord && MABWorld.DownedMeteorPlayerEX)
                                {
                                    if (AI2 % 6 == 2 && AI2 < 60 && AI2 > 12)
                                    {
                                        Vector2 Facing = Vector2.Normalize(target.Center - npc.Center + target.velocity / 2);
                                        Main.PlaySound(SoundID.Item12, npc.Center);
                                        int protmp = Projectile.NewProjectile(npc.Center, Facing * 25, ProjectileID.PurpleLaser, npc.damage, 1, Main.myPlayer);
                                        Main.projectile[protmp].tileCollide = false;
                                        Main.projectile[protmp].magic = false;
                                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                                    }
                                }
                                else
                                {
                                    if (AI2 % 12 == 2 && AI2 < 60 && AI2 > 12)
                                    {
                                        Vector2 Facing = Vector2.Normalize(target.Center - npc.Center);
                                        Main.PlaySound(SoundID.Item12, npc.Center);
                                        int protmp = Projectile.NewProjectile(npc.Center, Facing * 25, ProjectileID.GreenLaser, npc.damage, 1, Main.myPlayer);
                                        Main.projectile[protmp].tileCollide = false;
                                        Main.projectile[protmp].magic = false;
                                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                                    }
                                }
                                
                                if (AI2 > 100)
                                {
                                    AI1 = 0;
                                    AI2 = 0;
                                    AI3++;
                                    if (AI3 >= 3)
                                    {
                                        AI3 = 0;
                                        AI1++;
                                    }
                                }
                            }
                        }
                        break;
                    case 1:               //停顿
                        {
                            AI2++;
                            npc.velocity *= 0.9f;
                            if (AI2 > 10)
                            {

                                AI2 = 0;
                                AI1++;
                            }
                        }
                        break;
                    case 2:          //光剑冲刺
                        {
                            ChangeRot(target.Center);
                            if (AI2 == 0)
                            {
                                npc.dontTakeDamageFromHostiles = true;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PhaseCounter>(), (int)(npc.damage * 0.4f), 0, Main.myPlayer, npc.whoAmI);
                                AI2++;
                            }
                            if (AI2 == 1)
                            {
                                if (npc.Distance(SpawnPos) > 800)
                                {
                                    MoveToVector2(SpawnPos + Vector2.Normalize(npc.Center - SpawnPos) * 800, 20);
                                }
                                if (npc.Distance(SpawnPos) < 810)
                                {
                                    AI2++;
                                }
                            }
                            if (AI2 >= 2)
                            {
                                AI2++;
                            }
                            if (AI2 >= 80)
                            {
                                if (AI2 % 40 == 0)
                                { 
                                    npc.velocity = Vector2.Normalize(target.Center - npc.Center) * 5;
                                    if(MABWorld.DownedMeteorPlayerEX && NPC.downedMoonlord)
                                    {
                                        npc.velocity = Vector2.Normalize(target.Center - npc.Center + target.velocity / 3 * 2) * 5;
                                    }
                                    Main.PlaySound(SoundID.Item15, target.Center);
                                }
                                if (AI2 % 40 > 0 && AI2 % 40 < 10) 
                                {
                                    if (NPC.downedMoonlord && MABWorld.DownedMeteorPlayerEX) 
                                    {
                                        npc.velocity *= 1.3f;
                                    }
                                    else
                                    {
                                        npc.velocity *= 1.26f;
                                    }
                                }
                                if (AI2 % 40 == 10 && MABWorld.DownedMeteorPlayerEX && NPC.downedMoonlord)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.velocity / 1000, ModContent.ProjectileType<HotLaserFriendly>(), npc.damage / 4, 0, Main.myPlayer);
                                }
                                if (AI2 % 40 >= 10 && AI2 % 40 < 35)
                                {
                                    npc.velocity *= 0.92f;
                                    ChangeRot(npc.Center + npc.velocity);
                                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                                }
                                if (AI2 % 40 > 35)
                                {
                                    npc.velocity *= 0.96f;
                                }
                                if (AI2 > 39 + 40 * 5 + 80)
                                {
                                    npc.dontTakeDamageFromHostiles = false;
                                    AI2 = 0;
                                    AI1++;
                                }
                            }
                        }
                        break;
                    case 3:             //停顿
                        {
                            AI2++;
                            npc.velocity *= 0.9f;
                            if (AI2 > 10)
                            {

                                AI2 = 0;
                                AI1++;
                            }
                        }
                        break;
                    case 4:                //EZ炮
                        {
                            ChangeRot(target.Center);
                            if (AI2 == 0)
                            {
                                npc.velocity = Vector2.Zero;
                                //Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EZCannonFriendly>(), npc.damage, 0, default, npc.whoAmI);
                                Vector2 Pos = target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 300;
                                LocalAI0 = Pos.X;
                                LocalAI1 = Pos.Y;
                                AI2++;
                            }
                            MoveToVector2(new Vector2(LocalAI0, LocalAI1), 20);

                            if (Vector2.Distance(new Vector2(LocalAI0, LocalAI1), npc.Center) < 16)
                            {
                                AI2++;
                                npc.velocity *= 0.8f;

                                if (AI2 == 40)
                                {
                                    Vector2 Facing = Vector2.Normalize(target.Center - npc.Center);
                                    if (MABWorld.DownedMeteorPlayerEX && NPC.downedMoonlord)
                                    {
                                        Projectile.NewProjectile(npc.Center, Facing * 15, ModContent.ProjectileType<SuperStarFriendly>(), npc.damage, 0, Main.myPlayer);
                                    }
                                    else
                                    {
                                        Projectile.NewProjectile(npc.Center, Facing * 15, ModContent.ProjectileType<EZStarFriendlyS>(), (int)(npc.damage * 1.2f), 0, Main.myPlayer);
                                    }
                                }

                                if (AI2 > 100)
                                {
                                    AI2 = 0;
                                    AI3++;
                                    if (AI3 >= 3)
                                    {
                                        AI3 = 0;
                                        AI1++;
                                    }
                                }
                            }
                        }
                        break;
                    case 5:              //停顿
                        {
                            AI2++;
                            npc.velocity *= 0.9f;
                            if (AI2 > 10)
                            {
                                AI2 = 0;
                                AI1 = (Main.hardMode && MABWorld.DownedMeteorPlayerEX) ? 6 : 0;
                            }
                        }
                        break;
                    case 6:                 //陨石
                        {
                            ChangeRot(target.Center);
                            MoveToVector2(SpawnPos, 20);
                            if (npc.Distance(SpawnPos) < 16)
                            {
                                AI2++;
                                if (AI2 % 9 == 2)
                                {
                                    WeightedRandom<int> type = new WeightedRandom<int>();
                                    type.Add(ProjectileID.Meteor1);
                                    type.Add(ProjectileID.Meteor2);
                                    type.Add(ProjectileID.Meteor3);
                                    Vector2 TargetPos = target.Center + npc.velocity * 15f + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10);
                                    Vector2 Pos = target.Center + new Vector2(Main.rand.Next(-80, 80), -1500);
                                    int protmp = Projectile.NewProjectile(Pos, Vector2.Normalize(TargetPos - Pos) * 20, type, npc.damage, 0f, Main.myPlayer, 0f, Main.rand.NextFloat(1f, 1.5f));
                                    Main.projectile[protmp].tileCollide = false;
                                    Main.projectile[protmp].magic = false;
                                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                                }
                            }
                            if (AI2 > 300)
                            {
                                AI2 = 0;
                                AI1++;
                            }

                        }
                        break;
                    case 7:              //停顿
                        {
                            AI2++;
                            npc.velocity *= 0.9f;
                            if (AI2 > 10)
                            {
                                AI2 = 0;
                                AI1 = 0;
                            }
                        }
                        break;
                    default:
                        AI1 = 0;
                        break;
                }
            }

            if (AI0 == 2)
            {
                npc.velocity *= 0.9f;
                AI2++;
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 12f;
                }
                if (AI2 > 240)
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
            
            

        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat10"));
            chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat11"));
            chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat12"));
            if (Main.eclipse || Main.bloodMoon)
            {
                chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat13"));
                if (NPC.AnyNPCs(550))
                {
                    int FindNPC = NPC.FindFirstNPC(550);
                    chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat14") + Main.npc[FindNPC].GivenName + TranslationUtils.GetTranslation("MeteorGuardianChat15"));
                }
            }

            if (Utils.NPCUtils.AnyBosses() && !MABBossChallenge.mabconfig.NPCAttackBoss)
            {
                chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat16"), 99999);
            }
            return chat;
        }
        public override bool CheckDead()
        {
            if (AI0 < 2)
            {
                npc.life = 1;
                npc.dontTakeDamageFromHostiles = true;
                npc.dontTakeDamage = true;
                AI0 = 2;
                AI1 = 0;
                AI2 = 0;
                AI3 = 0;
                LocalAI0 = 0;
                LocalAI1 = 0;
                return false;
            }
            return true;
        }

        public override void NPCLoot()
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D tex = Main.npcTexture[npc.type];
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(tex, npc.Center - Main.screenPosition, null, drawColor, 0, tex.Size() / 2, npc.scale, SP, 0);
            if (AI1 == 0)
            {
                if (NPC.downedMoonlord && MABWorld.DownedMeteorPlayerEX)
                {
                    DrawWeapon(spriteBatch, ItemID.LaserRifle, drawColor);
                }
                else
                {
                    DrawWeapon(spriteBatch, ItemID.SpaceGun, drawColor);
                }
            }
            if (AI1 == 2)
            {
                DrawWeapon(spriteBatch, ItemID.BluePhaseblade, drawColor);
            }
            if (AI1 == 4)
            {
                if (NPC.downedMoonlord && MABWorld.DownedMeteorPlayerEX)
                {
                    Texture2D Tex = Main.projectileTexture[ModContent.ProjectileType<SuperEZShooterHostile>()];

                    if (SP == SpriteEffects.None)
                    {
                        spriteBatch.Draw(Tex, npc.Center - Main.screenPosition, null, drawColor, npc.rotation - MathHelper.Pi / 2, new Vector2(0, Tex.Height / 2), npc.scale, SP, 0);
                    }
                    if (SP == SpriteEffects.FlipHorizontally)
                    {
                        spriteBatch.Draw(Tex, npc.Center - Main.screenPosition, null, drawColor, MathHelper.Pi / 2 * 3 + npc.rotation, new Vector2(Tex.Width, Tex.Height / 2), npc.scale, SP, 0);
                    }

                }
                else
                {
                    DrawWeapon(spriteBatch, ItemID.StarCannon, drawColor);
                }
            }
            if (AI1 == 6)
            {
                DrawWeapon(spriteBatch, ItemID.MeteorStaff, drawColor);
            }
            return false;
        }

        public void DrawWeapon(SpriteBatch spriteBatch, int type, Color color)
        {
            Texture2D Tex = Main.itemTexture[type];
            SpriteEffects SP = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (type != ItemID.BluePhaseblade && type != ItemID.MeteorStaff) 
            {
                if (SP == SpriteEffects.None)
                {
                    spriteBatch.Draw(Tex, npc.Center - Main.screenPosition, null, color, npc.rotation - MathHelper.Pi / 2, new Vector2(0, Tex.Height / 2), npc.scale, SP, 0);
                }
                if (SP == SpriteEffects.FlipHorizontally)
                {
                    spriteBatch.Draw(Tex, npc.Center - Main.screenPosition, null, color, MathHelper.Pi / 2 * 3 + npc.rotation, new Vector2(Tex.Width, Tex.Height / 2), npc.scale, SP, 0);
                }
            }
            else
            {
                spriteBatch.Draw(Tex, npc.Center - Main.screenPosition, null, Color.White, npc.rotation + MathHelper.Pi / 4, new Vector2(0, Tex.Height), npc.scale, SpriteEffects.None, 0);
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public int TargetCount()
        {
            float homingMaximumRangeInPixels = NPCID.Sets.DangerDetectRange[ModContent.NPCType<MeteorPlayerNPC1>()] + 100;
            int count = 0;

            foreach (NPC target in Main.npc)
            {
                if (MABBossChallenge.mabconfig.NPCAttackBoss || target.damage > 0)
                {
                    if (target.active && !target.friendly && !target.immortal && !target.dontTakeDamage && (target.type != NPCID.SkeletonMerchant || !NPCID.Sets.Skeletons.Contains(target.netID)))
                    {
                        if (target.Distance(SpawnPos) <= homingMaximumRangeInPixels)
                        {
                            if (!target.boss || MABBossChallenge.mabconfig.NPCAttackBoss)
                            {
                                count++;
                            }
                        }
                    }
                }

            }
            return count;
        }

        public int HomeOnTarget()
        {
            if(Utils.NPCUtils.AnyBosses() && !MABBossChallenge.mabconfig.NPCAttackBoss)
            {
                return -1;
            }

            float homingMaximumRangeInPixels = NPCID.Sets.DangerDetectRange[ModContent.NPCType<MeteorPlayerNPC1>()] + 100;
            int selectedTarget = -1;
            if (npc.ai[3] > 0 && npc.ai[3] < 200)
            {
                if (Main.npc[(int)npc.ai[3]].active && !Main.npc[(int)npc.ai[3]].dontTakeDamage && Main.npc[(int)npc.ai[3]].Distance(SpawnPos) < homingMaximumRangeInPixels)
                {
                    return (int)npc.ai[3];
                }
            }
            foreach (NPC target in Main.npc)
            {
                if (MABBossChallenge.mabconfig.NPCAttackBoss || target.damage > 0) 
                {
                    if (target.active && !target.friendly && !target.immortal && !target.dontTakeDamage && (target.type != NPCID.SkeletonMerchant || !NPCID.Sets.Skeletons.Contains(target.netID)))
                    {
                        float distance1 = Vector2.Distance(SpawnPos, target.Center);
                        float distance2 = Vector2.Distance(npc.Center, target.Center);
                        if (distance1 <= homingMaximumRangeInPixels && (selectedTarget == -1 || distance2 < Vector2.Distance(Main.npc[selectedTarget].Center, npc.Center)))
                        {
                            selectedTarget = target.whoAmI;
                        }

                    }
                }

            }
            return selectedTarget;
        }
        
        public void ChangeRot(Vector2 TargetPos)
        { 
            Vector2 Facing = Vector2.Normalize(TargetPos - npc.Center);
            if (npc.spriteDirection > 0)
            {
                Vector2 TargetRot = ((float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi / 2).ToRotationVector2();
                npc.rotation = (npc.rotation.ToRotationVector2() + TargetRot * 0.2f).ToRotation();
            }
            else
            {
                Vector2 TargetRot = ((float)Math.Atan2(Facing.Y, Facing.X) - MathHelper.Pi / 2).ToRotationVector2();
                npc.rotation = (npc.rotation.ToRotationVector2() + TargetRot * 0.2f).ToRotation();
            }
            if (AI1 == 2 || AI1 == 6) 
            {
                Vector2 TargetRot = Facing.ToRotation().ToRotationVector2();
                npc.rotation = (npc.rotation.ToRotationVector2() + TargetRot * 4f).ToRotation();
            }
            
        }



        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AI0);
            writer.Write(AI1);
            writer.Write(AI2);
            writer.Write(AI3);
            writer.Write(LocalAI0);
            writer.Write(LocalAI1);
            writer.Write(SpawnPos.X);
            writer.Write(SpawnPos.Y);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AI0 = reader.ReadInt32();
            AI1 = reader.ReadInt32();
            AI2 = reader.ReadInt32();
            AI3 = reader.ReadInt32();
            LocalAI0 = reader.ReadSingle();
            LocalAI1 = reader.ReadSingle();
            SpawnPos.X = reader.ReadSingle();
            SpawnPos.Y = reader.ReadSingle();
        }

        public void MoveToVector2(Vector2 p,float Vel)
        {
            float moveSpeed = Vel;
            float velMultiplier = 1f;
            Vector2 dist = p - npc.Center;
            float length = (dist == Vector2.Zero) ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }
            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }
            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }
            npc.velocity = ((length == 0f) ? Vector2.Zero : Vector2.Normalize(dist));
            npc.velocity *= moveSpeed;
            npc.velocity *= velMultiplier;
        }

        public override string TownNPCName()
        {
            return TranslationUtils.GetTranslation("MeteorGuardianNPCName");
        }


        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
        }
    }

    public class MeteorPlayerNPCDR : GlobalNPC
    {
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.type == ModContent.NPCType<MeteorPlayerNPC2>())
            {
                damage /= 2;
            }
        }
    }
}