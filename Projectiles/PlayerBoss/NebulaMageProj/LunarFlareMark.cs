using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class LunarFlareMark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strike Mark");
            DisplayName.AddTranslation(GameCulture.Chinese, "打击标记");
            Main.projFrames[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 40;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 120 / 4;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 2;
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item88, projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Vector2 Pos = projectile.Center + new Vector2(Main.rand.Next(-200, 200), -3000);
                int protmp = Projectile.NewProjectile(Pos, Vector2.Normalize(projectile.Center - Pos) * 20, ModContent.ProjectileType<LunarFlareHostile>(), projectile.damage, 0f, Main.myPlayer, 0f);
                Main.projectile[protmp].localAI[0] = projectile.Center.Y;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, 40 * projectile.frame, 40, 40);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.White, 0, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

    }
}