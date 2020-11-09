using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class DayBreakHostile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daybreak");
            DisplayName.AddTranslation(GameCulture.Chinese, "破晓");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;
            if ((projectile.Center - Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center).Length() < 100) projectile.velocity *= 0.95f;
            if (projectile.ai[0] != -1)
            {
                projectile.Center = Main.npc[(int)projectile.ai[0]].Center;
                projectile.scale = 2.0f;
                projectile.width = 60;
                projectile.height = 60;

            }
            projectile.rotation += 0.25f + ((float)projectile.timeLeft / 80) * 0.15f;
            if (projectile.timeLeft > 40)
            {
                projectile.alpha = (byte)(255 * ((float)projectile.timeLeft - 40) / 40);
            }
            else
            {
                projectile.alpha = 0;
            }

            if (projectile.timeLeft == 1)
            {
                Main.PlaySound(SoundID.Item1, projectile.Center);
                Vector2 ShootVel = Vector2.Normalize(Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center);
                int protmp = Projectile.NewProjectile(projectile.Center, ShootVel * 20 + Main.player[Player.FindClosest(projectile.Center, 1, 1)].velocity * 0.275f, ProjectileID.Daybreak, projectile.damage, projectile.knockBack, Main.myPlayer);
                //Main.projectile[protmp].Center = projectile.Center;
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false;
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                if (projectile.ai[0] != -1)
                {
                    Main.projectile[protmp].scale = 1.987f;
                    Main.projectile[protmp].width = (int)(Main.projectile[protmp].width * 1.5f);
                    Main.projectile[protmp].height = (int)(Main.projectile[protmp].height * 1.5f);
                    Main.projectile[protmp].Center = projectile.Center;
                    Main.projectile[protmp].velocity = ShootVel * 30 + Main.player[Player.FindClosest(projectile.Center, 1, 1)].velocity * 0.2f;
                    Main.npc[(int)projectile.ai[0]].velocity = -ShootVel;
                }
                projectile.Kill();
            }
        }

        public override bool CanDamage()
        {
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            Color color27 = Color.White;
            color27.A /= 2;
            color27 *= projectile.Opacity;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color28 = color27 * ((float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type]);
                spriteBatch.Draw(Tex, projectile.oldPos[i] + projectile.Size / 2 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, color28, projectile.oldRot[i], Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}