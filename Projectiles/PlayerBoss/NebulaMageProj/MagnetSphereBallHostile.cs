using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MentalAIBoost.Projectiles.DestroyerEXProj
{
    class MagnetSphereBallHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_254";
        public List<int> others = new List<int>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnet Sphere");
            DisplayName.AddTranslation(GameCulture.Chinese, "磁球");
            Main.projFrames[projectile.type] = 5;
            
        }
        public override void SetDefaults()
        {
            projectile.scale = 1.0f;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.velocity *= 0.96f;
            if (projectile.timeLeft < 60)
            {
                projectile.Opacity = (float)projectile.timeLeft / 60;
            }

            if (projectile.ai[1] == 0)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] > 60 + Main.rand.Next(100))
                {
                    projectile.ai[0] = 0;

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<MagnetSphereBallHostile>() && proj.whoAmI != projectile.whoAmI)
                        {
                            others.Add(proj.whoAmI);
                        }
                    }
                    if (others.Count > 0)
                    {
                        projectile.ai[1] = others[Main.rand.Next(others.Count)];
                    }
                }
            }
            else
            {
                Projectile proj = Main.projectile[(int)projectile.ai[1]];
                int dustamount = (int)((projectile.Center - proj.Center).Length() / 2);
                for (int i = 0; i < dustamount; i++)
                {
                    Vector2 DustPos = projectile.Center + Vector2.Normalize(proj.Center - projectile.Center) * (projectile.Center - proj.Center).Length() * Main.rand.NextFloat();
                    Dust dust = Dust.NewDustDirect(DustPos, 1, 1, 160, default, default, default, default, 3);
                    dust.position = DustPos;
                    dust.scale = Main.rand.Next(70, 110) * 0.013f;
                    dust.velocity *= 0.2f;
                }
                projectile.ai[1] = 0;
            }

        }
        public override bool CanDamage()
        {
            return projectile.velocity.Length() < 2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color alpha = Color.White * projectile.Opacity;
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }
            Rectangle Frame = new Rectangle(0, 44 * projectile.frame, 38, 44);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), Frame, alpha, projectile.rotation, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            return false;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 240);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 240);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.ai[1] == 0)
            {
                return null;
            }
            else
            {
                float point = 0;
                Projectile proj = Main.projectile[(int)projectile.ai[1]];
                return projHitbox.Intersects(targetHitbox) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, proj.Center, 12, ref point);
            }

        }
    }
}