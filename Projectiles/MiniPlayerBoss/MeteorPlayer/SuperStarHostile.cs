using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer
{
    public class SuperStarHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Star");
			DisplayName.AddTranslation(GameCulture.Chinese, "³¬¼¶ÐÇÐÇ");
		}
        public override void SetDefaults()
        {
			projectile.width = 24;
			projectile.height = 24;
			//projectile.aiStyle = 151;
			projectile.alpha = 255;
			projectile.tileCollide = false;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
		}
        public override void AI()
        {
			projectile.ai[0]++;
			projectile.localAI[0] = (projectile.localAI[0] + 1) % 60;
			projectile.alpha -= 10;
			if (projectile.alpha < 100)
			{
				projectile.alpha = 100;
			}
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				Main.PlaySound(SoundID.Item9, projectile.position);
			}
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.005f * projectile.direction;
			if (projectile.ai[0] % 40 == 19)
            {
				for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
				{
					int protmp = Projectile.NewProjectile(projectile.Center + i.ToRotationVector2(), i.ToRotationVector2(), ModContent.ProjectileType<SuperStarSlashHostile2>(), projectile.damage / 3 * 2, 0, default, default, 1);
					Main.projectile[protmp].localAI[0] = projectile.Center.X;
					Main.projectile[protmp].localAI[1] = projectile.Center.Y;
				}
            }

			if (projectile.ai[0] % 40 == 39)
			{
				for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 3)
				{
					int protmp = Projectile.NewProjectile(projectile.Center + i.ToRotationVector2(), i.ToRotationVector2(), ModContent.ProjectileType<SuperStarSlashHostile2>(), projectile.damage / 3 * 2, 0, default, default, -1);
					Main.projectile[protmp].localAI[0] = projectile.Center.X;
					Main.projectile[protmp].localAI[1] = projectile.Center.Y;
				}
			}
			Vector2 value = new Vector2(Main.screenWidth, Main.screenHeight);
			if (projectile.Hitbox.Intersects(Terraria.Utils.CenteredRectangle(Main.screenPosition + value / 2f, value + new Vector2(400f))) && Main.rand.Next(6) == 0)
			{
				Gore.NewGore(projectile.position, projectile.velocity * 0.2f, Terraria.Utils.SelectRandom<int>(Main.rand, new int[]
				{
					16,
					17,
					17,
					17
				}), 1f);
			}
			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.Next(8) == 0)
				{
					int num2 = 228;
					Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, num2, 0f, 0f, 127, default, 1f);
					dust.velocity *= 0.25f;
					dust.scale = 1.3f;
					dust.noGravity = true;
					dust.velocity += projectile.velocity.RotatedBy(0.3926991f * (1f - 2 * i), default) * 0.2f;
				}
			}

		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D value23 = Main.projectileTexture[projectile.type];
			Rectangle rectangle9 = new Rectangle(0, 0, value23.Width, value23.Height);
			Vector2 origin7 = rectangle9.Size() / 2f;
			Color alpha3 = Color.White * projectile.Opacity;
			Texture2D value24 = mod.GetTexture("Images/StarTrail");
			Rectangle rectangle10 = new Rectangle(0, 0, value24.Width, value24.Height);
			Vector2 origin8 = new Vector2(rectangle10.Width / 2f, 10f);
			//Color.Cyan * 0.5f;
			Vector2 value25 = new Vector2(0f, projectile.gfxOffY);
			Vector2 vector33 = new Vector2(0f, -10f);
			float num202 = projectile.localAI[0] / 60;
			Vector2 value26 = projectile.Center + projectile.velocity;
			Color color43 = Color.Orange * 0.2f;
			Color value27 = Color.Gold * 0.5f;
			value27.A = 50;
			float num203 = -0.2f;

			Color color44 = color43;
			color44.A = 0;
			Texture2D texture4 = value24;
			Vector2 value28 = value26 - Main.screenPosition + value25;
			Vector2 spinningpoint5 = vector33;
			double radians5 = MathHelper.TwoPi * num202;

			spriteBatch.Draw(texture4, value28 + spinningpoint5.RotatedBy(radians5), new Rectangle?(rectangle10), color44, projectile.velocity.ToRotation() + MathHelper.Pi / 2, origin8, 1.5f + num203, SpriteEffects.None, 0);
			Texture2D texture5 = value24;
			Vector2 value29 = value26 - Main.screenPosition + value25;
			Vector2 spinningpoint6 = vector33;
			double radians6 = MathHelper.TwoPi * num202 + 2.09439516f;
			spriteBatch.Draw(texture5, value29 + spinningpoint6.RotatedBy(radians6), new Rectangle?(rectangle10), color44, projectile.velocity.ToRotation() + MathHelper.Pi / 2, origin8, 1.1f + num203, SpriteEffects.None, 0);
			Texture2D texture6 = value24;
			Vector2 value30 = value26 - Main.screenPosition + value25;
			Vector2 spinningpoint7 = vector33;
			double radians7 = MathHelper.TwoPi * num202 + 4.18879032f;
			spriteBatch.Draw(texture6, value30 + spinningpoint7.RotatedBy(radians7), new Microsoft.Xna.Framework.Rectangle?(rectangle10), color44, projectile.velocity.ToRotation() + 1.57079637f, origin8, 1.3f + num203, SpriteEffects.None, 0);
			Vector2 value31 = projectile.Center - projectile.velocity * 0.5f;
			for (float num204 = 0f; num204 < 1f; num204 += 0.5f)
			{
				float num205 = num202 % 0.5f / 0.5f;
				num205 = (num205 + num204) % 1f;
				float num206 = num205 * 2f;
				if (num206 > 1f)
				{
					num206 = 2f - num206;
				}
				spriteBatch.Draw(value24, value31 - Main.screenPosition + value25, new Microsoft.Xna.Framework.Rectangle?(rectangle10), value27 * num206, projectile.velocity.ToRotation() + MathHelper.Pi / 2, origin8, 0.3f + num205 * 0.5f, SpriteEffects.None, 0);
			}
			spriteBatch.Draw(value23, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle9), alpha3, projectile.rotation, origin7, projectile.scale + 0.1f, SpriteEffects.None, 0);

			return false;
		}
        public override Color? GetAlpha(Color lightColor)
        {
			return new Color(255, 255, 255, 0) * projectile.Opacity;
		}
        public override void Kill(int timeLeft)
        {
			Main.PlaySound(SoundID.Item10, projectile.position);
			int num6;
			for (int num542 = 0; num542 < 7; num542 = num6 + 1)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 0.8f);
				num6 = num542;
			}
			for (float num543 = 0f; num543 < 1f; num543 += 0.125f)
			{
				Dust.NewDustPerfect(projectile.Center, 278, new Vector2?(Vector2.UnitY.RotatedBy(num543 * MathHelper.TwoPi + Main.rand.NextFloat() * 0.5f, default) * (4f + Main.rand.NextFloat() * 4f)), 150, Color.CornflowerBlue, 1f).noGravity = true;
			}
			for (float num544 = 0f; num544 < 1f; num544 += 0.25f)
			{
				Dust.NewDustPerfect(projectile.Center, 278, new Vector2?(Vector2.UnitY.RotatedBy(num544 * MathHelper.TwoPi + Main.rand.NextFloat() * 0.5f, default) * (2f + Main.rand.NextFloat() * 3f)), 150, Color.Gold, 1f).noGravity = true;
			}
			Vector2 value20 = new Vector2(Main.screenWidth, Main.screenHeight);
			bool flag5 = projectile.Hitbox.Intersects(Terraria.Utils.CenteredRectangle(Main.screenPosition + value20 / 2f, value20 + new Vector2(400f)));
			if (flag5)
			{
				for (int num545 = 0; num545 < 7; num545 = num6 + 1)
				{
					Gore.NewGore(projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * projectile.velocity.Length(), Terraria.Utils.SelectRandom<int>(Main.rand, new int[]
					{
								16,
								17,
								17,
								17,
								17,
								17,
								17,
								17
					}), 1f);
					num6 = num545;
				}
			}
			SummonSuperStarSlash(projectile.Center);
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.Dazed, 180);
			target.AddBuff(BuffID.BrokenArmor, 600);
			SummonSuperStarSlash(projectile.Center);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.Dazed, 180);
			target.AddBuff(BuffID.BrokenArmor, 600);
			SummonSuperStarSlash(projectile.Center);
		}
        private void SummonSuperStarSlash(Vector2 target)
		{
			Vector2 vector = Main.rand.NextVector2CircularEdge(200f, 200f);
			if (vector.Y < 0f)
			{
				vector.Y *= -1f;
			}
			vector.Y += 100f;
			Vector2 vector2 = vector.SafeNormalize(Vector2.UnitY) * 6f;
			Projectile.NewProjectile(target - vector2 * 20f, vector2, ModContent.ProjectileType<SuperStarSlashHostile>(), projectile.damage / 2, 0f, projectile.owner, 0f, target.Y);
		}

	}


}