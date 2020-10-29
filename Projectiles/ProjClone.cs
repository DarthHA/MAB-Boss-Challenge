using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles
{
    public abstract class ProjClone : ModProjectile
    {

        public virtual int Type => ProjectileID.None;
        public virtual string OverrideTexture => "Terraria/Projectile_" + Type;
        public virtual string OverrideName => "";
        public override string Texture => OverrideTexture;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(OverrideName);
            Main.projFrames[projectile.type] = Main.projFrames[Type];
            ProjectileID.Sets.CanDistortWater[projectile.type] = ProjectileID.Sets.CanDistortWater[Type];
            ProjectileID.Sets.TrailCacheLength[projectile.type] = ProjectileID.Sets.TrailCacheLength[Type];
            ProjectileID.Sets.TrailingMode[projectile.type] = ProjectileID.Sets.TrailingMode[Type];
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(Type);
            aiType = Type;
        }
        public override bool PreKill(int timeLeft)
        {
            projectile.type = Type;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}