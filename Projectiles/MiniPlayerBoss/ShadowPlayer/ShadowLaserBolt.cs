using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer
{
    public class ShadowLaserBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影剑气");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 2f;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 30;
            projectile.hide = true;
        }
        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.timeLeft > 25)
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
            if (projectile.timeLeft < 5)
            {
                projectile.alpha += 50;
                if (projectile.alpha > 255) projectile.alpha = 255;
            }
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
            drawCacheProjsBehindProjectiles.Add(index);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - projectile.velocity * 2 - Main.screenPosition, null, Color.White * 0.3f, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, projectile.Center - projectile.velocity * 1 - Main.screenPosition, null, Color.White * 0.6f, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 240);
            target.AddBuff(BuffID.BrokenArmor, 240);

        }
    }
}