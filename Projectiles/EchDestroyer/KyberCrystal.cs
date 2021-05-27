using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class KyberCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kyber Crtstal");
            DisplayName.AddTranslation(GameCulture.Chinese, "凯伯水晶");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 250;
            projectile.timeLeft = 9999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.hostile = true;

        }

        public override void AI()
        {
            if(!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<EchDestroyerHead>())
            {
                projectile.Kill();
                return;
            }
            if (projectile.ai[1] == 0)
            {
                if (projectile.localAI[0] < 110)
                {
                    projectile.localAI[0]++;
                }
            }
            else
            {   
                foreach (Projectile Laser in Main.projectile)
                {
                    if (Laser.active && Laser.type == ModContent.ProjectileType<DSRay>() && Laser.ai[0] == projectile.whoAmI)
                    {
                        Laser.ai[1] = 1;
                    }
                }
                if (projectile.localAI[0] > 0)
                {
                    projectile.localAI[0]--;
                }
                else
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.localAI[0] < 30)
            {
                projectile.Opacity = projectile.localAI[0] / 30;
            }
            else
            {
                projectile.Opacity = 1;
            }
            if (projectile.localAI[0] == 70 && projectile.ai[1] == 0)
            {
                Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
                int protmp = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<DSRay>(), (int)(projectile.damage * 1.25f), 0, default, projectile.whoAmI, 0);
                Main.projectile[protmp].rotation = MathHelper.Pi * (Math.Sign(projectile.velocity.X) + 1) / 2;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.localAI[0] < 60)
            {
                return false;
            }
            return targetHitbox.Distance(projectile.Center) < 200 * projectile.scale;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            float r = 200 + (30 - projectile.localAI[0]) / 30f * 50;
            if (r < 200) r = 200;
            for(float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 8)
            {

                Vector2 DrawPos = projectile.Center + i.ToRotationVector2() * r;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                if (projectile.localAI[0] > 60)             //60-70
                {
                    Texture2D LaserHead = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Head");
                    Texture2D LaserBody = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Body");
                    Texture2D LaserTail = mod.GetTexture("Projectiles/EchDestroyer/DSRay_Tail");
                    float len = (projectile.localAI[0] - 60) / 10 * 200;
                    if (len > 200) len = 200;
                    spriteBatch.Draw(LaserHead, DrawPos - Main.screenPosition, null, Color.White * 0.9f, i + MathHelper.Pi / 2, LaserHead.Size() / 2, projectile.scale, SpriteEffects.None, 0);
                    for (int j = LaserHead.Height / 2; j < len; j += LaserBody.Height)
                    {
                        if (j + LaserBody.Height >= len)
                        {
                            spriteBatch.Draw(LaserTail, DrawPos - i.ToRotationVector2() * j - Main.screenPosition, null, Color.White * 0.9f, i + MathHelper.Pi / 2, LaserTail.Size() / 2, projectile.scale, SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(LaserBody, DrawPos - i.ToRotationVector2() * j - Main.screenPosition, null, Color.White * 0.9f, i + MathHelper.Pi / 2, LaserBody.Size() / 2, projectile.scale, SpriteEffects.None, 0);
                        }
                    }
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(tex, DrawPos - Main.screenPosition, null, Color.White * projectile.Opacity * 0.9f, i, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage < 60) damage = 60;
        }
    }
}