using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class HolyArrowHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_91";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Arrow");
            DisplayName.AddTranslation(GameCulture.Chinese, "圣箭");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.ignoreWater = true;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 100;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);

            if (Main.rand.Next(2) == 0)
            {
                int num97;
                if (Main.rand.Next(2) == 0)
                {
                    num97 = 15;
                }
                else
                {
                    num97 = 58;
                }
                int num98 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num97, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                Dust dust3 = Main.dust[num98];
                dust3.velocity *= 0.25f;
            }

            projectile.velocity.Y = 23;
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.Pi / 2;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            int num3;
            for (int num480 = 0; num480 < 10; num480 = num3 + 1)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.2f);
                num3 = num480;
            }
            for (int num481 = 0; num481 < 3; num481 = num3 + 1)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                num3 = num481;
            }

            if (Main.rand.Next(3) >= 1)
            {
                float x = projectile.position.X + Main.rand.Next(-400, 400);
                float y = projectile.position.Y - Main.rand.Next(1800, 2400);
                Vector2 vector21 = new Vector2(x, y);
                float num484 = projectile.Center.X - vector21.X;
                float num485 = projectile.Center.Y - vector21.Y;
                float num487 = (float)Math.Sqrt(num484 * num484 + num485 * num485);
                num487 = 22 / num487;
                num484 *= num487;
                num485 *= num487;
                int num489 = Projectile.NewProjectile(x, y, num484, num485, ProjectileID.HallowStar, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[num489].ai[1] = projectile.position.Y;
                Main.projectile[num489].ai[0] = 1f;
                Main.projectile[num489].friendly = false;
                Main.projectile[num489].tileCollide = false;
                Main.projectile[num489].hostile = true;
                Main.projectile[num489].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex1 = Main.projectileTexture[projectile.type];


            for (int i = 1; i < 4; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.6f;
                color27 *= (float)(4 - i) / 4;
                Vector2 value4 = projectile.position - projectile.velocity * i;

                Main.spriteBatch.Draw(Tex1, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, color27, projectile.rotation, Tex1.Size() * 0.5f, 1.0f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, Color.White, projectile.rotation, Tex1.Size() * 0.5f, 1.0f, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}