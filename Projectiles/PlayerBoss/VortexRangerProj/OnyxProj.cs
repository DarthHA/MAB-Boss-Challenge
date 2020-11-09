using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class OnyxProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_661";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Shard");
            DisplayName.AddTranslation(GameCulture.Chinese, "玛瑙碎片");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;

        }
        public override void AI()
        {
            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);



            int num111 = 0;
            while (num111 < 16)
            {
                Vector2 vector14 = Vector2.UnitX * 0f;
                vector14 += -Vector2.UnitY.RotatedBy(num111 * (MathHelper.TwoPi / 16), default) * new Vector2(1f, 4f);
                vector14 = vector14.RotatedBy(projectile.velocity.ToRotation(), default);
                int num112 = Dust.NewDust(projectile.Center, 0, 0, 62, 0f, 0f, 0, default, 1f);
                Main.dust[num112].scale = 1.5f;
                Main.dust[num112].noLight = true;
                Main.dust[num112].noGravity = true;
                Main.dust[num112].position = projectile.Center + vector14;
                Main.dust[num112].velocity = Main.dust[num112].velocity * 4f + projectile.velocity * 0.3f;

                num111++;

            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.Pi / 2;
        }
        public override bool PreKill(int timeLeft)
        {
            return false;
        }
        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = (projectile.height = 160);
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);
            Vector2 position = projectile.Center + Vector2.One * -20f;
            int num85 = 40;
            int height3 = num85;
            int num3;
            for (int num86 = 0; num86 < 4; num86 = num3 + 1)
            {
                int num87 = Dust.NewDust(position, num85, height3, 240, 0f, 0f, 100, default, 1.5f);
                Main.dust[num87].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num85 / 2f;
                num3 = num86;
            }
            for (int num88 = 0; num88 < 20; num88 = num3 + 1)
            {
                int num89 = Dust.NewDust(position, num85, height3, 62, 0f, 0f, 200, default, 3.7f);
                Main.dust[num89].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num85 / 2f;
                Main.dust[num89].noGravity = true;
                Main.dust[num89].noLight = true;
                Dust dust = Main.dust[num89];
                dust.velocity *= 3f;
                dust = Main.dust[num89];
                dust.velocity += projectile.DirectionTo(Main.dust[num89].position) * (2f + Main.rand.NextFloat() * 4f);
                num89 = Dust.NewDust(position, num85, height3, 62, 0f, 0f, 100, default, 1.5f);
                Main.dust[num89].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * num85 / 2f;
                dust = Main.dust[num89];
                dust.velocity *= 2f;
                Main.dust[num89].noGravity = true;
                Main.dust[num89].fadeIn = 1f;
                Main.dust[num89].color = Color.Crimson * 0.5f;
                Main.dust[num89].noLight = true;
                dust = Main.dust[num89];
                dust.velocity += projectile.DirectionTo(Main.dust[num89].position) * 8f;
                num3 = num88;
            }
            for (int num90 = 0; num90 < 20; num90 = num3 + 1)
            {
                int num91 = Dust.NewDust(position, num85, height3, 62, 0f, 0f, 0, default, 2.7f);
                Main.dust[num91].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation(), default) * num85 / 2f;
                Main.dust[num91].noGravity = true;
                Main.dust[num91].noLight = true;
                Dust dust = Main.dust[num91];
                dust.velocity *= 3f;
                dust = Main.dust[num91];
                dust.velocity += projectile.DirectionTo(Main.dust[num91].position) * 2f;
                num3 = num90;
            }
            for (int num92 = 0; num92 < 70; num92 = num3 + 1)
            {
                int num93 = Dust.NewDust(position, num85, height3, 240, 0f, 0f, 0, default, 1.5f);
                Main.dust[num93].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation(), default) * num85 / 2f;
                Main.dust[num93].noGravity = true;
                Dust dust = Main.dust[num93];
                dust.velocity *= 3f;
                dust = Main.dust[num93];
                dust.velocity += projectile.DirectionTo(Main.dust[num93].position) * 3f;
                num3 = num92;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Dazed, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex1 = Main.projectileTexture[projectile.type];
            Texture2D tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/VortexRangerProj/OnyxExtra");
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.Purple, projectile.rotation, tex1.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex1.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}