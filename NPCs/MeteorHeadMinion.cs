using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    public class MeteorHeadMinion : ModNPC
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("陨星头");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.damage = 70;
            npc.defense = 5;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit3;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.aiStyle = -1;
            if (Main.hardMode && MABWorld.DownedMeteorPlayer)
            {
                npc.lifeMax = 4000;
                npc.life = 4000;
                npc.damage = 120;
                npc.defense = 10;
                if (NPC.downedMoonlord)
                {
                    npc.lifeMax = 10000;
                    npc.life = 10000;
                    npc.damage = 240;
                    npc.defense = 25;
                }
            }
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
            npc.TargetClosest();
            if (!Main.player[npc.target].active || Main.player[npc.target].dead)
            {
                npc.velocity.Y -= 0.1f;
                return;
            }

            Player player = Main.player[npc.target];
            npc.direction = Math.Sign(player.Center.X - npc.Center.X);
            Vector2 Facing = Vector2.Normalize(player.Center - npc.Center);
            if (Main.hardMode && MABWorld.DownedMeteorPlayer)
            {
                if (Main.rand.Next(120) == 1)
                {
                    float r = MathHelper.TwoPi * Main.rand.NextFloat();
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(npc.Center, r.ToRotationVector2() * 10, ProjectileID.GreekFire1, npc.damage / 5, 0, default);
                    }
                }
            }
            if (npc.Distance(player.Center) > 400)
            {
                npc.velocity = (250 * npc.velocity + 6 * Facing * 15) / 255;
            }
            else
            {
                npc.velocity = (500 * npc.velocity + 6 * Facing * 30) / 505;
            }
            if (npc.direction >= 0)
            {
                npc.rotation = (float)Math.Atan2(Facing.Y, Facing.X);
            }
            else
            {
                npc.rotation = (float)Math.Atan2(Facing.Y, Facing.X) - MathHelper.Pi;
            }

            foreach (NPC n in Main.npc)
            {
                if (n.active && n.type == npc.type && n.whoAmI != npc.whoAmI)
                {
                    if (n.Distance(npc.Center) < 30)
                    {
                        if (n.Center == npc.Center)
                        {
                            return;
                        }
                        Vector2 AwayVel = Vector2.Normalize(npc.Center - n.Center);
                        npc.velocity = AwayVel * 2;
                    }
                }
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
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Burning, 120);

        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Burning, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, lightColor, npc.rotation, npc.frame.Size() / 2, npc.scale, SP, 0f);
            return false;
        }
    }
}