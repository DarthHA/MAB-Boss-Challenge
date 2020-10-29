using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustDragonHostileHead : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Dragon");
            DisplayName.AddTranslation(GameCulture.Chinese, "星尘龙");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.penetrate = -1;
            projectile.timeLeft = 440;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.netImportant = true;
            cooldownSlot = 1;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null,
                projectile.GetAlpha(Color.White), projectile.rotation, texture2D13.Size() / 2, projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>())) projectile.Kill();

            //keep the head looking right
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2;
            projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;

            //const int aislotHomingCooldown = 1;
            int homingDelay = 30;
            float desiredFlySpeedInPixelsPerFrame = 30 + projectile.ai[1] / 60;
            const float amountOfFramesToLerpBy = 30; // minimum of 1, please keep in full numbers even though it's a float!

            projectile.ai[1]++;
            if (projectile.ai[1] > homingDelay)
            {
                int foundTarget = (int)projectile.ai[0];
                Player p = Main.player[foundTarget];
                Vector2 Dest = p.Center + p.velocity * projectile.ai[1] / 20;
                Vector2 desiredVelocity = projectile.DirectionTo(Dest) * desiredFlySpeedInPixelsPerFrame;
                projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
            }


        }



        public override void Kill(int timeLeft)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, -projectile.velocity.X * 0.2f,
                -projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 2f;
            dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, MyDustId.BlueTrans, -projectile.velocity.X * 0.2f,
                -projectile.velocity.Y * 0.2f, 100);
            Main.dust[dust].velocity *= 2f;
        }
    }
}