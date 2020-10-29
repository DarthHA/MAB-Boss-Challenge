using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.ShadowPlayer
{
    public class LightsBaneHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lights Bane");
            DisplayName.AddTranslation(GameCulture.Chinese, "光之驱逐");
        }
        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {

            projectile.width = (int)(36 * projectile.scale);
            projectile.height = (int)(36 * projectile.scale);
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active) projectile.Kill();
            //projectile.spriteDirection = owner.spriteDirection;
            //projectile.direction = owner.direction;
            projectile.Center = owner.Center;
            projectile.hide = owner.hide;
            Vector2 Facing = Main.player[owner.target].Center - owner.Center;
            projectile.rotation = Facing.ToRotation() + MathHelper.Pi / 4;

            if (owner.ai[1] == 0 || owner.ai[1] == 2 || owner.ai[1] == 4)
            {
                projectile.rotation = owner.direction > 0 ? 0 : -MathHelper.Pi / 2;
            }
            if (owner.ai[1] == 5)
            {
                projectile.rotation = owner.direction > 0 ? MathHelper.Pi / 4 : -MathHelper.Pi / 4 * 3;
            }
            if (owner.ai[1] == 1)
            {
                if (owner.ai[2] > 10)
                {
                    if (owner.ai[2] <= 15)
                    {
                        projectile.rotation -= owner.direction * (owner.ai[2] - 10) / 5 * MathHelper.Pi / 2;
                    }
                    if (owner.ai[2] == 15)
                    {
                        Main.PlaySound(SoundID.Item71, projectile.Center);
                    }

                    if (owner.ai[2] > 15 && owner.ai[2] <= 25)
                    {
                        projectile.rotation -= owner.direction * MathHelper.Pi / 2;
                        projectile.rotation += owner.direction * (owner.ai[2] - 15) / 10 * MathHelper.Pi;
                        //Projectile.NewProjectile(projectile.Center, (projectile.rotation - MathHelper.Pi / 4 + MathHelper.Pi / 40).ToRotationVector2() * 10, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);
                        //Projectile.NewProjectile(projectile.Center, (projectile.rotation - MathHelper.Pi / 4 - MathHelper.Pi / 40).ToRotationVector2() * 10, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);

                    }
                    if (owner.ai[2] == 20)
                    {
                        if (NPCUtils.BuffedEvilFighter())
                        {
                            for (float i = -MathHelper.Pi / 2; i <= MathHelper.Pi / 2; i += MathHelper.Pi / 10)
                            {
                                Projectile.NewProjectile(projectile.Center, (i + projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 10, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);
                            }
                        }
                        else
                        {
                            for (float i = -MathHelper.Pi / 2; i <= MathHelper.Pi / 2; i += MathHelper.Pi / 12)
                            {
                                Projectile.NewProjectile(projectile.Center, (i + projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 10, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);
                            }
                        }
                    }

                    if (owner.ai[2] > 25 && owner.ai[2] < 35)
                    {
                        projectile.rotation += owner.direction * MathHelper.Pi / 2;
                    }
                }

            }

            if (owner.ai[1] == 3)
            {
                if (owner.direction > 0)
                {
                    projectile.rotation = (owner.velocity + new Vector2(0, 0.01f)).ToRotation() + MathHelper.Pi / 4 * 3;
                    if (owner.velocity.Y >= 0) projectile.rotation = MathHelper.Pi / 4 * 3;
                }
                else
                {
                    projectile.rotation = (owner.velocity + new Vector2(0, 0.01f)).ToRotation() - MathHelper.Pi / 4;
                    if (owner.velocity.Y >= 0) projectile.rotation = MathHelper.Pi / 4 * 3;
                }

            }
            if (owner.ai[1] == 6 || owner.ai[1] == 7)
            {
                projectile.rotation = MathHelper.Pi / 4 * 3;
            }
            if (owner.ai[1] == 9)
            {

                if (owner.ai[2] % 20 == 5 && owner.ai[2] > 20)
                {
                    Main.PlaySound(SoundID.Item71, projectile.Center);
                    float FacingR = (float)Math.Atan2(Facing.Y, Facing.X);
                    float r = NPCUtils.BuffedEvilFighter() ? MathHelper.Pi / 16 : MathHelper.Pi / 12;
                    Projectile.NewProjectile(projectile.Center, FacingR.ToRotationVector2() * 15, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);
                    Projectile.NewProjectile(projectile.Center, (FacingR + r).ToRotationVector2() * 15, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);
                    Projectile.NewProjectile(projectile.Center, (FacingR - r).ToRotationVector2() * 15, ModContent.ProjectileType<ShadowBolt>(), projectile.damage, 0);

                }
                if (owner.ai[2] % 20 < 15 && owner.ai[2] > 20)
                {
                    int T = (int)(owner.ai[2] / 20);
                    if (T % 2 == 0)
                    {
                        projectile.rotation -= MathHelper.Pi / 3 * owner.direction;
                        projectile.rotation += owner.direction * (owner.ai[2] % 20) / 10 * MathHelper.Pi / 3 * 2;
                    }
                    else
                    {
                        projectile.rotation += MathHelper.Pi / 3 * owner.direction;
                        projectile.rotation -= owner.direction * (owner.ai[2] % 20) / 10 * MathHelper.Pi / 3 * 2;
                    }
                }
                else
                {
                    int T = (int)(owner.ai[2] / 20);
                    if (T % 2 == 0)
                    {
                        projectile.rotation += MathHelper.Pi / 3 * owner.direction;
                    }
                    else
                    {
                        projectile.rotation -= MathHelper.Pi / 3 * owner.direction;
                    }
                }

            }
            //if (owner.ai[1] == 4)
            //{
            //   if (owner.ai[2] > 5)
            //   {
            //        int T = (int)(owner.ai[2] / 35) * 35;
            //        projectile.rotation += Utils.NPCUtils.SinEX((owner.ai[2] - 5) / 15 * MathHelper.Pi) * MathHelper.Pi;
            //    }
            // }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //NPC owner = Main.npc[(int)projectile.ai[0]];
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(Tex, projectile.Center + new Vector2(0, 6) - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(0, Tex.Height), projectile.scale, SP, 0);


            //DP(spriteBatch, owner.Center - Main.screenPosition, lightColor);
            return false;
        }
        private void DP(SpriteBatch spritebatch, Vector2 Pos, Color a)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            int FrameR = 2;
            if (owner.direction > 0)
            {
                if (projectile.rotation < 0 && projectile.rotation > -MathHelper.Pi / 8)
                {
                    FrameR = 2;
                }
                if (projectile.rotation >= 0 && projectile.rotation < MathHelper.Pi / 8)
                {
                    FrameR = 3;
                }
                if (projectile.rotation >= MathHelper.Pi / 8 && projectile.rotation < MathHelper.Pi / 2)
                {
                    FrameR = 4;
                }
                if (projectile.rotation <= -MathHelper.Pi / 8)
                {
                    FrameR = 5;
                }
            }
            else
            {
                if (projectile.rotation > -MathHelper.Pi && projectile.rotation < -MathHelper.Pi / 8 * 7)
                {
                    FrameR = 2;
                }
                if (projectile.rotation <= MathHelper.Pi && projectile.rotation > MathHelper.Pi / 8 * 7)
                {
                    FrameR = 3;
                }
                if (projectile.rotation <= MathHelper.Pi / 8 * 7 && projectile.rotation > MathHelper.Pi / 2)
                {
                    FrameR = 4;
                }
                if (projectile.rotation >= -MathHelper.Pi / 8 * 7)
                {
                    FrameR = 5;
                }
            }
            owner.localAI[2] = FrameR;
            Rectangle Frame = new Rectangle(0, 56 * FrameR, 40, 56);
            SpriteEffects SP = owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/ShadowPlayerBoss_Arm");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);

        }
        public override bool CanDamage()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            return !owner.hide;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 240);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 240);
            target.AddBuff(BuffID.CursedInferno, 240);
            target.AddBuff(BuffID.Darkness, 300);
        }
    }
}