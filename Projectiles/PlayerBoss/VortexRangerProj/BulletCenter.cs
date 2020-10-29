using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class BulletCenter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("µ¯Ä»Ð£×¼µã");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.hostile = false;
            projectile.alpha = 255;
            projectile.hide = true;
            projectile.scale = 1f;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            projectile.localAI[0]++;
            if (projectile.localAI[0] == 1)
            {
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 5)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<ElectrosphereMissileHostile>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.whoAmI, i + (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X));

                }
            }
            projectile.ai[0]--;
            if (projectile.ai[0] < 0) projectile.Kill();
        }


    }
}