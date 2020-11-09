
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class NebulaBlazeHostileEX : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Blaze");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云烈焰");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.extraUpdates = 2;
            projectile.scale = 1.5f;
            cooldownSlot = 1;
        }

        public override void AI()
        {

            Player target = Main.player[projectile.owner];
            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);

            if (++projectile.localAI[1] < 85 * projectile.MaxUpdates)  //home
            {
                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = Main.player[projectile.owner].Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.016f));
                projectile.tileCollide = false;
            }

            float num1 = 5f;
            Vector2 vector2_1 = new Vector2(8f, 10f);
            float num4 = 1.2f;
            int Type1 = RandomSelect();
            int Type2 = 255;
            if (projectile.ai[1] == 0.0)
            {
                projectile.ai[1] = 1f;
                projectile.localAI[0] = -Main.rand.Next(48);
                Main.PlaySound(SoundID.Item34, projectile.position);
            }

            if (projectile.ai[1] >= 1.0 && projectile.ai[1] < (double)num1)
            {
                ++projectile.ai[1];
                if (projectile.ai[1] == (double)num1)
                    projectile.ai[1] = 1f;
            }
            projectile.alpha -= 40;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            projectile.spriteDirection = projectile.direction;
            projectile.frameCounter++;
            if (projectile.frameCounter >= 4 * projectile.MaxUpdates)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= 4)
                    projectile.frame = 0;
            }
            Lighting.AddLight(projectile.Center, 0.7f, 0.1f, 0.5f);
            projectile.rotation = projectile.velocity.ToRotation();
            ++projectile.localAI[0];
            if (projectile.localAI[0] == 48.0)
            {
                projectile.localAI[0] = 0.0f;
            }
            else if (projectile.alpha == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2_3 = -Vector2.UnitY.RotatedBy(projectile.localAI[0] * 0.130899697542191 + index1 * 3.14159274101257, new Vector2()) * vector2_1 - projectile.rotation.ToRotationVector2() * 10f;
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, Type2, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = num4;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2_3 + projectile.velocity * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(projectile.Center + projectile.velocity * 2f * 8f - Main.dust[index2].position) * 2f + projectile.velocity * 2f;
                }
            }
            if (Main.rand.Next(12) == 0)
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy(projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index2].velocity *= 0.1f;
                    Main.dust[index2].position = projectile.Center + vector2_2 * projectile.width / 2f + projectile.velocity * 2f;
                    Main.dust[index2].fadeIn = 0.9f;
                }
            }
            if (Main.rand.Next(64) == 0)
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.392699092626572).RotatedBy(projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 155, new Color(), 0.8f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].position = projectile.Center + vector2_2 * projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy(projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Type1, 0.0f, 0.0f, 0, new Color(), 1.2f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2_2 * projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }

            if (Main.rand.Next(3) == 0)
            {
                Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy(projectile.velocity.ToRotation(), new Vector2());
                int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, Type2, 0.0f, 0.0f, 100, new Color(), 1f);
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].position = projectile.Center + vector2_2 * projectile.width / 2f;
                Main.dust[index].fadeIn = 1.2f;
                Main.dust[index].scale = 1.5f;
                Main.dust[index].noGravity = true;

            }
        }

        public override void Kill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            int Type1 = 88;
            int Type2 = 88;
            int num2 = 50;
            float Scale1 = 3.7f;
            float Scale2 = 1.5f;
            float Scale3 = 2.2f;
            Vector2 vector2 = (projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * projectile.velocity.Length() * projectile.MaxUpdates;
            vector2 *= 0.5f;
            
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = num2;
            projectile.Center = projectile.position;
            for (int index1 = 0; index1 < 40; ++index1)
            {
                int Type3 = RandomSelect2();
                
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type3, 0.0f, 0.0f, 200, new Color(), Scale1);
                Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity += vector2 * Main.rand.NextFloat();
                int index3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type1, 0.0f, 0.0f, 100, new Color(), Scale2);
                Main.dust[index3].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                Dust dust3 = Main.dust[index3];
                dust3.velocity *= 2f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].fadeIn = 1f;
                Main.dust[index3].color = Color.Crimson * 0.5f;
                Dust dust4 = Main.dust[index3];
                dust4.velocity += vector2 * Main.rand.NextFloat();
            }
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type2, 0.0f, 0.0f, 0, new Color(), Scale3);
                Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy(projectile.velocity.ToRotation(), new Vector2()) * projectile.width / 3f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 0.5f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity += vector2 * (float)(0.600000023841858 + 0.600000023841858 * Main.rand.NextFloat());
            }
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = texture2D13.Height / 4; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public int RandomSelect()
        {
            int Type1 = 242;
            switch (Main.rand.Next(5))
            {
                case 0:
                    Type1 = 242;
                    break;
                case 1:
                    Type1 = 73;
                    break;
                case 2:
                    Type1 = 72;
                    break;
                case 3:
                    Type1 = 71;
                    break;
                case 4:
                    Type1 = 255;
                    break;
                default:
                    break;
            }
            return Type1;
        }

        public int RandomSelect2()
        {
            int Type1 = 242;
            switch (Main.rand.Next(3))
            {
                case 0:
                    Type1 = 242;
                    break;
                case 1:
                    Type1 = 59;
                    break;
                case 2:
                    Type1 = 88;
                    break;
                default:
                    break;
            }
            return Type1;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}