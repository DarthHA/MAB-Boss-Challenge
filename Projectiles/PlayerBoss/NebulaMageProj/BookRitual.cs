using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class BookRitual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Book Ritual");
            DisplayName.AddTranslation(GameCulture.Chinese, "法书法阵");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.scale = 0.8f;
            projectile.scale = 5f;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }
    
        public override void AI()
        {
            if (projectile.localAI[1] == 1f)
            {
                projectile.scale *= 0.995f;
                projectile.alpha += 3;
                if (projectile.alpha >= 250)
                {
                    projectile.Kill();
                }
            }
            else
            {
                projectile.scale *= 1.01f;
                projectile.alpha -= 7;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[1] = 1f;
                }
            }

            return;

        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.itemTexture[(int)projectile.ai[0]];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


    }
}