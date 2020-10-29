using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class CrimsonBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Wave");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥剑气");
        }
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.scale = 2.0f;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            Vector2 unit = (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - unit * 30, projectile.Center + unit * 30, 30 * projectile.scale, ref point))
            {
                return true;
            }
            return false;
        }
        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            projectile.alpha -= 50;
            if (projectile.alpha < 100) projectile.alpha = 100;

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, MyDustId.RedBlood);
                dust.velocity = Vector2.Normalize(projectile.velocity + new Vector2(0, 0.01f)) * 5;
                dust.noGravity = true;
                dust.noLight = false;
            }
            Vector2 unit = (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * projectile.scale;
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position - unit * 20 + projectile.rotation.ToRotationVector2() * 6, projectile.width, projectile.height, MyDustId.RedBlood);
                dust.velocity = Vector2.Normalize(projectile.velocity + new Vector2(0, 0.01f)) * 5;
                dust.noGravity = true;
                dust.noLight = false;
            }

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position + unit * 20 + projectile.rotation.ToRotationVector2() * 6, projectile.width, projectile.height, MyDustId.RedBlood);
                dust.velocity = Vector2.Normalize(projectile.velocity + new Vector2(0, 0.01f)) * 5;
                dust.noGravity = true;
                dust.noLight = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            if (Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.Ichor, 120);
            target.AddBuff(BuffID.Bleeding, 120);
        }

    }
}