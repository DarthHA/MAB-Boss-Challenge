using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.EchDestroyer;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpBolt : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Bolt");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁飞弹");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.light = 1f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
            projectile.hostile = true;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0]++;
                Main.PlaySound(SoundID.Item115, projectile.Center);
            }
            if (++projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 5;
            }

            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, 24, 24);
            if (projectile.ai[1] == 0)
            {
                Vector2 LtP = target.Center - projectile.Center;
                LtP = Vector2.Normalize(LtP) * (15 + ((float)(360 - projectile.timeLeft) / 36));
                projectile.velocity = (LtP * 6 + projectile.velocity * 85) / 90;
            }

            if (projectile.timeLeft < 270) projectile.ai[1] = 1;
            for (int i = 0; i < 2; i++)
            {
                var dust = Dust.NewDustDirect(projectile.Center, 1, 1, MyDustId.LightBlueParticle, 0, 0, 100, Color.White, 2f);
                dust.noGravity = true;
                dust.velocity *= 0f;
            }
            projectile.rotation = projectile.velocity.ToRotation();
            foreach (Player plr in Main.player)
            {
                if (projectile.Hitbox.Intersects(plr.Hitbox))
                {
                    projectile.Kill();
                }
            }
           
            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.1f);
        }


        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);

            projectile.Center -= projectile.Size * 3;
            projectile.width *= 6;
            projectile.height *= 6;
            for (int i = 0; i < 20; i++)
            {
                var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, MyDustId.LightBlueParticle, 0, 0, 100, Color.White, 2f);
                dust.noGravity = true;
                dust.velocity *= 0f;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            projectile.timeLeft = 0;
            target.AddBuff(ModContent.BuffType<TimeDisort>(), 100);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation + MathHelper.Pi / 2, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}