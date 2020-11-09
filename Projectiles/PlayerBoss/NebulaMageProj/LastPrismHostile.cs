using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using MABBossChallenge.NPCs.PlayerBoss;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class LastPrismHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Last Prism");
            DisplayName.AddTranslation(GameCulture.Chinese, "◊Ó÷’¿‚æµ");
            Main.projFrames[projectile.type] = 5;
            
        }
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 18;
            //projectile.aiStyle = 75;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.timeLeft = 830;
			projectile.alpha = 250;
        }
        public override void AI()
		{
			if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<NebulaMageBoss>())
			{
				projectile.Kill();
				return;
			}
			projectile.friendly = false;
			projectile.hostile = true;
			NPC owner = Main.npc[(int)projectile.ai[0]];
			projectile.Center = owner.Center;
			projectile.localAI[0]++;
			
			projectile.frameCounter++;
			if (projectile.frameCounter >= 2)   //4,1
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= 5)
				{
					projectile.frame = 0;
				}
			}
			projectile.alpha -= 2;
            if (projectile.alpha < 0)
            {
				projectile.alpha = 0;
            }
            float r;
            if (projectile.localAI[0] < 120)
            {
				r = projectile.localAI[0] / 2;
            }
            else
            {
				r = 60;
            }
			if (projectile.localAI[0] % 25 == 20)
			{
				if (projectile.localAI[0] < 80)
				{
					Main.PlaySound(SoundID.Item13, projectile.position);
				}
                else
                {
					Main.PlaySound(SoundID.Item15, projectile.position);
				}
			}

			projectile.rotation = projectile.localAI[0] / 400 * MathHelper.TwoPi + projectile.ai[1];
			projectile.velocity = projectile.rotation.ToRotationVector2();
			projectile.Center = owner.Center + projectile.velocity * r;

			if (projectile.localAI[0] == 110f)
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.localAI[0] < 120 && projectile.localAI[0] > 40) 
			{
				Terraria.Utils.DrawLine(spriteBatch, projectile.Center, projectile.Center + Vector2.Normalize(projectile.velocity) * 2400, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha), new Color(255 - Main.DiscoR, 255 - Main.DiscoG, 255 - Main.DiscoB, projectile.alpha), 5);
			}
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle Frame = new Rectangle(0, tex.Height / Main.projFrames[projectile.type] * projectile.frame, tex.Width, tex.Height / Main.projFrames[projectile.type]);
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, Frame.Size() / 2, projectile.scale, SpriteEffects.None, 0);
			return false;
		}
    }
}