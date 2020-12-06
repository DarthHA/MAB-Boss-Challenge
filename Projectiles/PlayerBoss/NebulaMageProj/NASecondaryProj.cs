
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class NASecondaryProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_618";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Arcanum");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云奥秘");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            cooldownSlot = 1;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            float x4 = 0.15f;
            float y3 = 0.15f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 420)
            {
                bool flag61 = true;
                int num1028 = (int)projectile.ai[1];
                if (Main.projectile[num1028].active && Main.projectile[num1028].type == ModContent.ProjectileType<NebulaArcanumHostile>())
                {
                    if (Main.projectile[num1028].oldPosition != Vector2.Zero)       //oldpos[1]
                    {
                        projectile.position += Main.projectile[num1028].position - Main.projectile[num1028].oldPosition;
                    }
                    if (projectile.Center.HasNaNs())
                    {
                        projectile.Kill();
                        return;
                    }
                }
                else
                {
                    projectile.ai[0] = 420;
                    flag61 = false;
                    projectile.Kill();
                }
                if (flag61)
                {
                    projectile.velocity += new Vector2(Math.Sign(Main.projectile[num1028].Center.X - projectile.Center.X), Math.Sign(Main.projectile[num1028].Center.Y - projectile.Center.Y)) * new Vector2(x4, y3);
                    if (projectile.velocity.Length() > 6f)
                    {
                        projectile.velocity *= 6f / projectile.velocity.Length();
                    }
                }

                if (Main.rand.Next(2) == 0)
                {
                    int num1029 = Dust.NewDust(projectile.Center, 8, 8, 86, 0f, 0f, 0, default, 1f);
                    Main.dust[num1029].position = projectile.Center;
                    Main.dust[num1029].velocity = projectile.velocity;
                    Main.dust[num1029].noGravity = true;
                    Main.dust[num1029].scale = 1.5f;
                    if (flag61)
                    {
                        Main.dust[num1029].customData = Main.projectile[(int)projectile.ai[1]];
                    }
                }
                projectile.alpha = 255;
                return;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.ai[0] = 86f;
            for (int num113 = 0; num113 < 10; num113++)
            {
                int num114 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)projectile.ai[0], projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                if (Main.rand.Next(3) == 0)
                {
                    Main.dust[num114].fadeIn = 0.75f + Main.rand.Next(-10, 11) * 0.01f;
                    Main.dust[num114].scale = 0.25f + Main.rand.Next(-10, 11) * 0.005f;
                    dust = Main.dust[num114];
                    Dust dust15 = dust;
                    dust15.type++;
                }
                else
                {
                    Main.dust[num114].scale = 1f + Main.rand.Next(-10, 11) * 0.01f;
                }
                Main.dust[num114].noGravity = true;
                dust = Main.dust[num114];
                dust.velocity *= 1.25f;
                dust = Main.dust[num114];
                dust.velocity -= projectile.oldVelocity / 10f;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 vector60 = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D31 = Main.projectileTexture[projectile.type];
            Color alpha4 = projectile.GetAlpha(lightColor);
            Vector2 origin8 = new Vector2(texture2D31.Width, texture2D31.Height) / 2f;
            spriteBatch.Draw(texture2D31, vector60, null, alpha4, projectile.rotation, origin8, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}