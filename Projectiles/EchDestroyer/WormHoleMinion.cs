using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WormHoleMinion : ModProjectile
    {

        public const int Radius = 450;

        public float RotSpeed = MathHelper.Pi / 100;

        public float[] rot = new float[3] { 0, 0, 0 };
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Hole");
            DisplayName.AddTranslation(GameCulture.Chinese, "虫洞");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 999999;
            projectile.netImportant = true;
        }

        public override bool CanDamage()
        {
            return false;
        }


        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
                if (projectile.timeLeft > 120)
                {
                    projectile.timeLeft = 120;
                }
            }

            projectile.Center = Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center;
            projectile.rotation += RotSpeed;
            float maxScale = 1.2f;

            projectile.ai[1]++;
            if (projectile.ai[1] <= 25)
            {
                projectile.scale = projectile.ai[1] / 25 * maxScale;
                projectile.alpha = 255 - (int)(255 * projectile.scale / maxScale);
                for (int i = 0; i < 3; i++)
                {
                    Color DustColor = i == 0 ? Color.LightBlue : Color.LightPink;
                    Vector2 Pos = projectile.Center + (projectile.rotation + MathHelper.TwoPi / 3 * i).ToRotationVector2() * Radius;
                    rot[i] -= MathHelper.Pi / 20;

                    if (Main.rand.Next(4) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(Pos - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = Pos - spinningpoint * Main.rand.Next(10, 21);
                        dust.velocity = spinningpoint.RotatedBy(MathHelper.Pi / 2, new Vector2()) * 5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = Pos;
                        dust.color = DustColor;
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(Pos - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = Pos - spinningpoint * 30f;
                        dust.velocity = spinningpoint.RotatedBy(-MathHelper.Pi / 2, new Vector2()) * 2.5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = Pos;
                        dust.color = DustColor;
                    }
                }
            }
            else if (projectile.ai[1] > 25)
            {
                projectile.scale = maxScale;
                projectile.alpha = 0;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 Pos = projectile.Center + (projectile.rotation + MathHelper.TwoPi / 3 * i).ToRotationVector2() * Radius;
                    rot[i] -= MathHelper.Pi / 60f;
                    Color DustColor = i == 0 ? Color.LightBlue : Color.LightPink;

                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(Pos - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = Pos - spinningpoint * Main.rand.Next(10, 21);
                        dust.velocity = spinningpoint.RotatedBy(MathHelper.Pi / 2, new Vector2()) * 5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = Pos;
                        dust.color = DustColor;
                    }
                    else
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(Pos - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = Pos - spinningpoint * 30f;
                        dust.velocity = spinningpoint.RotatedBy(-MathHelper.Pi / 2, new Vector2()) * 2.5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = Pos;
                        dust.color = DustColor;
                    }
                }



                if (projectile.ai[1] % 80 <= 40 && projectile.ai[1] <= 160 && projectile.ai[1] > 40)
                {
                    if (projectile.ai[1] % 15 == 0)
                    {
                        for (int i = 1; i < 3; i++)
                        {
                            Vector2 Pos = projectile.Center + (projectile.rotation + MathHelper.TwoPi / 3 * i).ToRotationVector2() * Radius;
                            Vector2 ShootVel = Vector2.Normalize(projectile.Center - Pos);
                            Projectile.NewProjectile(Pos, ShootVel * 14, ModContent.ProjectileType<WarpLaser>(), projectile.damage, 0);
                        }
                    }
                }
            }


            for (int i = 0; i < 3; i++)
            {
                Vector2 Pos = projectile.Center + (projectile.rotation + MathHelper.TwoPi / 3 * i).ToRotationVector2() * Radius;
                Color DustColor = i == 0 ? Color.LightBlue : Color.LightPink;
                if (Main.rand.Next(2) == 0)
                {
                    Dust dust3 = Main.dust[Dust.NewDust(Pos, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
                    dust3.velocity *= 5f;
                    dust3.fadeIn = 1f;
                    dust3.scale = 0.5f + Main.rand.NextFloat();
                    dust3.noGravity = true;
                    dust3.color = DustColor;
                }
            }
        }



        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            for (int i = 0; i < 3; i++)
            {
                Texture2D texture2D13;
                if (i == 0)
                {
                    texture2D13 = Main.projectileTexture[ModContent.ProjectileType<WormHole>()];
                }
                else
                {
                    texture2D13 = Main.projectileTexture[projectile.type];
                }

                Vector2 Pos = projectile.Center + (projectile.rotation + MathHelper.TwoPi / 3 * i).ToRotationVector2() * Radius;

                int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;
                Main.spriteBatch.Draw(texture2D13, Pos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -rot[i], origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
                Main.spriteBatch.Draw(texture2D13, Pos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), rot[i], origin2, projectile.scale, SpriteEffects.None, 0f);

            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


        public static int FindFirstMinion()
        {
            foreach(Projectile proj in Main.projectile)
            {
                if(proj.active && proj.type == ModContent.ProjectileType<WormHoleMinion>())
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }
    }
}