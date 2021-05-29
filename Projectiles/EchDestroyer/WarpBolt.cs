using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁飞弹");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.alpha = 250;
            projectile.timeLeft = 480;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1.8f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.localAI[0]++;
            if (projectile.localAI[0] == 1)
            {
                Main.PlaySound(SoundID.Item115, projectile.Center);
            }
            if (projectile.alpha > 50)
            {
                projectile.alpha -= 50;
            }
            else
            {
                projectile.alpha = 0;
            }
            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }
            if (projectile.localAI[0] < 40)
            {
                projectile.velocity *= 1.03f;
            }
            else
            {
                projectile.velocity *= 1.08f;
            }
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.velocity.Length() > 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    var dust = Dust.NewDustDirect(projectile.Center, 1, 1, MyDustId.LightBlueParticle, 0, 0, 100, Color.White, 2f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                    dust.noLight = false;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[0] > 0 && projectile.localAI[0] < 60)
            {
                if (projectile.velocity != Vector2.Zero) 
                {
                    float k = (float)Math.Sin(projectile.localAI[0] / 60 * MathHelper.Pi);
                    Vector2 Unit = Vector2.Normalize(projectile.velocity);
                    Terraria.Utils.DrawLine(spriteBatch, projectile.Center, projectile.Center + Unit * 2000, Color.Cyan * k, Color.Cyan * k, 3);
                } 
            }
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), projectile.GetAlpha(lightColor) * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }
    }
}