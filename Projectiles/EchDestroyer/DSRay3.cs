using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class DSRay3 : ModProjectile
    {
        public override string Texture => "MABBossChallenge/Projectiles/EchDestroyer/DSRay_Body";
        public int LaserLen = 9999;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DS Deathray");
            DisplayName.AddTranslation(GameCulture.Chinese, "死星致命光");
        }

        public override void SetDefaults()
        {
            projectile.width = 35;
            projectile.height = 35;
            projectile.timeLeft = 9999;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if(!Main.projectile[(int)projectile.ai[0]].active || Main.projectile[(int)projectile.ai[0]].type != ModContent.ProjectileType<KyberCrystal2>())
            {
                projectile.Kill();
                return;
            }
            if (projectile.ai[1] == 0)
            {
                if (projectile.localAI[0] < 20)
                {
                    projectile.localAI[0]++;
                }
            }
            else
            {
                if (projectile.localAI[0] > 0)
                {
                    projectile.localAI[0]--;
                }
                else
                {
                    projectile.Kill();
                    return;
                }
            }
            projectile.rotation = -MathHelper.Pi / 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float k = projectile.localAI[0] / 20;
            Texture2D LaserHead = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Head");
            Texture2D LaserBody = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Body");
            Texture2D LaserTail = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Tail");
            int HeadHeight = (int)(LaserHead.Height * 3f);
            int BodyHeight= (int)(LaserBody.Height * 3f);
            spriteBatch.Draw(LaserHead, projectile.Center - Main.screenPosition, null, Color.White * 0.9f, projectile.rotation - MathHelper.Pi / 2, LaserHead.Size() / 2, new Vector2(k, 1) * projectile.scale * 3f, SpriteEffects.None, 0);
            for (int j = HeadHeight / 2; j < LaserLen; j += BodyHeight)
            {
                if (j + BodyHeight >= LaserLen)
                {
                    spriteBatch.Draw(LaserTail, projectile.Center + projectile.rotation.ToRotationVector2() * j - Main.screenPosition, null, Color.White * 0.9f, projectile.rotation - MathHelper.Pi / 2, LaserTail.Size() / 2, new Vector2(k, 1) * projectile.scale * 3f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(LaserBody, projectile.Center + projectile.rotation.ToRotationVector2() * j - Main.screenPosition, null, Color.White * 0.9f, projectile.rotation - MathHelper.Pi / 2, LaserBody.Size() / 2, new Vector2(k, 1) * projectile.scale * 3f, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float k = projectile.localAI[0] / 20;
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.rotation.ToRotationVector2() * LaserLen, 35 * k, ref point);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 100) damage = 100;
        }
    }
}