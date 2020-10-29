
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class BloodBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Wave");
            DisplayName.AddTranslation(GameCulture.Chinese, "血之剑波");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 63;
            projectile.scale = 6.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 250;
            projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }
        public override void AI()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, MyDustId.RedBlood, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
            }

            if (projectile.velocity.X == 0) projectile.velocity.X = 10;
            if (NPCUtils.BuffedEvilFighter())
            {
                projectile.velocity.X *= 1.1f;
            }
            else
            {
                projectile.velocity.X *= 1.15f;
            }

            projectile.direction = Math.Sign(projectile.velocity.X);
            if (projectile.alpha >= 25)
            {
                projectile.alpha -= 25;
            }
            else
            {
                projectile.alpha = 0;
            }
            projectile.velocity.Y += 0.1f;
            projectile.localAI[0]++;
            if (projectile.localAI[0] == 50)
            {
                Main.PlaySound(SoundID.Item113, projectile.Center);
            }
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Color alpha = Color.White * 0.7f * projectile.Opacity;
            SpriteEffects SP = (projectile.direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float i = 4; i > 0; i--)
            {
                float a = (5 - i) / 5;
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition - new Vector2(Math.Sign(projectile.velocity.X) * 15 * i, 0), null, alpha * a, 0, tex.Size() / 2, projectile.scale, SP, 0);
            }
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, alpha, 0, tex.Size() / 2, projectile.scale, SP, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X == 0 && oldVelocity.X != 0 && projectile.alpha == 0) { projectile.Kill(); }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Bleeding, 240);
            target.AddBuff(BuffID.Ichor, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 120);
        }
    }
}