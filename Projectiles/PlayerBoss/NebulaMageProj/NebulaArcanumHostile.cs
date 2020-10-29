
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class NebulaArcanumHostile : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_617";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Arcanum");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云奥秘");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.scale = 1.0f;
            cooldownSlot = 1;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            //projectile.hide = true;
            projectile.penetrate = 3;
        }

        public override void AI()
        {
            SwarmAI();
            Player target = Main.player[projectile.owner];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);

            projectile.ai[0] += 1f;
            int num1013 = 0;
            if (projectile.velocity.Length() <= 7f)
            {
                num1013 = 1;
            }
            projectile.alpha -= 15;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (num1013 == 0)
            {
                projectile.rotation -= 0.104719758f;
                if (Main.rand.Next(3) == 0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector139 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust122 = Main.dust[Dust.NewDust(projectile.Center - vector139 * 30f, 0, 0, (Main.rand.Next(2) == 0) ? 86 : 90, 0f, 0f, 0, default, 1f)];
                        dust122.noGravity = true;
                        dust122.position = projectile.Center - vector139 * Main.rand.Next(10, 21);
                        dust122.velocity = vector139.RotatedBy(MathHelper.Pi / 2, default) * 6f;
                        dust122.scale = 0.5f + Main.rand.NextFloat();
                        dust122.fadeIn = 0.5f;
                        dust122.customData = projectile;
                    }
                    else
                    {
                        Vector2 vector140 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust123 = Main.dust[Dust.NewDust(projectile.Center - vector140 * 30f, 0, 0, 240, 0f, 0f, 0, default, 1f)];
                        dust123.noGravity = true;
                        dust123.position = projectile.Center - vector140 * 30f;
                        dust123.velocity = vector140.RotatedBy(-MathHelper.Pi / 2, default) * 3f;
                        dust123.scale = 0.5f + Main.rand.NextFloat();
                        dust123.fadeIn = 0.5f;
                        dust123.customData = projectile;
                    }
                }
                if (projectile.ai[0] >= 30f)
                {
                    projectile.velocity *= 0.98f;
                    projectile.scale += 0.00744680827f;
                    if (projectile.scale > 1.3f)
                    {
                        projectile.scale = 1.3f;
                    }
                    projectile.rotation -= 0.0174532924f;
                }
                if (projectile.velocity.Length() < 5.1f)
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 5f;
                    projectile.ai[0] = 0f;
                }
            }
            else if (num1013 == 1)
            {
                projectile.rotation -= 0.104719758f;
                int num3;
                for (int num1014 = 0; num1014 < 1; num1014 = num3 + 1)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector141 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust124 = Main.dust[Dust.NewDust(projectile.Center - vector141 * 30f, 0, 0, 86, 0f, 0f, 0, default, 1f)];
                        dust124.noGravity = true;
                        dust124.position = projectile.Center - vector141 * Main.rand.Next(10, 21);
                        dust124.velocity = vector141.RotatedBy(MathHelper.Pi / 2, default) * 6f;
                        dust124.scale = 0.9f + Main.rand.NextFloat();
                        dust124.fadeIn = 0.5f;
                        dust124.customData = projectile;
                        vector141 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        dust124 = Main.dust[Dust.NewDust(projectile.Center - vector141 * 30f, 0, 0, 90, 0f, 0f, 0, default, 1f)];
                        dust124.noGravity = true;
                        dust124.position = projectile.Center - vector141 * Main.rand.Next(10, 21);
                        dust124.velocity = vector141.RotatedBy(MathHelper.Pi / 2, default) * 6f;
                        dust124.scale = 0.9f + Main.rand.NextFloat();
                        dust124.fadeIn = 0.5f;
                        dust124.customData = projectile;
                        dust124.color = Color.Crimson;
                    }
                    else
                    {
                        Vector2 vector142 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        Dust dust125 = Main.dust[Dust.NewDust(projectile.Center - vector142 * 30f, 0, 0, 240, 0f, 0f, 0, default, 1f)];
                        dust125.noGravity = true;
                        dust125.position = projectile.Center - vector142 * Main.rand.Next(20, 31);
                        dust125.velocity = vector142.RotatedBy(-MathHelper.Pi / 2, default) * 5f;
                        dust125.scale = 0.9f + Main.rand.NextFloat();
                        dust125.fadeIn = 0.5f;
                        dust125.customData = projectile;
                    }
                    num3 = num1014;
                }
                if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 241f)
                {
                    Vector2 vector143 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector143.X, vector143.Y, ModContent.ProjectileType<NASecondaryProj>(), projectile.damage / 3 * 2, 0f, projectile.owner, 0f, projectile.whoAmI);

                }
                Vector2 vector144 = projectile.Center;
                bool flag58 = false;
                if (projectile.ai[1] == 0f)
                {
                    Vector2 center13 = Main.player[projectile.owner].Center;
                    int num1016;
                    if (Collision.CanHit(new Vector2(projectile.position.X + projectile.width / 2, projectile.position.Y + projectile.height / 2), 1, 1, Main.player[projectile.owner].position, Main.player[projectile.owner].width, Main.player[projectile.owner].height) || true)
                    {
                        vector144 = center13;
                        flag58 = true;
                        num1016 = projectile.owner;
                    }
                    if (flag58)
                    {
                        if (projectile.ai[1] != num1016 + 1)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.ai[1] = num1016 + 1;
                    }
                    flag58 = false;
                }
                if (projectile.ai[1] != 0f)
                {
                    flag58 = true;
                    vector144 = Main.player[projectile.owner].Center;
                }
                if (flag58)
                {
                    float num1019 = 7f;          //速度
                    int num1020 = 8;
                    Vector2 vector145 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    float num1021 = vector144.X - vector145.X;
                    float num1022 = vector144.Y - vector145.Y;
                    float num1023 = (float)Math.Sqrt(num1021 * num1021 + num1022 * num1022);
                    num1023 = num1019 / num1023;
                    num1021 *= num1023;
                    num1022 *= num1023;
                    projectile.velocity.X = (projectile.velocity.X * (num1020 - 1) + num1021) / num1020;
                    projectile.velocity.Y = (projectile.velocity.Y * (num1020 - 1) + num1022) / num1020;
                }
            }

            if (projectile.alpha < 150)
            {
                Lighting.AddLight(projectile.Center, 0.7f, 0.2f, 0.6f);
            }
            if (projectile.ai[0] >= 600f)
            {
                projectile.Kill();
                return;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = (projectile.height = 176);
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);
            int num3;
            for (int num94 = 0; num94 < 4; num94 = num3 + 1)
            {
                int num95 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 240, 0f, 0f, 100, default, 1.5f);
                Main.dust[num95].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                num3 = num94;
            }
            for (int num96 = 0; num96 < 30; num96 = num3 + 1)
            {
                int num97 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, 0f, 0f, 200, default, 3.7f);
                Main.dust[num97].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                Main.dust[num97].noGravity = true;
                Dust dust = Main.dust[num97];
                dust.velocity *= 3f;
                num97 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, 0f, 0f, 100, default, 1.5f);
                Main.dust[num97].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                dust = Main.dust[num97];
                dust.velocity *= 2f;
                Main.dust[num97].noGravity = true;
                Main.dust[num97].fadeIn = 1f;
                Main.dust[num97].color = Color.Crimson * 0.5f;
                num3 = num96;
            }
            for (int num98 = 0; num98 < 10; num98 = num3 + 1)
            {
                int num99 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, 0f, 0f, 0, default, 2.7f);
                Main.dust[num99].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation(), default) * projectile.width / 2f;
                Main.dust[num99].noGravity = true;
                Dust dust = Main.dust[num99];
                dust.velocity *= 3f;
                num3 = num98;
            }
            for (int num100 = 0; num100 < 10; num100 = num3 + 1)
            {
                int num101 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 240, 0f, 0f, 0, default, 1.5f);
                Main.dust[num101].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation(), default) * projectile.width / 2f;
                Main.dust[num101].noGravity = true;
                Dust dust = Main.dust[num101];
                dust.velocity *= 3f;
                num3 = num100;
            }
            for (int num102 = 0; num102 < 2; num102 = num3 + 1)
            {
                int num103 = Gore.NewGore(projectile.position + new Vector2(projectile.width * Main.rand.Next(100) / 100f, projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64), 1f);
                Main.gore[num103].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                Gore gore = Main.gore[num103];
                gore.velocity *= 0.3f;
                Gore gore17 = Main.gore[num103];
                gore17.velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Gore gore18 = Main.gore[num103];
                gore18.velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
                num3 = num102;
            }

            for (int num104 = 0; num104 < 1000; num104 = num3 + 1)
            {
                if (Main.projectile[num104].active && Main.projectile[num104].type == 618 && Main.projectile[num104].ai[1] == projectile.whoAmI)
                {
                    Main.projectile[num104].Kill();
                }
                num3 = num104;
            }
            int num105 = Main.rand.Next(5, 9);
            int num106 = Main.rand.Next(5, 9);
            int num107 = (Main.rand.Next(2) == 0) ? 86 : 90;
            int num108 = (num107 == 86) ? 90 : 86;
            for (int num109 = 0; num109 < num105; num109 = num3 + 1)
            {
                Vector2 vector4 = projectile.Center + RandomVector2(Main.rand, -30f, 30f);
                Vector2 vector5 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                while (vector5.X == 0f && vector5.Y == 0f)
                {
                    vector5 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                }
                vector5.Normalize();
                if (vector5.Y > 0.2f)
                {
                    vector5.Y *= -1f;
                }
                vector5 *= Main.rand.Next(70, 101) * 0.1f;
                int protmp = Projectile.NewProjectile(vector4.X, vector4.Y, vector5.X, vector5.Y, ProjectileID.NebulaArcanumExplosionShotShard, (int)(projectile.damage * 0.75), projectile.knockBack * 0.8f, projectile.owner, num107, 0f);
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false;
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                num3 = num109;
            }
            for (int num110 = 0; num110 < num106; num110 = num3 + 1)
            {
                Vector2 vector6 = projectile.Center + RandomVector2(Main.rand, -30f, 30f);
                Vector2 vector7 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                while (vector7.X == 0f && vector7.Y == 0f)
                {
                    vector7 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                }
                vector7.Normalize();
                if (vector7.Y > 0.4f)
                {
                    vector7.Y *= -1f;
                }
                vector7 *= Main.rand.Next(40, 81) * 0.1f;
                int protmp = Projectile.NewProjectile(vector6.X, vector6.Y, vector7.X, vector7.Y, ProjectileID.NebulaArcanumExplosionShotShard, (int)(projectile.damage * 0.75), projectile.knockBack * 0.8f, projectile.owner, num108, 0f);
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false;
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                num3 = num110;
            }
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 vector60 = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D31 = Main.projectileTexture[projectile.type];
            Color alpha4 = projectile.GetAlpha(lightColor);
            Vector2 origin8 = new Vector2(texture2D31.Width, texture2D31.Height) / 2f;
            Color color57 = alpha4 * 0.8f;
            color57.A /= 2;
            Color color58 = Color.Lerp(alpha4, Color.Black, 0.5f);
            color58.A = alpha4.A;
            float num279 = 0.95f + (projectile.rotation * 0.75f).ToRotationVector2().Y * 0.1f;
            color58 *= num279;
            float scale13 = 0.6f + projectile.scale * 0.6f * num279;
            Texture2D texture30 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/NAExtra");
            spriteBatch.Draw(texture30, vector60, null, color58, -projectile.rotation + 0.35f, origin8, scale13, SpriteEffects.FlipHorizontally, 0f);   //color58
            spriteBatch.Draw(texture30, vector60, null, alpha4, -projectile.rotation, origin8, projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture2D31, vector60, null, color57, -projectile.rotation * 0.7f, origin8, projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(texture30, vector60, null, alpha4 * 0.8f, projectile.rotation * 0.5f, origin8, projectile.scale * 0.9f, SpriteEffects.None, 0f);
            alpha4.A = 0;
            //Texture2D TexEX = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/NebulaArcanumExtra");
            //spriteBatch.Draw(TexEX, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, TexEX.Size() * 0.5f,0.4f, SpriteEffects.None, 0);
            /*
			int texid = ((int)projectile.ai[0] == 1) ? ProjectileID.NebulaBlaze2 : ProjectileID.NebulaBlaze1;
            Texture2D texture2D13 = Main.projectileTexture[texid];
            int num156 = Main.projectileTexture[texid].Height / Main.projFrames[texid]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            */
            return false;
        }



        public void SwarmAI()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.Hitbox.Intersects(projectile.Hitbox) && proj.active && proj.type == projectile.type && proj.whoAmI != projectile.whoAmI)
                {
                    Vector2 AwayVel = proj.Center - projectile.Center;
                    AwayVel.Normalize();
                    projectile.Center -= AwayVel;
                }
            }
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            return false;
        }
        private Vector2 RandomVector2(UnifiedRandom random, float min, float max)
        {
            return new Vector2((max - min) * (float)random.NextDouble() + min, (max - min) * (float)random.NextDouble() + min);
        }
    }
}