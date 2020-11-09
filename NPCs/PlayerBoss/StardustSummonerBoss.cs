using MABBossChallenge.Buffs;
using MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj;
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
    public class StardustSummonerBoss : ModNPC
    {
        private int WingsFrame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Defender");
            DisplayName.AddTranslation(GameCulture.Chinese, "星尘守护者");
            TranslationUtils.AddTranslation("StardustDefender", "Stardust Defender", "星尘守护者");
            TranslationUtils.AddTranslation("StardustDefenderDescription", "The bizarre summoner ruleing the power of stardust", "统领星尘之力的奇妙召唤师");
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.chaseable = true;
            npc.width = 36;
            npc.height = 51;
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
            if (!MABWorld.DownedStardustPlayer)
            {
                NPC.ShieldStrengthTowerStardust = 5;
            }

            npc.TargetClosest();

            Player player = Main.player[npc.target];
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<StardustGuardianHostile>() && proj.ai[0] == npc.whoAmI)
                {
                    proj.owner = player.whoAmI;
                }
            }
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

            if (npc.ai[0] == 0)                  //开局
            {
                npc.GivenName = TranslationUtils.GetTranslation("StardustDefender");
                if (npc.ai[2] == 0)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StardustGuardianHostile>(), npc.damage / 4, 0, player.whoAmI, npc.whoAmI);
                }
                if (!MABWorld.DownedStardustPlayer)
                {
                    npc.ai[2]++;
                    if (npc.ai[2] == 1)
                    {
                        Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(npc.Center, TranslationUtils.GetTranslation("StardustDefender"), TranslationUtils.GetTranslation("StardustDefenderDescription"));
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
                    case 0:               //星尘细胞
                        {
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, false);
                            npc.ai[2]++;

                            if (npc.ai[2] == 100)
                            {
                                Main.PlaySound(SoundID.Item44, npc.Center);
                                for (float i = 0; i < 18; i++)
                                {
                                    switch (i)
                                    {
                                        case 1:
                                        case 2:
                                        case 8:
                                        case 9:
                                        case 15:
                                        case 16:
                                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StardustCellHostile1>(), npc.damage / 4, 0, player.whoAmI, i);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                int protmp;
                                protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StardustCellHostile3>(), npc.damage / 4, 0, player.whoAmI, 0);
                                Main.projectile[protmp].timeLeft = 150;
                                protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StardustCellHostile3>(), npc.damage / 4, 0, player.whoAmI, 1);
                                Main.projectile[protmp].timeLeft = 300;
                                protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<StardustCellHostile3>(), npc.damage / 4, 0, player.whoAmI, 2);
                                Main.projectile[protmp].timeLeft = 450;
                            }

                            if (npc.ai[2] > 660)
                            {
                                npc.ai[1] = 1;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 1:                 //星尘龙
                        {
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, false);
                            npc.ai[2]++;
                            if (npc.ai[2] == 25 || npc.ai[2] == 125 || npc.ai[2] == 225)
                            {
                                Main.PlaySound(SoundID.Item44, npc.Center);
                                Vector2 SummonPos = player.Center + (Main.rand.Next(3) * MathHelper.Pi / 3 * 2 + MathHelper.Pi / 3).ToRotationVector2() * 400f;
                                Vector2 vel = Vector2.Normalize(player.Center - npc.Center) * 5;
                                int current = Projectile.NewProjectile(SummonPos, vel, ModContent.ProjectileType<StardustDragonHostileHead>(), npc.damage / 4, 0f, Main.myPlayer, Main.myPlayer);   //ai[0]=npc.target
                                for (int i = 0; i < 36; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        current = Projectile.NewProjectile(SummonPos, vel, ModContent.ProjectileType<StardustDragonHostileBody1>(), npc.damage / 4, 0f, Main.myPlayer, current);
                                    }
                                    else
                                    {
                                        current = Projectile.NewProjectile(SummonPos, vel, ModContent.ProjectileType<StardustDragonHostileBody2>(), npc.damage / 4, 0f, Main.myPlayer, current);
                                    }
                                }
                                int previous = current;
                                current = Projectile.NewProjectile(SummonPos, vel, ModContent.ProjectileType<StardustDragonHostileTail>(), npc.damage / 4, 0f, Main.myPlayer, current);
                                Main.projectile[previous].localAI[1] = current;
                            }

                            if (npc.ai[2] == 450)
                            {
                                foreach (Projectile D in Main.projectile)
                                {
                                    if (D.active && (D.type == ModContent.ProjectileType<StardustDragonHostileBody1>() || D.type == ModContent.ProjectileType<StardustDragonHostileBody2>() || D.type == ModContent.ProjectileType<StardustDragonHostileTail>() || D.type == ModContent.ProjectileType<StardustDragonHostileHead>()))
                                    {
                                        D.Kill();
                                    }
                                }
                            }
                            if (npc.ai[2] >= 485)
                            {
                                npc.ai[1] = 2;
                                npc.ai[2] = 0;
                            }
                        }
                        break;
                    case 2:              //星尘守卫
                        {
                            Movement(player.Center + Vector2.Normalize(npc.Center - player.Center) * 400, 0.3f, false);
                            npc.ai[2]++;
                            if (npc.ai[2] == 1)
                            {
                                foreach (Projectile proj in Main.projectile)
                                {
                                    if (proj.active && proj.type == ModContent.ProjectileType<StardustGuardianHostile>() && proj.ai[0] == npc.whoAmI)
                                    {
                                        proj.ai[1] = 1;
                                    }
                                }
                            }
                            if (npc.ai[2] >= 500)
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
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {



            //绘制召唤师
            SpriteEffects SP = (npc.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle WingFrame = new Rectangle(0, 62 * WingsFrame, 86, 62);
            Texture2D Tex1 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/StardustSummonerBoss");
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/StardustWings");
            Texture2D Tex3 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/StardustWingsGlow");

            for (int i = 1; i < 4; i++)
            {
                Color color27 = Color.White * npc.Opacity;
                color27 *= (float)(4 - i) / 4;
                Vector2 value4 = npc.position - npc.velocity * i;
                Main.spriteBatch.Draw(Tex2, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 8, 92), WingFrame, color27, 0, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                Main.spriteBatch.Draw(Tex3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY) + new Vector2(-npc.spriteDirection * 8, 92), WingFrame, color27, 0, Tex2.Size() * 0.5f, npc.scale, SP, 0f);
                Main.spriteBatch.Draw(Tex1, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, color27, 0, Tex1.Size() * 0.5f, npc.scale, SP, 0f);
            }
            spriteBatch.Draw(Tex2, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 8, 92) + new Vector2(0, npc.gfxOffY), WingFrame, Color.White, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0f);
            spriteBatch.Draw(Tex3, npc.Center - Main.screenPosition + new Vector2(-npc.spriteDirection * 8, 92) + new Vector2(0, npc.gfxOffY), WingFrame, Color.White, 0, Tex2.Size() * 0.5f, 1.0f, SP, 0f);
            if (WingsFrame == 1 || WingsFrame == 3) spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, Color.White * 0.5f, 0, Tex1.Size() * 0.5f, 1.1f, SP, 0f);
            if (WingsFrame == 2) spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, Color.White * 0.5f, 0, Tex1.Size() * 0.5f, 1.2f, SP, 0f);
            spriteBatch.Draw(Tex1, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), null, Color.White, 0, Tex1.Size() * 0.5f, 1.0f, SP, 0f);

            if (npc.ai[0] == 1)
            {
                Texture2D TexStaff1 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/StardustCellStaff");
                if (npc.ai[1] == 0 && npc.ai[2] > 80 && npc.ai[2] < 112)
                {
                    if (npc.spriteDirection > 0)
                    {
                        spriteBatch.Draw(TexStaff1, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, ((npc.ai[2] - 80) - 16) / 16 * MathHelper.Pi / 3, new Vector2(0, TexStaff1.Size().Y), 1, SP, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(TexStaff1, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (16 - (npc.ai[2] - 80)) / 16 * MathHelper.Pi / 3, TexStaff1.Size(), 1, SP, 0);
                    }
                }

                Texture2D TexStaff2 = MABBossChallenge.Instance.GetTexture("NPCs/PlayerBoss/StardustDragonStaff");
                if (npc.ai[1] == 1 && npc.ai[2] < 32)
                {
                    if (npc.spriteDirection > 0)
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (npc.ai[2] - 16) / 16 * MathHelper.Pi / 3 + MathHelper.Pi / 3, new Vector2(0, TexStaff1.Size().Y), 1, SP, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (16 - npc.ai[2]) / 16 * MathHelper.Pi / 3 - MathHelper.Pi / 3, TexStaff1.Size(), 1, SP, 0);
                    }
                }

                if (npc.ai[1] == 1 && npc.ai[2] < 132 && npc.ai[2] > 100)
                {
                    if (npc.spriteDirection > 0)
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (npc.ai[2] - 16) / 16 * MathHelper.Pi / 3 + MathHelper.Pi / 3, new Vector2(0, TexStaff1.Size().Y), 1, SP, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (16 - npc.ai[2]) / 16 * MathHelper.Pi / 3 - MathHelper.Pi / 3, TexStaff1.Size(), 1, SP, 0);
                    }
                }

                if (npc.ai[1] == 1 && npc.ai[2] < 232 && npc.ai[2] > 200)
                {
                    if (npc.spriteDirection > 0)
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (npc.ai[2] - 16) / 16 * MathHelper.Pi / 3 + MathHelper.Pi / 3, new Vector2(0, TexStaff1.Size().Y), 1, SP, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(TexStaff2, npc.Center + new Vector2(npc.spriteDirection * 6, 4) - Main.screenPosition, null, Color.White, (16 - npc.ai[2]) / 16 * MathHelper.Pi / 3 - MathHelper.Pi / 3, TexStaff1.Size(), 1, SP, 0);
                    }
                }
            }


            return false;
        }


        public override bool PreNPCLoot()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && (proj.type == ModContent.ProjectileType<StardustCellHostile1>() || proj.type == ModContent.ProjectileType<StardustCellHostile3>()))
                {
                    proj.Kill();
                }

                foreach (Projectile D in Main.projectile)
                {
                    if (D.active && (D.type == ModContent.ProjectileType<StardustDragonHostileBody1>() || D.type == ModContent.ProjectileType<StardustDragonHostileBody2>() || D.type == ModContent.ProjectileType<StardustDragonHostileTail>() || D.type == ModContent.ProjectileType<StardustDragonHostileHead>()))
                    {
                        D.Kill();
                    }
                }

            }
            return true;
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
            if (!MABWorld.DownedStardustPlayer)
            {
                NPC.ShieldStrengthTowerStardust = 1;
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.TowerDamageBolt, 0, 0f, Main.myPlayer, NPC.FindFirstNPC(NPCID.LunarTowerStardust), 0f);
            }
            MABWorld.DownedStardustPlayer = true;
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = (npc.ai[0] == 1) ? "星尘守护者" : "星尘使者" + "被击败了，凶手是" + Main.LocalPlayer.name + "。";
            int LootNum = Main.rand.Next(5) + 10;

            for (int i = 0; i < LootNum; i++)
            {
                Item.NewItem(npc.Hitbox, ItemID.FragmentStardust);
            }
            switch (Main.rand.Next(2))
            {
                case 0:
                    Item.NewItem(npc.Hitbox, ItemID.StardustCellStaff);
                    break;
                case 1:
                    Item.NewItem(npc.Hitbox, ItemID.StardustDragonStaff);
                    break;
                default:
                    break;
            }
        }
        private void ClearAll()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<StardustCellHostile1>() || proj.type == ModContent.ProjectileType<StardustCellHostile3>() ||
                        proj.type == ModContent.ProjectileType<StardustDragonHostileBody1>() || proj.type == ModContent.ProjectileType<StardustDragonHostileBody2>()
                        || proj.type == ModContent.ProjectileType<StardustDragonHostileHead>() || proj.type == ModContent.ProjectileType<StardustDragonHostileTail>())
                    {
                        proj.Kill();
                    }
                }
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
            name = npc.GivenName;
        }
    }
}