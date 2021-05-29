using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpSphere2 : ModProjectile
    {
        public Vector2 CenterPos = Vector2.Zero;
        public float R = 200;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Sphere");
            DisplayName.AddTranslation(GameCulture.Chinese, "Ô¾Ç¨ÄÜÁ¿µ¯");
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.scale = 1.0f;
            projectile.width = 35;
            projectile.height = 35;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item20, projectile.Center);
            }


            projectile.localAI[0]++;
            if (projectile.localAI[0] <= 240)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                projectile.Center = CenterPos + (projectile.localAI[0] * MathHelper.TwoPi / 240 * projectile.localAI[1] - MathHelper.Pi / 2).ToRotationVector2() * R;
            }
            else
            {
                projectile.alpha += 20;
                if (projectile.alpha > 250)
                {
                    projectile.Kill();
                    return;
                }
            }
            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
        public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] <= 20 || projectile.localAI[0] > 240)
            {
                return false;
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color alpha = Color.White * projectile.Opacity;

            Rectangle Frame = new Rectangle(0, 44 * projectile.frame, 38, 44);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), Frame, alpha, projectile.rotation, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public static void SummonSphere(Vector2 Center, float r, int dmg, int dir)
        {
            int protmp = Projectile.NewProjectile(Center + new Vector2(0, -r), Vector2.Zero, ModContent.ProjectileType<WarpSphere2>(), dmg, 0, default);
            (Main.projectile[protmp].modProjectile as WarpSphere2).CenterPos = Center;
            (Main.projectile[protmp].modProjectile as WarpSphere2).R = r;
            Main.projectile[protmp].localAI[1] = dir;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}