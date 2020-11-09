using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class LunarArrowHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_639";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminite Arrow");
            DisplayName.AddTranslation(GameCulture.Chinese, "夜明箭");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            projectile.MaxUpdates = 2;
            projectile.timeLeft = projectile.MaxUpdates * 60;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.alpha = 255;
            projectile.penetrate = 4;
            projectile.friendly = false;
            projectile.hostile = true;

        }
        public override void AI()
        {
            Player target = Main.player[projectile.owner];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);

            if (projectile.localAI[0] == 0 && projectile.localAI[1] == 0)
            {
                projectile.localAI[0] = projectile.Center.X;
                projectile.localAI[1] = projectile.Center.Y;
                projectile.ai[0] = projectile.velocity.X;
                projectile.ai[1] = projectile.velocity.Y;
            }
            projectile.alpha -= 25;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.timeLeft <= projectile.MaxUpdates * 45 - 14)
            {
                projectile.velocity.Y = projectile.velocity.Y - 0.05f;
            }
        }

        public override void Kill(int timeLeft)
        {
            int num272 = Main.rand.Next(5, 10);       //夜明箭次级弹幕也有
            int num3;
            for (int num273 = 0; num273 < num272; num273 = num3 + 1)
            {
                int num274 = Dust.NewDust(projectile.Center, 0, 0, 220, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[num274];
                dust.velocity *= 1.6f;
                Dust dust28 = Main.dust[num274];
                dust28.velocity.Y -= 1f;
                Main.dust[num274].position = Vector2.Lerp(Main.dust[num274].position, projectile.Center, 0.5f);
                Main.dust[num274].noGravity = true;
                num3 = num273;
            }


            int num = projectile.timeLeft;
            int num275 = num + 1;
            int nextSlot = Projectile.GetNextSlot();
            if (Main.ProjectileUpdateLoopIndex < nextSlot && Main.ProjectileUpdateLoopIndex != -1)
            {
                num3 = num275;
                num275 = num3 + 1;
            }
            //Vector2 vector14 = new Vector2(projectile.ai[0], projectile.ai[1]);
            Vector2 vector14 = projectile.velocity;
            int protmp = Projectile.NewProjectile(projectile.localAI[0] + projectile.velocity.X * 10, projectile.localAI[1] + projectile.velocity.Y * 10, vector14.X, vector14.Y, ProjectileID.MoonlordArrowTrail, projectile.damage, projectile.knockBack, projectile.owner, 0f, num275);
            Main.projectile[protmp].friendly = false;
            Main.projectile[protmp].hostile = true;
            Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            projectile.damage = (int)(projectile.damage * 0.96);
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