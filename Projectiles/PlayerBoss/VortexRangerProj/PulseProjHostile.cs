using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class PulseProjHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_357";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "脉冲矢");
        }
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.penetrate = 6;
            projectile.alpha = 255;
            projectile.extraUpdates = 2;
            projectile.scale = 1.2f;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            if (projectile.alpha < 170)
            {
                int num;
                for (int num163 = 0; num163 < 10; num163 = num + 1)
                {
                    float x = projectile.position.X - projectile.velocity.X / 10f * num163;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * num163;
                    int num164 = Dust.NewDust(new Vector2(x, y), 1, 1, 206, 0f, 0f, 0, default, 1f);
                    Main.dust[num164].alpha = projectile.alpha;
                    Main.dust[num164].position.X = x;
                    Main.dust[num164].position.Y = y;
                    Dust dust3 = Main.dust[num164];
                    dust3.velocity *= 0f;
                    Main.dust[num164].noGravity = true;
                    num = num163;
                }
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }


            projectile.ai[1]++;
            if (projectile.ai[1] % 150 == 149)
            {
                Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
                Vector2 ShootVel = Vector2.Normalize(target.Center - projectile.Center);
                projectile.velocity = ShootVel * projectile.velocity.Length();
                projectile.penetrate -= 1;
                if (projectile.penetrate == 0) projectile.Kill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}