using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
/// <summary>
/// ai[0]是残影朝向，ai[1]是残影种类
/// </summary>
namespace MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer
{
    public class ShadowPlayerClone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Clone");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影分身");
        }
        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 52;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.extraUpdates = 1;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, new Vector3(100, 100, 100));
            Player p = Main.player[Player.FindClosest(projectile.position, 20, 20)];
            if (projectile.ai[1] <= 80)
            {
                Vector2 Facing = Vector2.Normalize(p.Center + p.velocity / 2 - projectile.Center);
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi / 4;
                projectile.ai[0] = Math.Sign(Facing.X);
            }
            projectile.ai[1]++;
            if (projectile.ai[1] == 80)
            {
                Main.PlaySound(SoundID.Item20, projectile.Center);
            }
            if (projectile.ai[1] >= 80)
            {
                projectile.velocity += (projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 1;
                if (projectile.velocity.Length() > 20) projectile.velocity = Vector2.Normalize(projectile.velocity) * 20;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            SpriteEffects SP = projectile.ai[0] > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = Main.projectileTexture[projectile.type];

            Color alpha = lightColor * 0.2f;
            alpha *= Terraria.Utils.Clamp(projectile.ai[1] / 30, 0, 1);
            float d = 100 * Terraria.Utils.Clamp((30 - projectile.ai[1]) / 30, 0, 1);

            if (projectile.velocity == Vector2.Zero)
            {
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 4)
                {
                    spriteBatch.Draw(tex, projectile.Center + i.ToRotationVector2() * d * 0.5f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);
                    spriteBatch.Draw(Main.projectileTexture[ModContent.ProjectileType<LightsBaneHostile>()], projectile.Center + i.ToRotationVector2() * d * 0.5f + new Vector2(0, 6) - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, projectile.rotation, new Vector2(0, 36), projectile.scale, SpriteEffects.None, 0);
                }
            }
            else
            {
                for (float i = 8; i > 0; i--)
                {
                    spriteBatch.Draw(tex, projectile.Center - projectile.velocity * i / 2 - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, lightColor * (1 - i / 9), 0, tex.Size() / 2f, projectile.scale, SP, 0f);
                    spriteBatch.Draw(Main.projectileTexture[ModContent.ProjectileType<LightsBaneHostile>()], projectile.Center - projectile.velocity * i / 2 + new Vector2(0, 6) - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, lightColor * (1 - i / 9), projectile.rotation, new Vector2(0, 36), projectile.scale, SpriteEffects.None, 0);
                }
            }
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, 0, tex.Size() / 2f, projectile.scale, SP, 0f);
            spriteBatch.Draw(Main.projectileTexture[ModContent.ProjectileType<LightsBaneHostile>()], projectile.Center + new Vector2(0, 6) - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null, alpha, projectile.rotation, new Vector2(0, 36), projectile.scale, SpriteEffects.None, 0);


            return false;
        }
        public override bool CanDamage()
        {
            return projectile.ai[1] >= 30;
        }


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.BrokenArmor, 60);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 60);
            target.AddBuff(BuffID.Darkness, 120);
        }
    }
}