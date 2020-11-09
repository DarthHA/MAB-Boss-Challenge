using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class ElectrosphereMissileHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_442";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrosphere Missile");
            DisplayName.AddTranslation(GameCulture.Chinese, "电磁火箭");
        }
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.scale = 1f;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.Pi / 2;
            if (projectile.ai[0] > 1000 || projectile.ai[0] < 0) projectile.Kill();
            if (!Main.projectile[(int)projectile.ai[0]].active || Main.projectile[(int)projectile.ai[0]].type != ModContent.ProjectileType<BulletCenter>()) projectile.Kill();
            projectile.Center = Main.projectile[(int)projectile.ai[0]].Center + projectile.ai[1].ToRotationVector2() * (300 - projectile.timeLeft) * 10;
        }
        public override bool CanDamage()
        {
            return false;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item94, projectile.position);
            int num291 = Main.rand.Next(3, 7);
            int num3;
            for (int num292 = 0; num292 < num291; num292 = num3 + 1)
            {
                int num293 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100, default, 2.1f);
                Dust dust = Main.dust[num293];
                dust.velocity *= 2f;
                Main.dust[num293].noGravity = true;
                num3 = num292;
            }

            int num489 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.Electrosphere, projectile.damage, 0f, projectile.owner, 0f, 0f);
            Main.projectile[num489].friendly = false;
            Main.projectile[num489].tileCollide = false;
            Main.projectile[num489].hostile = true;
            Main.projectile[num489].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;


        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex1 = Main.projectileTexture[projectile.type];


            for (int i = 1; i < 4; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.6f;
                color27 *= (float)(4 - i) / 4;
                Vector2 value4 = projectile.position - projectile.velocity * i;

                Main.spriteBatch.Draw(Tex1, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, color27, projectile.rotation, Tex1.Size() * 0.5f, 1.0f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, Color.White, projectile.rotation, Tex1.Size() * 0.5f, 1.0f, SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}