using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class CNimbusHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Nimbus");
            DisplayName.AddTranslation(GameCulture.Chinese, "血云");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 28) % 112;
            }

            Vector2 TargetPos = new Vector2(projectile.ai[0], projectile.ai[1]);
            projectile.velocity = Vector2.Normalize(TargetPos - projectile.Center) * 15;
            if (projectile.Distance(TargetPos) < 15)
            {
                projectile.Kill();
            }
            projectile.rotation += projectile.velocity.X * 0.02f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, projectile.frame, 28, 28);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, lightColor * projectile.Opacity, projectile.rotation, Frame.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CNimbusHostile2>(), projectile.damage, 0);
        }


    }
}