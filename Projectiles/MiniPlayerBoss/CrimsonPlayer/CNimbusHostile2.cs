using MABBossChallenge.Projectiles.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class CNimbusHostile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Nimbus");
            DisplayName.AddTranslation(GameCulture.Chinese, "血云");
            Main.projFrames[projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            projectile.width = 45;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 120;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 26) % 156;              //28
            }

            if (NPCUtils.BuffedEvilFighter())
            {
                projectile.extraUpdates = 1;
            }

            projectile.ai[0] += NPCUtils.BuffedEvilFighter() ? (Main.rand.Next(2) + 1) : 1;
            int freq = NPCUtils.BuffedEvilFighter() ? 15 : 10;
            if (projectile.ai[0] > freq)
            {
                projectile.ai[0] = 0f;
                int num417 = (int)(projectile.position.X + 14f + Main.rand.Next(projectile.width - 28));
                int num418 = (int)(projectile.position.Y + projectile.height + 4f);
                int protmp = Projectile.NewProjectile(num417, num418, 0f, 5f, ProjectileID.BloodRain, projectile.damage, 0f, projectile.owner, 0f, 0f);
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false;
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
            }

            if (projectile.timeLeft < 40)
            {
                projectile.alpha = (byte)((float)(40 - projectile.timeLeft) / 40 * 255);
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, projectile.frame, 54, 26);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, lightColor * projectile.Opacity, projectile.rotation, Frame.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

    }
}