using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class LastPrismHostile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Last Prism");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终棱镜");
            Main.projFrames[projectile.type] = 5;
            
        }
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 18;
            //projectile.aiStyle = 75;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.timeLeft = 120;
			projectile.alpha = 250;
        }
        public override void AI()
		{
            /*
			if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<NebulaMageBoss>())
			{
				projectile.Kill();
				return;
			}
			*/
            if (projectile.velocity == Vector2.Zero)
            {
				projectile.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 0.01f;
            }
			projectile.localAI[0]++;
			
			projectile.frameCounter++;
			if (projectile.frameCounter >= 2)   //4,1
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 5;
			}
			projectile.alpha -= 20;
            if (projectile.alpha < 0)
            {
				projectile.alpha = 0;
            }

			if (projectile.localAI[0] % 25 == 20)
			{
				if (projectile.localAI[0] < 60)
				{
					Main.PlaySound(SoundID.Item13, projectile.position);
				}
                else
                {
					Main.PlaySound(SoundID.Item15, projectile.position);
				}
			}


			projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.localAI[0] < 60)
            {
				Vector2 FirePos = projectile.Center + Vector2.Normalize(projectile.velocity) * 16;
				float num1 = 0.33f;
				for (int i = 0; i < 9; i++)
				{
					if (Main.rand.NextFloat() >= num1)
					{
						float f = Main.rand.NextFloat() * MathHelper.TwoPi;
						float num2 = Main.rand.NextFloat();
						Dust dust = Dust.NewDustPerfect(FirePos + f.ToRotationVector2() * (110 + 200 * num2), MyDustId.WhiteShortFx, (f - MathHelper.Pi).ToRotationVector2() * (14 + 8 * num2), 0, default, 2f);
						dust.scale = 0.9f;
						dust.fadeIn = 1.15f + num2 * 0.3f;
						dust.color = Main.DiscoColor;
						dust.noGravity = true;
						dust.noLight = false;
					}
				}

			}

			if (projectile.localAI[0] == 60f)
			{
				Vector2 vector19 = Vector2.Normalize(projectile.velocity);
				if (float.IsNaN(vector19.X) || float.IsNaN(vector19.Y))
				{
					vector19 = -Vector2.UnitY;
				}
				for (int l = 0; l < 6; l++)
				{
					Projectile.NewProjectile(projectile.Center, vector19, ModContent.ProjectileType<PrismLaserHostile>(), projectile.damage, projectile.knockBack, default, l, projectile.whoAmI);
				}
			}

		}

        public override void Kill(int timeLeft)
        {
            foreach(Projectile proj in Main.projectile)
            {
				if(proj.active && proj.type == ModContent.ProjectileType<PrismLaserHostile>())
                {
                    if (proj.ai[1] == projectile.whoAmI)
                    {
						proj.Kill();
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            if (projectile.localAI[0] < 70)
            {
				Terraria.Utils.DrawLine(spriteBatch, projectile.Center, projectile.Center + Vector2.Normalize(projectile.velocity) * 2400, new Color(Main.DiscoR,Main.DiscoG, Main.DiscoB, projectile.alpha), new Color(255 - Main.DiscoR, 255 - Main.DiscoG, 255 - Main.DiscoB, projectile.alpha), 5);
            }
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle Frame = new Rectangle(0, tex.Height / Main.projFrames[projectile.type] * projectile.frame, tex.Width, tex.Height / Main.projFrames[projectile.type]);
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, Frame.Size() / 2, projectile.scale, SpriteEffects.None, 0);
			return false;
		}
    }
}