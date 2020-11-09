using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class FlameThrowerHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Thrower");
            DisplayName.AddTranslation(GameCulture.Chinese, "火焰喷射器");
        }
        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 16;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 320;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<VortexRangerBoss>())
            {
                projectile.Kill();
                return;
            }
            projectile.alpha = owner.alpha;
            Player target = Main.player[owner.target];
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center + new Vector2(projectile.spriteDirection * 17, 0);
            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            projectile.ai[1]++;
            if (projectile.ai[1] == 1)
            {
                projectile.localAI[0] = Main.rand.Next(2) * 2 - 1;
            }
            if (projectile.ai[1] <= 20)
            {
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) - MathHelper.Pi / 3 * projectile.localAI[0];
            }

            if (projectile.ai[1] >= 80)
            {
                float FacingR = (float)Math.Atan2(Facing.Y, Facing.X);
                projectile.rotation = NN(projectile.rotation);
                if (Math.Abs(FacingR - projectile.rotation) > MathHelper.Pi)
                {
                    projectile.localAI[0] = Math.Sign(projectile.rotation - FacingR);
                }
                else
                {
                    projectile.localAI[0] = -Math.Sign(projectile.rotation - FacingR);
                }
                projectile.rotation += MathHelper.TwoPi / 240 * projectile.localAI[0];
                projectile.rotation = NN(projectile.rotation);
                if (projectile.rotation > MathHelper.Pi / 2 || projectile.rotation < -MathHelper.Pi / 2)
                {
                    owner.direction = owner.spriteDirection = -1;
                }
                else
                {
                    owner.direction = owner.spriteDirection = 1;
                }
            }
            if (projectile.ai[1] > 70)
            {
                if (projectile.ai[1] % 40 == 20)
                {
                    Main.PlaySound(SoundID.Item34, projectile.position);
                }
                if (projectile.ai[1] % 2 == 1)
                {
                    int protmp = Projectile.NewProjectile(projectile.Center, projectile.rotation.ToRotationVector2() * 25, ProjectileID.Flames, projectile.damage, 0, default);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].ignoreWater = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].tileCollide = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                }

                if (projectile.ai[1] % 4 == 2)
                {
                    int protmp = Projectile.NewProjectile(projectile.Center + projectile.rotation.ToRotationVector2() * Main.rand.Next(1000), (projectile.rotation + MathHelper.Pi * Main.rand.Next(2) - MathHelper.Pi / 2).ToRotationVector2() * 25, ProjectileID.GreekFire2, projectile.damage, 0, default);
                    Main.projectile[protmp].ignoreWater = true;
                    Main.projectile[protmp].tileCollide = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                    if (Main.rand.Next(3) == 1)
                    {
                        protmp = Projectile.NewProjectile(owner.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 7, ProjectileID.GreekFire2, projectile.damage, 0, default);
                        Main.projectile[protmp].ignoreWater = true;
                        Main.projectile[protmp].tileCollide = false;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                    }
                }

            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color27 = Color.White * projectile.Opacity;
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (SP == SpriteEffects.None)
            {
                spriteBatch.Draw(Tex, projectile.position + new Vector2(10, 10) - Main.screenPosition, null, color27, projectile.rotation, new Vector2(10, 10), 1, SP, 0);
            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spriteBatch.Draw(Tex, projectile.position + new Vector2(44, 10) - Main.screenPosition, null, color27, projectile.rotation + MathHelper.Pi, new Vector2(44, 10), 1, SP, 0);
            }
            return false;
        }
        private float NN(float r)
        {
            Vector2 V = r.ToRotationVector2();
            return (float)Math.Atan2(V.Y, V.X);
        }
    }
}