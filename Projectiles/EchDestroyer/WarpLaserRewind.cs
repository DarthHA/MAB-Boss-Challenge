using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpLaserRewind : ModProjectile
    {
        private Vector2 DefaultVelocity;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Laser - Rewind");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁射线 - 倒带");
            Main.projFrames[projectile.type] = 4;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.alpha = 250;
            projectile.timeLeft = 480;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1.4f;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = new Vector2(0, -10);
            }
            if (projectile.localAI[0] == 0)
            {
                DefaultVelocity = projectile.velocity;
                Main.PlaySound(SoundID.Item12, projectile.Center);
            }
            projectile.localAI[0]++;
            if (projectile.localAI[0] > 80)
            {
                if (projectile.localAI[0] < 105)
                {
                    projectile.velocity = DefaultVelocity * (105 - projectile.localAI[0]) / 25;
                }
                else if (projectile.localAI[0] < 115)
                {
                    projectile.velocity = Vector2.Zero;
                }
                else if (projectile.localAI[0] < 140)
                {
                    projectile.velocity = -DefaultVelocity * (projectile.localAI[0] - 115) / 25 * 4f;
                }
            }

            projectile.alpha -= 50;
            if (projectile.alpha < 0) projectile.alpha = 0;

            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                if (projectile.localAI[0] < 105)
                {
                    projectile.frame = (projectile.frame + 1) % 4;
                }
                else if (projectile.localAI[0] > 115)
                {
                    projectile.frame = projectile.frame - 1;
                    if (projectile.frame < 0) projectile.frame = 3;
                }
            }
            projectile.rotation = DefaultVelocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            int num = Main.rand.Next(3, 7);
            for (int index1 = 0; index1 < num; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, MyDustId.BlueCircle, 0.0f, 0.0f, 100, new Color(), 2.1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 2f;
                Main.dust[index2].noGravity = true;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<TimeDisort>(), 80);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, tex.Height / Main.projFrames[projectile.type] * projectile.frame, tex.Width, tex.Height / Main.projFrames[projectile.type]);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rectangle, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, new Vector2(tex.Width, tex.Width) / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }

    }
}