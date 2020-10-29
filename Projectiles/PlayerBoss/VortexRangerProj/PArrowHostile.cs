using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class PArrowHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_631";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Arrow");
            DisplayName.AddTranslation(GameCulture.Chinese, "幻象矢");
        }
        public override void SetDefaults()
        {
            projectile.scale = 2;
            projectile.width = 15;
            projectile.height = 15;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 50;

        }
        public override void AI()
        {
            Player target = Main.player[projectile.owner];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.Pi / 2;
            return;

        }

        public override void Kill(int timeLeft)
        {
            int num135 = Main.rand.Next(5, 10);
            int num3;
            for (int num136 = 0; num136 < num135; num136 = num3 + 1)
            {
                int num137 = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[num137];
                dust.velocity *= 1.6f;
                Dust dust18 = Main.dust[num137];
                dust18.velocity.Y = dust18.velocity.Y - 1f;
                dust = Main.dust[num137];
                dust.position -= Vector2.One * 4f;
                Main.dust[num137].position = Vector2.Lerp(Main.dust[num137].position, projectile.Center, 0.5f);
                Main.dust[num137].noGravity = true;
                num3 = num136;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

    }
}