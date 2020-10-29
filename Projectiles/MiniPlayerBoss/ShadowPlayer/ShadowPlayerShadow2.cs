using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
/// <summary>
/// ai[0]是残影朝向，ai[1]是残影种类
/// </summary>
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer
{
    public class ShadowPlayerShadow2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Trail");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影残影");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            SpriteEffects SP = projectile.ai[0] > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = Main.projectileTexture[projectile.type];

            Color alpha = lightColor;
            alpha *= (float)projectile.timeLeft / 30;
            float d = 100 * (30 - (float)projectile.timeLeft) / 30;
            if (projectile.ai[1] == 1)
            {
                d = 100 * (float)projectile.timeLeft / 30;
            }

            spriteBatch.Draw(tex, projectile.Center + new Vector2(0.55f, 0) * d * projectile.ai[0] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);
            spriteBatch.Draw(tex, projectile.Center + new Vector2(0.65f, 0) * d * projectile.ai[0] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);
            spriteBatch.Draw(tex, projectile.Center + new Vector2(0.75f, 0) * d * projectile.ai[0] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);
            spriteBatch.Draw(tex, projectile.Center + new Vector2(0.45f, 0) * d * projectile.ai[0] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);





            return false;
        }
    }
}