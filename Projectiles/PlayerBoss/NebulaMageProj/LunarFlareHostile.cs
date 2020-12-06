using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net.Util;
using System.Security.Policy;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class LunarFlareHostile : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_645";
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "月曜");
			Main.projFrames[projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
			projectile.scale = 1.6f;
			cooldownSlot = 1;
        }
		public override void AI()
		{
			
			if (projectile.position.HasNaNs())
			{
				projectile.Kill();
				return;
			}
			Dust dust36 = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 0, default, 1f)];
			dust36.position = projectile.Center;
			dust36.velocity = Vector2.Zero;
			dust36.noGravity = true;

			if (projectile.ai[1] == -1f)
			{
				Main.LocalPlayer.GetModPlayer<ShakeScreenPlayer>().shake = true;
				projectile.ai[0]++;
				projectile.velocity = Vector2.Zero;
				projectile.tileCollide = false;
				projectile.penetrate = -1;
				projectile.position = projectile.Center;
				projectile.width = (projectile.height = 210);
				projectile.Center = projectile.position;
				projectile.alpha -= 10;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
				projectile.frameCounter++;
				if (projectile.frameCounter >= projectile.MaxUpdates * 3)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
				}
				if (projectile.ai[0] >= projectile.MaxUpdates * 3 * Main.projFrames[projectile.type]) 
				{
					projectile.Kill();
				}
				//Main.NewText(projectile.ai[0]+" "+ Main.projFrames[projectile.type] * projectile.MaxUpdates * 3);
				return;
			}
			projectile.alpha = 255;

			if (projectile.Center.Y > projectile.localAI[0])
			{
				projectile.ai[0] = 0f;
				projectile.ai[1] = -1f;
				return;
			}
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.MoonLeech, 3600);
            if (projectile.ai[1] != -1)
            {
				projectile.ai[0] = 0f;
				projectile.ai[1] = -1f;
			}
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.MoonLeech, 3600);
			if (projectile.ai[1] != -1)
			{
				projectile.ai[0] = 0f;
				projectile.ai[1] = -1f;
			}
		}
		
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage *= 10;
		}

		public override void Kill(int timeLeft)
        {
			for (int num59 = 0; num59 < 4; num59++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
			}
			for (int num60 = 0; num60 < 4; num60++)
			{
				int num61 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 0, default, 2.5f);
				Main.dust[num61].noGravity = true;
				Dust dust = Main.dust[num61];
				dust.velocity *= 3f;

				num61 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 100, default, 1.5f);
				dust = Main.dust[num61];
				dust.velocity *= 2f;
				Main.dust[num61].noGravity = true;

			}
			for (int num62 = 0; num62 < 1; num62++)
			{
				int num63 = Gore.NewGore(projectile.position + new Vector2((float)(projectile.width * Main.rand.Next(100)) / 100f, projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
				Gore gore = Main.gore[num63];
				gore.velocity *= 0.3f;
				Gore gore13 = Main.gore[num63];
				gore13.velocity.X += Main.rand.Next(-10, 11) * 0.05f;
				Gore gore14 = Main.gore[num63];
				gore14.velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
            
            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
			return false;
		}
	}
}