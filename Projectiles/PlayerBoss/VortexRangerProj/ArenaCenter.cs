using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class ArenaCenter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arena Calibration Point");
            DisplayName.AddTranslation(GameCulture.Chinese, "场地校准点");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.hostile = false;
            projectile.alpha = 255;
            projectile.scale = 1f;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if ((!owner.active || owner.type != ModContent.NPCType<VortexRangerBoss>()) && projectile.ai[1] > 0) projectile.ai[1] = -41;
            if (owner.localAI[3] == 1)
            {
                owner.localAI[3] = 0;
                projectile.ai[1] = -41;
            }
            projectile.Center = owner.Center;
            if (projectile.ai[1] <= 41)
            {
                projectile.ai[1]++;
            }
            if (projectile.ai[1] == -1) projectile.Kill();

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            float r = projectile.ai[1] * 37.5f;
            if (projectile.ai[1] < 40 && projectile.ai[1] >= 0)
            {
                r = projectile.ai[1] * 37.5f;
            }
            if (projectile.ai[1] >= 40) r = 1500;
            if (projectile.ai[1] < 0)
            {
                r = (-projectile.ai[1] - 1) * 37.5f;
            }
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 1000)
            {
                spriteBatch.Draw(Main.magicPixel, projectile.Center + i.ToRotationVector2() * r - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.DarkSeaGreen, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}