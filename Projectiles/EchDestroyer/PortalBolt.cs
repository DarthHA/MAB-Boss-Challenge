using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class PortalBolt : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "传送门弹");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.light = 1f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 30;
            projectile.hostile = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0]++;
                Main.PlaySound(SoundID.Item115, projectile.Center);
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>())) 
            {
                projectile.active = false;
                return;
            }
            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }
            projectile.rotation = projectile.velocity.ToRotation();
            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.1f);
        }

        public override void Kill(int timeLeft)
        {
            Player Target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);
            Projectile.NewProjectile(Target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 300, Vector2.Zero, ModContent.ProjectileType<WormHoleEX>(), 0, 0);

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation + MathHelper.Pi / 2, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool CanDamage()
        {
            return false;
        }
    }
}