using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class BloodLustClusterThown : ModProjectile
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("血腥屠刀");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.scale = 3f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            projectile.rotation += 0.8f;
            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 15;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            for (int i = 6; i > 0; i--)
            {
                float a = (7 - i) / 7;

                spriteBatch.Draw(Tex, projectile.Center + projectile.oldPos[i] - projectile.position - Main.screenPosition, null, lightColor * a, projectile.oldRot[i], Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 240);
            target.AddBuff(BuffID.Ichor, 240);
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}