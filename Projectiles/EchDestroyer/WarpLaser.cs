using MABBossChallenge.Buffs;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpLaser : ModProjectile
    {

        public override void SetStaticDefaults()
        {

            DisplayName.SetDefault("Warp Laser");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁射线");
            //DisplayName.AddTranslation(GameCulture.Chinese, "等离子束");
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
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0]++;
                Main.PlaySound(SoundID.Item12, projectile.Center);
            }

            projectile.alpha -= 50;
            if (projectile.alpha < 0) projectile.alpha = 0;

            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 4;
            }
            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = new Vector2(0, -10);
            }
            projectile.rotation = projectile.velocity.ToRotation();
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
            target.AddBuff(ModContent.BuffType<TimeDisort>(), 80);
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

    }
}