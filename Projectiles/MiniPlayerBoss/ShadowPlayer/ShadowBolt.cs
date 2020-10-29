using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer
{
    public class ShadowBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影剑气");
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
        }
        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            projectile.alpha -= 50;
            if (projectile.alpha < 150) projectile.alpha = 150;
            if (projectile.alpha == 150)
            {
                //projectile.ai[1]++;
                //float k = (float)Math.Sin(projectile.ai[1] / 60 * MathHelper.TwoPi);
                //projectile.alpha = (k + 1) / 2 * 150;
            }
            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, MyDustId.PurpleBlackGrey);
            dust.velocity = Vector2.Normalize(projectile.velocity + new Vector2(0, 0.01f)) * 5;
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.CursedInferno, 240);
        }

    }
}