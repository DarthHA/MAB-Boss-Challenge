using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer;
using MABBossChallenge.Utils;
using Terraria.DataStructures;
using Terraria.Utilities;
using MABBossChallenge.NPCs.MeteorPlayerNPC;
using Terraria.Localization;

namespace MABBossChallenge.NPCs.MiniPlayerBoss
{
    [AutoloadBossHead]
    public class MeteorPlayerBoss : ModNPC
    {
        private Vector2 SpawnPos = Vector2.Zero;
        private int AtkPattern = 0;
        private int StateRage = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Guardian");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨石守卫");

            Main.npcFrameCount[npc.type] = 20;
            TranslationUtils.AddTranslation("MeteorGuardian", "Meteor Guardian", "陨石守卫");
            TranslationUtils.AddTranslation("MeteorGuardianEnrage", "Your actions have enraged Meteor Guardian!", "你的行为激怒了陨石守卫！");
            TranslationUtils.AddTranslation("MeteorGuardianAwoken", "Meteor Guardian has awokened!", "陨石守卫已苏醒！");
            TranslationUtils.AddTranslation("MeteorGuardianEnrageAgain", "Come on, let me know what you learnt these days!", "来吧，让我看看你最近有多少进步");
            TranslationUtils.AddTranslation("MeteorGuardianGG", "Seems that you need more training.", "看起来你需要更多的锻炼");
            TranslationUtils.AddTranslation("MeteorGuardianBeaten", "Good!", "好！很有精神！");
            TranslationUtils.AddTranslation("MeteorGuardianDescription1", "The mysterious high-tech warrior", "神秘的高科技战士");
            TranslationUtils.AddTranslation("MeteorGuardianDescription2", "The strong warrior who retired", "卸甲归田的强劲战士");
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 36;
            npc.height = 52;
            npc.damage = 60;
            npc.defense = 10;
            npc.lifeMax = 4000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Burning] = true;
            if (Main.hardMode && MABWorld.DownedMeteorPlayer)
            {
                npc.lifeMax = 30000;
                npc.life = 30000;
                npc.damage = 100;
                npc.defense = 30;
                for(int i = 0; i < npc.buffImmune.Length; i++)
                {
                    npc.buffImmune[i] = true;
                }
                if (NPC.downedMoonlord)
                {
                    npc.lifeMax = 100000;
                    npc.life = 100000;
                    npc.damage = 200;
                    npc.defense = 60;
                }
            }
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            npc.dontTakeDamage = true;
            npc.value = 10000;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Pure Tension");
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.damage /= 2;
        }
        public override void AI()
        {
            npc.TargetClosest();
            if (SpawnPos == Vector2.Zero)
            {
                SpawnPos = Main.player[npc.target].Center;
                if (!MABWorld.DownedMeteorPlayer)
                {
                    Main.NewText(TranslationUtils.GetTranslation("MeteorGuardianEnrage"), 175, 75, 255);
                    //CombatText.NewText(npc.Hitbox, Color.Red, "警告！发现入侵者,开始攻击！");
                }
                else
                {
                    Main.NewText(TranslationUtils.GetTranslation("MeteorGuardianAwoken"), 175, 75, 255);
                    npc.GivenName = TranslationUtils.GetTranslation("MeteorGuardianNPCName");
                    Main.NewText(TranslationUtils.GetTranslation("MeteorGuardianEnrageAgain"), Color.Orange);
                    //CombatText.NewText(npc.Hitbox, Color.Red, "来吧，让我看看你最近有多少进步");
                }
            }
            if (NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC1>()))
            {
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].active = false;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC2>()))
            {
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].active = false;
            }
            Arenaing();
            Player player = Main.player[npc.target];
            player.buffImmune[BuffID.OnFire] = false;

            if (player.dead || (player.Center - npc.Center).Length() > 4000)
            {
                npc.ai[0] = 2;
            }
            npc.spriteDirection = npc.direction = Math.Sign(player.Center.X - npc.Center.X);


            if (Main.rand.Next(2) == 1 && npc.ai[0] < 3) 
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;

                dust = Main.dust[Dust.NewDust(npc.position + new Vector2(0, npc.height), npc.width, 10, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity.Y += 5f;
            }


            if (npc.ai[0] == 0)               //开局
            {
                npc.velocity *= 0.9f;
                npc.ai[2]++;
                if (npc.ai[2] == 1)
                {
                    if (!MABWorld.DownedMeteorPlayer)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center,TranslationUtils.GetTranslation("MeteorGuardian"),TranslationUtils.GetTranslation("MeteorGuardianDescription1"));
                    }
                    else
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("MeteorGuardian") + "-" + TranslationUtils.GetTranslation("MeteorGuardianNPCName"), TranslationUtils.GetTranslation("MeteorGuardianDescription2"));
                    }
                } 

                if (npc.ai[2] >= 330)
                {
                    npc.dontTakeDamage = false;
                    npc.ai[0] = 1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }

            }

            if (npc.ai[0] == 1)
            {

                switch (npc.ai[1])
                {
                    case 0:                   //空间枪攻击
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.velocity = Vector2.Zero;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SpaceGunHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                                Vector2 Pos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 200;
                                npc.localAI[0] = Pos.X;
                                npc.localAI[1] = Pos.Y;
                                npc.ai[2]++;
                            }
                            MoveToVector2(new Vector2(npc.localAI[0], npc.localAI[1]), 20);
                            
                            if(Vector2.Distance(new Vector2(npc.localAI[0], npc.localAI[1]),npc.Center) < 16)
                            {
                                npc.ai[2]++;
                                npc.velocity *= 0.8f;
                                if (npc.ai[2] > 100)
                                {
                                    npc.ai[1] = 0;
                                    npc.ai[2] = 0;
                                    npc.ai[3]++;
                                    if (npc.ai[3] >= 3)
                                    {
                                        npc.ai[3] = 0;
                                        npc.ai[1]++;
                                    }
                                }
                            }
                        }
                        break;
                    case 1:               //停顿
                        {
                            npc.ai[2]++;
                            npc.velocity *= 0.9f;
                            if (npc.ai[2] > 30)
                            {

                                npc.ai[2] = 0;
                                npc.ai[1] = UseAtkPattern();
                            }
                        }
                        break;
                    case 2:          //光剑冲刺
                        {
                            if (npc.ai[2] == 0)
                            {
                                int damage = 30 / 4;
                                if (Main.hardMode && MABWorld.DownedMeteorPlayer) damage = 60 / 4;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PhraseBladeHostile>(), damage, 0, default, npc.whoAmI);
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] == 1)
                            {
                                if (npc.Distance(SpawnPos) > 800) 
                                {
                                    MoveToVector2(SpawnPos + Vector2.Normalize(npc.Center - SpawnPos) * 800, 20);
                                }
                                if (npc.Distance(SpawnPos) < 810) 
                                {
                                    npc.ai[2]++;
                                }
                            }
                            if (npc.ai[2] >= 2)
                            {
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] >= 80)
                            {
                                if (npc.ai[2] % 40 == 0)
                                {
                                    float PredistIndex = 1 / 4;
                                    if (npc.life < npc.lifeMax / 3 * 2) PredistIndex = 3 / 8;
                                    if (npc.life < npc.lifeMax / 3) PredistIndex = 1 / 2;
                                    if (Main.hardMode && MABWorld.DownedMeteorPlayer) PredistIndex = 1 / 2;
                                    npc.velocity = Vector2.Normalize(player.Center - npc.Center) * 50 + player.velocity * PredistIndex;
                                    Main.PlaySound(SoundID.Item15, player.Center);
                                }
                                if (npc.ai[2] % 40 > 0)
                                {
                                    npc.velocity *= 0.94f;
                                }
                                if (npc.ai[2] % 40 < 35)
                                {
                                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                                }
                                if (npc.ai[2] > 39 + 40 * 5 + 80)
                                {
                                    npc.ai[2] = 0;
                                    npc.ai[1]++;
                                }
                            }
                        }
                        break;
                    case 3:             //停顿
                        {
                            npc.ai[2]++;
                            npc.velocity *= 0.9f;
                            if (npc.ai[2] > 30)
                            {

                                npc.ai[2] = 0;
                                npc.ai[1] = UseAtkPattern();
                            }
                        }
                        break;
                    case 4:                //EZ炮
                        {
                            if (npc.ai[2] == 0)
                            {
                                npc.velocity = Vector2.Zero;
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EZCannonHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                                Vector2 Pos = player.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 200;
                                npc.localAI[0] = Pos.X;
                                npc.localAI[1] = Pos.Y;
                                npc.ai[2]++;
                            }
                            MoveToVector2(new Vector2(npc.localAI[0], npc.localAI[1]), 20);

                            if (Vector2.Distance(new Vector2(npc.localAI[0], npc.localAI[1]), npc.Center) < 16)
                            {
                                npc.ai[2]++;
                                npc.velocity *= 0.8f;
                                if (npc.ai[2] > 100)
                                {
                                    npc.ai[2] = 0;
                                    npc.ai[3]++;
                                    if (npc.ai[3] >= 3)
                                    {
                                        npc.ai[3] = 0;
                                        npc.ai[1]++;
                                    }
                                }
                            }
                        }
                        break;
                    case 5:              //停顿
                        {
                            npc.ai[2]++;
                            npc.velocity *= 0.9f;
                            if (npc.ai[2] > 30)
                            {

                                npc.ai[2] = 0;
                                npc.ai[1] = UseAtkPattern();
                            }
                        }
                        break;
                    case 6:                    //召唤EZ炮
                        {
                            MoveToVector2(SpawnPos, 20);
                            if (npc.Distance(SpawnPos) < 16 && npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3 * 2)
                                {
                                    int protmp = Projectile.NewProjectile(SpawnPos, Vector2.Zero, ModContent.ProjectileType<EZCannonMinion>(), npc.damage / 5, 0, default, i);
                                    Main.projectile[protmp].localAI[0] = SpawnPos.X;
                                    Main.projectile[protmp].localAI[1] = SpawnPos.Y;
                                }
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EZCannonHostile>(), npc.damage / 4, 0, default, npc.whoAmI);
                            }
                            if (npc.ai[2] > 0)
                            {
                                npc.ai[2]++;
                                if (npc.ai[2] > 360) 
                                {
                                    npc.ai[2] = 0;
                                    npc.ai[1] = 0;
                                }
                            }
                        }
                        break;
                    case 7:             //召唤陨石头
                        {

                            MoveToVector2(SpawnPos, 20);
                            if (npc.Distance(SpawnPos) < 16 && npc.ai[2] == 0)
                            {
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] > 0)
                            {
                                npc.ai[2]++;
                            }
                            if (npc.ai[2] == 45)
                            {
                                Main.PlaySound(SoundID.Roar, player.Center, 0);
                            }
                            if (npc.ai[2] == 90)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    int dust = Dust.NewDust(npc.Center, 0, 0, 86, default, default, default, Color.Yellow, 2);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 10;
                                }
                            }
                            if (npc.ai[2] == 150)
                            {
                                for (int i = 0; i < ((StateRage == 1) ? 3 : 5); i++) 
                                {
                                    int npctmp = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MeteorHeadMinion>());
                                    Main.npc[npctmp].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 15;
                                }


                            }
                            if (npc.ai[2] > 150)
                            {
                                if (Main.hardMode && MABWorld.DownedMeteorPlayer)
                                {
                                    int Freq = 15;
                                    if (npc.life < npc.life / 3 * 2)
                                    {
                                        Freq = 10;
                                    }
                                    if (npc.ai[2] % Freq == 2)
                                    {
                                        Vector2 Pos = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(100);
                                        Projectile.NewProjectile(player.Center + Pos, Vector2.Zero, ModContent.ProjectileType<MeteorMarkHostile>(), npc.damage / 4, 0);
                                    }
                                }
                                if (!NPC.AnyNPCs(ModContent.NPCType<MeteorHeadMinion>()))
                                {
                                    npc.dontTakeDamage = false;
                                    npc.ai[1] = 1;
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
            
            if (npc.ai[0] == 2)
            {
                if (!MABWorld.DownedMeteorPlayer)
                {
                    npc.velocity += new Vector2(0, -1);
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                }
                else
                {
                    CombatText.NewText(npc.Hitbox, Color.Yellow, TranslationUtils.GetTranslation("MeteorGuardianGG"));
                    npc.Transform(ModContent.NPCType<MeteorPlayerNPC1>());
                }
                return;
            }
            if (npc.ai[0] < 2)
            {
                if (npc.life < npc.lifeMax / 3 * 2 && StateRage == 0) 
                {
                    StateRage = 1;
                    npc.dontTakeDamage = true;
                    npc.damage *= 57 / 55;
                    npc.ai[1] = 7;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                }
                if (npc.life < npc.lifeMax / 3 && StateRage == 1)
                {
                    StateRage = 2;
                    npc.dontTakeDamage = true;
                    npc.damage *= 60 / 57;
                    npc.ai[1] = 7;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                }
            }


            if (npc.ai[0] == 3)                 //第二次击败
            {
                npc.velocity *= 0.9f;
                npc.ai[2]++;
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 12f;
                }
                if (npc.ai[2] > 240)
                {
                    CombatText.NewText(npc.Hitbox, Color.Yellow, TranslationUtils.GetTranslation("MeteorGuardianBeaten"));
                    npc.life = 0;
                    npc.checkDead();
                }
            }

            if (npc.ai[0] == 4)                    //首次击败
            {
                npc.ai[2]++;
                if (npc.ai[2] < 240)
                {
                    npc.velocity *= 0.9f;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 12f;
                    }
                }
                if (npc.ai[2] > 240)
                {
                    npc.velocity.Y -= 0.3f;
                    Dust dust = Main.dust[Dust.NewDust(npc.position + new Vector2(0, npc.height), npc.width, 10, 6, 0f, 0f, 100, default, 2f)];
                    dust.noGravity = true;
                    dust.scale = 1.7f;
                    dust.fadeIn = 0.5f;
                    dust.velocity.Y += 5f;
                }
                if (npc.ai[2] > 480 || npc.Distance(Main.player[Player.FindClosest(npc.Center, 1, 1)].Center) > 3000 || npc.Center.Y < 100) 
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.RedPotion;
        }

        public override void NPCLoot()
        {
            MABWorld.DownedMeteorPlayer = true;
        }
        private void DP(SpriteBatch spritebatch, Vector2 Pos, Color a)
        {
            if (npc.ai[1] == 7)
            {
                Texture2D tex = MABBossChallenge.Instance.GetTexture("Images/BubbleShield");
                spritebatch.Draw(tex, npc.Center - Main.screenPosition, null, Color.White, 0, tex.Size() * 0.5f, npc.scale * 1.2f, SpriteEffects.None, 0);
            }
            Rectangle Frame = new Rectangle(0, 56 * 5, 40, 56);
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Legs");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Frame = new Rectangle(0, 56 * 6, 40, 56);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Body");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Frame = new Rectangle(0, 56 * 5, 40, 56);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Head");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);

            if (npc.ai[1] == 7 && npc.ai[2] > 150 && Main.hardMode && MABWorld.DownedMeteorPlayer)
            {
                Tex = Main.itemTexture[ItemID.MeteorStaff];
                Vector2 Facing = Main.player[npc.target].Center - npc.Center;
                float FacingR = (float)Math.Atan2(Facing.Y, Facing.X);
                spritebatch.Draw(Tex, Pos + new Vector2(0, 6), null, a, FacingR + MathHelper.Pi / 4, new Vector2(0, Tex.Height), 1, SpriteEffects.None, 0);
            }


        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            DP(spriteBatch, npc.Center - Main.screenPosition, drawColor);
            return false;
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = (npc.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private int UseAtkPattern()
        {
            if (AtkPattern == 0)
            {
                int[] Seed = { 135, 531, 153, 513, 315, 351 };
                AtkPattern = Seed[Main.rand.Next(Seed.Length)];
            }

            int result;
            if (AtkPattern > 100)
            {
                result = AtkPattern / 100;
                AtkPattern -= result * 100;

            }
            else if (AtkPattern > 10)
            {
                result = AtkPattern / 10;
                AtkPattern -= result * 10;
            }
            else
            {
                result = AtkPattern;
                AtkPattern = 0;
            }
            return result - 1;

        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
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


        public override bool CheckDead()
        {
            if (npc.ai[0] < 3)
            {
                npc.dontTakeDamage = true;
                npc.life = 1;
                npc.ai[0] = MABWorld.DownedMeteorPlayer ? 3 : 4;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.localAI[3] = 0;
                npc.localAI[0] = 0;
                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.active)
                    {
                        if (proj.type == ModContent.ProjectileType<SpaceGunHostile>() ||
                            proj.type == ModContent.ProjectileType<PhraseBladeHostile>() ||
                            proj.type == ModContent.ProjectileType<EZCannonHostile>() ||
                            proj.type == ModContent.ProjectileType<EZCannonMinion>())
                        {
                            proj.Kill();
                        }
                    }
                }
                return false;
            }
            else
            {
                if (Main.hardMode && MABWorld.DownedMeteorPlayer)
                {
                    MABWorld.DownedMeteorPlayerEX = true;
                }
                if (MABWorld.DownedMeteorPlayer)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MeteorPlayerNPC1>());
                }
            }
            return true;
        }

        private void Arenaing()
        {
            for(int i = 0; i < 25; i++)
            {
                Vector2 Pos = SpawnPos + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 900;
                Dust dust = Dust.NewDustDirect(Pos, 1, 1, MyDustId.Fire, 0, 0, 0, default, 2);
                dust.noGravity = true;
            }
            foreach(Player player in Main.player)
            {
                if (player.active && !player.dead && player.Distance(SpawnPos) > 900)
                {
                    player.AddBuff(BuffID.OnFire, 2);
                    player.statLife -= 5;
                    CombatText.NewText(player.Hitbox, CombatText.DamagedHostileCrit, 5);
                    if (player.statLife <= 4)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "被烧死了"), 1, 0);
                    }
                }
            }
        }

    }
}