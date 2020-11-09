using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class HFProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Fire");
            DisplayName.AddTranslation(GameCulture.Chinese, "地狱之火");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.2f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            if (projectile.timeLeft > 170)
            {
                projectile.velocity *= 0.96f;
            }
            else
            {
                if (projectile.velocity.Length() < 30)
                    projectile.velocity *= 1.04f;
            }
            projectile.rotation += 0.5f;
            if (projectile.timeLeft < 60)
            {
                projectile.alpha = (byte)(((float)(60 - projectile.timeLeft)) / 60 * 255);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];


            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity;
                color27.A /= 3;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float s = (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type]; ;
                spriteBatch.Draw(Tex, projectile.oldPos[i] + projectile.Size / 2 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, color27, projectile.oldRot[i], Tex.Size() / 2, s, SpriteEffects.None, 0f);
            }
            Color color28 = Color.White * projectile.Opacity;
            color28.A /= 3;
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, color28, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}