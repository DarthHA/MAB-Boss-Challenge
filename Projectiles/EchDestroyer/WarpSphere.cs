using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpSphere : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Sphere");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁能量弹");
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.scale = 1.0f;
            projectile.width = 35;
            projectile.height = 35;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item20, projectile.Center);
            }
            projectile.localAI[0]++;
            if (projectile.timeLeft < 60)
            {
                projectile.Opacity = (float)projectile.timeLeft / 60;
            }
            else
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                if (projectile.localAI[0] > 30)
                {
                    Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
                    if (projectile.Distance(target.Center) < 150)
                    {
                        Vector2 MoveVel = Vector2.Normalize(target.Center - projectile.Center) * 5;
                        projectile.velocity = (projectile.velocity * 93 + MoveVel * 8) / 100;
                    }
                }
            }
            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }
        }

        public override bool CanDamage()
        {
            return projectile.timeLeft > 30;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<TimeDisort>(), 120);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
        public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] <= 30)
            {
                return false;
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color alpha = Color.White * projectile.Opacity;

            Rectangle Frame = new Rectangle(0, 44 * projectile.frame, 38, 44);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), Frame, alpha, projectile.rotation, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }

    }
}