using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WormHoleSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ech Hole Sentry");
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
            projectile.timeLeft = 265;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return projectile.Distance(target.Center) < projectile.width;
        }

        public override void AI()
        {

            const float maxScale = 1.2f;
            if (projectile.timeLeft > 120)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] == 1)
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
                    {
                        NPC head = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerHead>())];
                        float len = head.Distance(projectile.Center);
                        for(int i = 0; i < len / 8; i++)
                        {
                            Vector2 DustPos = projectile.Center + Vector2.Normalize(head.Center - projectile.Center) * Main.rand.Next((int)len);
                            Dust dust = Dust.NewDustDirect(DustPos, 0, 0, Utils.MyDustId.PurpleShortFx, 0, 0, 0, default, 1.1f);
                            dust.noGravity = true;
                            dust.fadeIn = 0.5f;
                        }
                    }
                }
                if (projectile.ai[0] <= 25)
                {
                    projectile.scale = projectile.ai[0] / 25 * maxScale;
                    projectile.alpha = 255 - (int)(255 * projectile.scale / maxScale);
                    projectile.rotation = projectile.rotation - MathHelper.Pi / 20;
                    if (Main.rand.Next(4) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                        dust.velocity = spinningpoint.RotatedBy(MathHelper.Pi / 2, new Vector2()) * 5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = projectile.Center;
                        dust.color = Color.LightPink;
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = projectile.Center - spinningpoint * 30f;
                        dust.velocity = spinningpoint.RotatedBy(-MathHelper.Pi / 2, new Vector2()) * 2.5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = projectile.Center;
                        dust.color = Color.LightPink;
                    }
                }
                else if (projectile.ai[0] > 25)
                {
                    projectile.scale = maxScale;
                    projectile.alpha = 0;
                    projectile.rotation = projectile.rotation - (float)Math.PI / 60f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                        dust.velocity = spinningpoint.RotatedBy(MathHelper.Pi / 2, new Vector2()) * 5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = projectile.Center;
                        dust.color = Color.LightPink;
                    }
                    else
                    {
                        Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust.noGravity = true;
                        dust.position = projectile.Center - spinningpoint * 30f;
                        dust.velocity = spinningpoint.RotatedBy(-MathHelper.Pi / 2, new Vector2()) * 2.5f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                        dust.customData = projectile.Center;
                        dust.color = Color.LightPink;
                    }
                    if (projectile.ai[1] == 0)
                    {
                        if (projectile.ai[0] % 60 <= 15)
                        {
                            if (projectile.ai[0] % 60 == 1)
                            {
                                Vector2 ShootVel = Vector2.Normalize(Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center - projectile.Center);
                                if (projectile.localAI[0] == 0)
                                {
                                    projectile.localAI[0] = Main.rand.Next(2) * 2 - 1;
                                }
                                else
                                {
                                    projectile.localAI[0] = -projectile.localAI[0];
                                }
                                float ShootR = ShootVel.ToRotation() - projectile.localAI[0] * MathHelper.Pi / 3;
                                projectile.localAI[1] = ShootR;
                            }
                            if (projectile.ai[0] % 2 == 1)
                            {
                                Vector2 ShootVel = projectile.localAI[1].ToRotationVector2();
                                projectile.localAI[1] += projectile.localAI[0] * MathHelper.Pi / 12;
                                Projectile.NewProjectile(projectile.Center, ShootVel * 14, ModContent.ProjectileType<WarpLaser>(), projectile.damage, 0);
                            }
                        }
                    }
                    else if (projectile.ai[1] == 1)
                    {
                        if (projectile.ai[0] % 100 == 15)
                        {
                            float r = Main.rand.NextFloat();
                            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 4) 
                            {
                                Projectile.NewProjectile(projectile.Center, (r + i).ToRotationVector2() * 25, ModContent.ProjectileType<WarpBolt>(), projectile.damage, 0);
                            }
                        }
                    }


                }
            }
            else
            {
                projectile.scale = (float)projectile.timeLeft / 120 * maxScale;
                projectile.alpha = 255 - (int)(255 * projectile.scale / maxScale);
                projectile.rotation = projectile.rotation - (float)Math.PI / 30f;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust1 = Main.dust[Dust.NewDust(projectile.Center - spinningpoint1 * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust1.noGravity = true;
                        dust1.position = projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                        dust1.velocity = spinningpoint1.RotatedBy(MathHelper.Pi / 2, new Vector2()) * 5f;
                        dust1.scale = 0.5f + Main.rand.NextFloat();
                        dust1.fadeIn = 0.5f;
                        dust1.customData = projectile.Center;
                        dust1.color = Color.LightPink;
                        break;
                    case 1:
                        Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * projectile.scale;
                        Dust dust2 = Main.dust[Dust.NewDust(projectile.Center - spinningpoint2 * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                        dust2.noGravity = true;
                        dust2.position = projectile.Center - spinningpoint2 * 30f;
                        dust2.velocity = spinningpoint2.RotatedBy(-MathHelper.Pi / 2, new Vector2()) * 2.5f;
                        dust2.scale = 0.5f + Main.rand.NextFloat();
                        dust2.fadeIn = 0.5f;
                        dust2.customData = projectile.Center;
                        dust2.color = Color.LightPink;
                        break;
                }
            }

            if (Main.rand.Next(2) == 0)
            {
                Dust dust3 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
                dust3.velocity *= 5f;
                dust3.fadeIn = 1f;
                dust3.scale = 0.5f + Main.rand.NextFloat();
                dust3.noGravity = true;
                dust3.color = Color.LightPink;
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
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}