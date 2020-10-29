using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class CrimsonRam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Ram");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥冲刺");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            projectile.Center = owner.Center;
            if (owner.velocity.X < 0)
            {
                projectile.rotation = MathHelper.Pi;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }



    }
}