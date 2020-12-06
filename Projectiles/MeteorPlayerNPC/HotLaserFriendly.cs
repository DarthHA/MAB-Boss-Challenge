using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MeteorPlayerNPC
{
    public class HotLaserFriendly : ModProjectile
    {
        public int Length = 650;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Hotstream");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨石热流"); //31 42    
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            //projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            CastLights();

            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = new Vector2(1, 0) / 100;
            }
            projectile.velocity = Vector2.Normalize(projectile.velocity) / 100;
            if (projectile.timeLeft > 105)
            {
                projectile.ai[1] = 0;
            }
            if (projectile.timeLeft < 105 && projectile.timeLeft > 20)
            {
                if (projectile.ai[1] < 20)
                {
                    projectile.ai[1]++;
                }
            }
            if (projectile.timeLeft < 20)
            {
                projectile.ai[1] = projectile.timeLeft;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            float maxDistance = Length;
            float TimeScale = projectile.ai[1] / 20;
            float step = 42f;
            Vector2 unit = Vector2.Normalize(projectile.velocity);
            Vector2 RanPos = new Vector2(Main.rand.Next(3) - 1, Main.rand.Next(3) - 1);
            float r = unit.ToRotation();
            for (float i = 0; i < maxDistance; i += step)
            {
                float Scale = (1 - (i / maxDistance)) * TimeScale;
                Rectangle OriginalFrame = new Rectangle((int)((1 - Scale) * tex.Width / 2), 0, (int)(tex.Width * Scale), tex.Height);
                spriteBatch.Draw(tex, RanPos + new Vector2(0, projectile.gfxOffY) + projectile.Center + unit * i - Main.screenPosition, OriginalFrame,
                    Color.White * 0.8f, r - MathHelper.Pi / 2, OriginalFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                if (i != 0)
                {
                    spriteBatch.Draw(tex, RanPos + new Vector2(0, projectile.gfxOffY) + projectile.Center - unit * i - Main.screenPosition, OriginalFrame,
        Color.White * 0.8f, r - MathHelper.Pi / 2, OriginalFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Vector2.Normalize(projectile.velocity);
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
                projectile.Center + unit * Length, 20, ref point) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
                projectile.Center - unit * Length, 20, ref point);
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
            target.AddBuff(BuffID.Burning, 240);
        }

        private void CastLights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.1f, 0.1f);
            Vector2 unit = Vector2.Normalize(projectile.velocity);
            Terraria.Utils.PlotTileLine(projectile.Center - unit * Length, projectile.Center + unit * Length, 10, DelegateMethods.CastLight);
        }
    }
}