using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class NASphere : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Arcanum");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云奥秘");
            Main.projFrames[projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 9;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, 24 * projectile.frame, 24, 24);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.White, projectile.rotation, Frame.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }


    }
}