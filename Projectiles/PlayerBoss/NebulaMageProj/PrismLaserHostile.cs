using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System.Linq;
using MABBossChallenge.Buffs;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class PrismLaserHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Last Prism");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终棱镜");
			
        }
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
			projectile.alpha = 0;
            projectile.tileCollide = false;
			projectile.timeLeft = 9999;
			projectile.scale = 0.1f;
        }
		public override void AI()
		{
			projectile.timeLeft = 9999;
			Vector2? vector77 = null;
			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
			{
				projectile.velocity = -Vector2.UnitY;
			}


			if (!Main.projectile[(int)projectile.ai[1]].active)
			{
				projectile.Kill();
				return;
			}
			projectile.friendly = false;
			projectile.hostile = true;
			Projectile owner = Main.projectile[(int)projectile.ai[1]];
			projectile.localAI[0]++;

			//float r = Terraria.Utils.Clamp<float>(projectile.localAI[0] / 300, 0, 1) * MathHelper.Pi / 10;
			float alpha = (float)(projectile.localAI[0] % 20) / 20 * MathHelper.TwoPi + projectile.ai[0] * MathHelper.TwoPi / 6;
			//if (projectile.localAI[0] > 100 && projectile.localAI[0] < 200)
			//{
			//	alpha = (float)(projectile.localAI[0] % 40) / 40 * MathHelper.TwoPi + projectile.ai[0] * MathHelper.TwoPi / 6;
			//}
			//if (projectile.localAI[0] > 200)
			//{
			//	alpha = (float)(projectile.localAI[0] % 80) / 80 * MathHelper.TwoPi + projectile.ai[0] * MathHelper.TwoPi / 6;
			//}
			projectile.Center = owner.Center + Vector2.Normalize(owner.velocity) * 16f;
			projectile.position += (owner.velocity.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * (float)Math.Sin(alpha) * 4 * projectile.scale;
			//projectile.velocity = (owner.velocity.ToRotation() + (float)Math.Sin(alpha) * r).ToRotationVector2();
			projectile.velocity = Vector2.Normalize(owner.velocity);
			if (owner.timeLeft <= 30)
			{
				projectile.scale -= 0.05f;
                if (projectile.scale <= 0.1f)
                {
					projectile.Kill();
                }
			}
			else
			{
				projectile.scale += 0.05f;
				if (projectile.scale > 1.4f)
				{
					projectile.scale = 1.4f;
				}
			}

			vector77 = new Vector2?(owner.Center);

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
			{
				projectile.velocity = -Vector2.UnitY;
			}
			projectile.rotation = projectile.velocity.ToRotation() - MathHelper.Pi / 2;
			Vector2 samplingPoint = projectile.Center;
			if (vector77 != null)
			{
				samplingPoint = vector77.Value;
			}


			float[] array5 = new float[2];
			Collision.LaserScan(samplingPoint, projectile.velocity, 1, 2400f, array5);
			float num808 = array5.Average();
			//float amount = 0.75f;

			//projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num808, amount);
			projectile.localAI[1] = 2400;
			if (Math.Abs(projectile.localAI[1] - num808) < 100f)
			{
				float prismHue = projectile.GetPrismHue(projectile.ai[0]);
				Color color = Main.hslToRgb(prismHue, 1f, 0.5f);
				color.A = 0;
				Vector2 vector86 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14.5f * projectile.scale);
				float x2 = Main.rgbToHsl(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB)).X;
				for (int num830 = 0; num830 < 2; num830++)
				{
					float num831 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
					float num832 = (float)Main.rand.NextDouble() * 0.8f + 1f;
					Vector2 vector87 = new Vector2((float)Math.Cos(num831) * num832, (float)Math.Sin(num831) * num832);
					int num833 = Dust.NewDust(vector86, 0, 0, 267, vector87.X, vector87.Y, 0, default, 1f);
					Main.dust[num833].color = color;
					Main.dust[num833].scale = 1.2f;
					if (projectile.scale > 1f)
					{
						Main.dust[num833].velocity *= projectile.scale;
						Main.dust[num833] = Main.dust[num833];
						Main.dust[num833].scale *= projectile.scale;
					}
					Main.dust[num833].noGravity = true;
					if (projectile.scale != 1.4f)
					{
						Dust dust100 = Dust.CloneDust(num833);
						dust100.color = Color.White;
						dust100.scale /= 2f;
					}
					float hue = (x2 + Main.rand.NextFloat() * 0.4f) % 1f;
					Main.dust[num833].color = Color.Lerp(color, Main.hslToRgb(hue, 1f, 0.75f), projectile.scale / 1.4f);
				}
				if (Main.rand.Next(5) == 0)
				{
					Vector2 value33 = projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
					int num834 = Dust.NewDust(vector86 + value33 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
					Main.dust[num834].velocity *= 0.5f;
					Main.dust[num834].velocity.Y = -Math.Abs(Main.dust[num834].velocity.Y);
				}
				DelegateMethods.v3_1 = color.ToVector3() * 0.3f;
				float value34 = 0.1f * (float)Math.Sin(Main.GlobalTime * 20f);
				Vector2 vector88 = new Vector2(projectile.velocity.Length() * projectile.localAI[1], projectile.width * projectile.scale);
				float num835 = projectile.velocity.ToRotation();
				((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(projectile.position + new Vector2(vector88.X * 0.5f, 0f).RotatedBy(num835, default), new Color(0.5f, 0.1f * Math.Sign(value34) + 0.5f, 0f, 1f) * Math.Abs(value34), vector88, RippleShape.Square, num835);
				Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
				return;
			}
		}
        public override bool CanDamage()
        {
			Projectile owner = Main.projectile[(int)projectile.ai[1]];
            if (owner.type == ModContent.ProjectileType<LastPrismHostile2>())
            {
				return projectile.scale >= 0.7f;
            }
			return projectile.scale >= 1.4f;
        }
        public override void CutTiles()
        {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CutTiles));
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float point = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref point);

		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(ModContent.BuffType<JusticeJudegmentBuff>(), (Main.rand.Next(3) + 3) * 60);
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			target.AddBuff(ModContent.BuffType<JusticeJudegmentBuff>(), 120);
		}
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			damage *= 30;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
			{
				return false;
			}
			Texture2D tex = Main.projectileTexture[projectile.type];
			float num232 = projectile.localAI[1];
			float prismHue = GetPrismHue2(projectile.ai[0]);
            Color value26 = Main.hslToRgb(prismHue, 1f, 0.5f);
			value26.A = 0;
			Vector2 value27 = projectile.Center.Floor();
			value27 += projectile.velocity * projectile.scale * 10.5f;
			num232 -= projectile.scale * 14.5f * projectile.scale;
			Vector2 vector42 = new Vector2(projectile.scale);
			DelegateMethods.f_1 = 1f;
			DelegateMethods.c_1 = value26 * 0.75f * projectile.Opacity;
			Terraria.Utils.DrawLaser(spriteBatch, tex, value27 - Main.screenPosition, value27 + projectile.velocity * num232 - Main.screenPosition, vector42, new Terraria.Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
			DelegateMethods.c_1 = new Color(255, 255, 255, 127) * 0.75f * projectile.Opacity;
			Terraria.Utils.DrawLaser(spriteBatch, tex, value27 - Main.screenPosition, value27 + projectile.velocity * num232 - Main.screenPosition, vector42 / 2f, new Terraria.Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));

			return false;
		}


		private float GetPrismHue2(float indexing)
		{
			return (int)indexing / 6f;
		}
	}
}