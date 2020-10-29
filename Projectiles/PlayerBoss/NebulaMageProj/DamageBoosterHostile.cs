﻿using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class DamageBoosterHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Damage Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "伤害强化焰");
        }
        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.width = 18;
            projectile.height = 28;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            cooldownSlot = 1;

        }

        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] <= 40)
            {
                projectile.alpha = (byte)(255 - projectile.ai[1] / 40 * 255);
            }
            if (projectile.ai[1] > 40)
            {
                Vector2 MoveVel = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center);
                MoveVel *= 5;
                projectile.velocity = (projectile.velocity * 75 + MoveVel * 6) / 80;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<DamageFlare>(), 300);
        }
        public override bool CanDamage()
        {
            if (projectile.ai[1] < 40) return false;
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3) projectile.frame = 0;
            }
            Rectangle TexFrame = new Rectangle(0, 28 * projectile.frame, 18, 28);
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, TexFrame, Color.White * projectile.Opacity, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}