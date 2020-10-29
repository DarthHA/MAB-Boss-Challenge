using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.MeteorPlayerNPC
{
    public class MeteorHeadFriendly : ModNPC
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Head");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨星头");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.friendly = true;
            npc.width = 24;
            npc.height = 24;
            npc.damage = 1000;
            npc.lifeMax = 10000;
            npc.HitSound = SoundID.NPCHit3;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.dontTakeDamageFromHostiles = true;
            npc.dontTakeDamage = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
            npc.damage /= 2;
        }
        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC2>()))
            {
                npc.life = 0;
                npc.HitEffect();
            }
            
            if (npc.ai[3] < 0 || npc.ai[3] > 200)
            {
                npc.ai[3] = HomeOnTarget();
            }
            if (!Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].dontTakeDamage || Main.npc[(int)npc.ai[3]].friendly || !Main.npc[(int)npc.ai[3]].CanBeChasedBy())
            {
                npc.ai[3] = HomeOnTarget();
            }
            if(!MABBossChallenge.mabconfig.NPCAttackBoss && Main.npc[(int)npc.ai[3]].boss)
            {
                npc.ai[3] = HomeOnTarget();
            }
            if (npc.ai[3] == -1)
            {
                npc.ai[3] = NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>());
                if (npc.Distance(Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].Center) < 16)
                {
                    npc.life = 0;
                    npc.HitEffect();
                }
            }

            NPC target = Main.npc[(int)npc.ai[3]];
            npc.direction = Math.Sign(target.Center.X - npc.Center.X);
            Vector2 Facing = Vector2.Normalize(target.Center - npc.Center);

            if (npc.ai[2] > 0) npc.ai[2]--;
            if (npc.ai[1] > 0) npc.ai[1]--;
            if (npc.ai[2] == 0)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && !n.dontTakeDamage)
                    {
                        if (npc.Hitbox.Intersects(n.Hitbox))
                        {
                            int protmp = Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SolarCounter, npc.damage, 2, Main.myPlayer);
                            Main.projectile[protmp].usesIDStaticNPCImmunity = true;
                            Main.projectile[protmp].idStaticNPCHitCooldown = 5;
                            Main.projectile[protmp].Center = npc.Center;
                            n.AddBuff(BuffID.Burning, 600);
                            n.AddBuff(BuffID.OnFire, 600);
                            npc.ai[2] = 2;
                            npc.ai[1] = 25;
                            break;
                        }
                    }
                }
            }

            if (npc.ai[1] == 0)
            {
                if (npc.Distance(target.Center) > 400)
                {
                    npc.velocity = (250 * npc.velocity + 6 * Facing * 15) / 255;
                }
                else
                {
                    npc.velocity = (500 * npc.velocity + 6 * Facing * 30) / 505;
                }
            }

            if (npc.direction >= 0)
            {
                npc.rotation = (float)Math.Atan2(Facing.Y, Facing.X);
            }
            else
            {
                npc.rotation = (float)Math.Atan2(Facing.Y, Facing.X) - MathHelper.Pi;
            }


            if (Main.rand.Next(3) == 1)
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

        }

        public override bool PreNPCLoot()
        {
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                int num687 = 0;
                while (num687 < damage / npc.lifeMax * 100.0)
                {
                    int num688 = 25;
                    if (Main.rand.Next(2) == 0)
                    {
                        num688 = 6;
                    }
                    Dust.NewDust(npc.position, npc.width, npc.height, num688, hitDirection, -1f, 0, default, 1f);
                    int num689 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num689].noGravity = true;
                    num687++;
                }
                return;
            }
            for (int num690 = 0; num690 < 50; num690++)
            {
                int num691 = 25;
                if (Main.rand.Next(2) == 0)
                {
                    num691 = 6;
                }
                Dust.NewDust(npc.position, npc.width, npc.height, num691, 2 * hitDirection, -2f, 0, default, 1f);
            }
            for (int num692 = 0; num692 < 50; num692++)
            {
                int num693 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default, 2.5f);
                Dust dust = Main.dust[num693];
                dust.velocity *= 6f;
                Main.dust[num693].noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y = (npc.frame.Y + 28) % 56;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, lightColor, npc.rotation, npc.frame.Size() / 2, npc.scale, SP, 0f);
            return false;
        }


        public int HomeOnTarget()
        {
            int selectedTarget = -1;
            foreach (NPC target in Main.npc)
            {
                if (target.active && !target.friendly && !target.immortal && !target.dontTakeDamage && (target.type != NPCID.SkeletonMerchant || !NPCID.Sets.Skeletons.Contains(target.netID)))
                {
                    if (target.Distance(npc.Center) < 1000)
                    {
                        selectedTarget = target.whoAmI;
                    }

                    if (target.boss && !MABBossChallenge.mabconfig.NPCAttackBoss)
                    {
                        continue;
                    }

                }
            }
            return selectedTarget;
        }
    }
}