using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class FlareLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Beam");      //31 42
            DisplayName.AddTranslation(GameCulture.Chinese, "熔岩剑气");
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.netImportant = true;

        }

        public override void AI()
        {
            CastLights();
            //if (!Main.npc[(int)projectile.ai[0]].active) projectile.Kill();
            //NPC owner = Main.npc[(int)projectile.ai[0]];
            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = new Vector2(1, 0) / 100;
            }
            projectile.velocity = Vector2.Normalize(projectile.velocity) / 100;
            if (projectile.timeLeft > 350)
            {
                projectile.ai[1] = 2;
            }
            if (projectile.timeLeft < 350 && projectile.timeLeft > 20)
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
            Texture2D tex = Main.projectileTexture[projectile.type];
            float maxDistance = 1000f;
            float TimeScale = projectile.ai[1] / 20;
            float step = 30f;
            Vector2 unit = Vector2.Normalize(projectile.velocity);

            float r = unit.ToRotation();
            for (float i = 0; i < maxDistance; i += step)
            {
                float Scale = (1 - (i / maxDistance)) * TimeScale;
                Rectangle OriginalFrame = new Rectangle((int)((1 - Scale) * tex.Width / 2), 0, (int)(tex.Width * Scale), tex.Height);
                spriteBatch.Draw(tex, new Vector2(0, projectile.gfxOffY) + projectile.Center + unit * i - Main.screenPosition, OriginalFrame,
                    Color.White, r - MathHelper.Pi / 2, OriginalFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                Lighting.AddLight(projectile.Center + unit * (i + 1), 120, 0, 120);

                spriteBatch.Draw(tex, new Vector2(0, projectile.gfxOffY) + projectile.Center - unit * i - Main.screenPosition, OriginalFrame,
    Color.White, r - MathHelper.Pi / 2, OriginalFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                Lighting.AddLight(projectile.Center - unit * (i + 1), 120, 0, 120);
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Vector2.Normalize(projectile.velocity);
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
                projectile.Center + unit * 1000, 20, ref point) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
                projectile.Center - unit * 1000, 20, ref point);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.Burning, 300);
        }
        public override bool CanDamage()
        {
            return projectile.ai[1] >= 20;
        }
        private void CastLights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Vector2 unit = Vector2.Normalize(projectile.velocity);
            Terraria.Utils.PlotTileLine(projectile.Center - unit * 1000, projectile.Center + unit * 1000, 10, DelegateMethods.CastLight);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}