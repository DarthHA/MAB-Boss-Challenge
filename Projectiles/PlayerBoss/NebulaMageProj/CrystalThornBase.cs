using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class CrystalThornBase : ModProjectile
    {
		private int Length = 25;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Crystal Shard");
            DisplayName.AddTranslation(GameCulture.Chinese, "邪恶水晶碎片");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.light = 0.2f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White) * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

		public override void AI()
		{
			if (projectile.velocity != Vector2.Zero)
			{
				projectile.velocity = Vector2.Normalize(projectile.velocity) * 0.01f;
			}
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
			if (projectile.ai[0] == 0f)
			{
				projectile.alpha -= 100;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.ai[0] = 1f;
					if (projectile.ai[1] == 0f)
					{
						projectile.ai[1]++;
						//projectile.position += projectile.velocity;
					}


					int type = ModContent.ProjectileType<CrystalThornBase>();
					if (projectile.ai[1] >= Length)
					{
						type = ModContent.ProjectileType<CrystalThornTip>();
					}
					Projectile.NewProjectile(projectile.Center + Vector2.Normalize(projectile.velocity) * 32 * projectile.scale, projectile.velocity, type, projectile.damage, projectile.knockBack, projectile.owner, 0f, projectile.ai[1] + 1f);
					return;

				}
			}
			else
			{
				if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
				{

					for (int num56 = 0; num56 < 8; num56++)
					{
						int num57 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(68, 71), projectile.velocity.X * 0.025f, projectile.velocity.Y * 0.025f, 200, default(Color), 1.3f);
						Main.dust[num57].noGravity = true;
						Dust dust3 = Main.dust[num57];
						dust3.velocity *= 0.5f;
					}

				}
				projectile.alpha += 5;

				if (projectile.alpha >= 255)
				{
					projectile.Kill();
					return;
				}
			}
		}

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			damage *= 10;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha);
		}
	}
}