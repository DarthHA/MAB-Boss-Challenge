using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class BloodLustClusterHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Cluster");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥屠刀");
        }
        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 50;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            cooldownSlot = 1;
        }
        public override void AI()
        {

            projectile.width = (int)(44 * projectile.scale);
            projectile.height = (int)(50 * projectile.scale);
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
            Vector2 Facing = Main.player[owner.target].Center - owner.Center;
            projectile.rotation = Facing.ToRotation();
            projectile.direction = projectile.spriteDirection = owner.direction;

            if (owner.ai[1] == 0 || owner.ai[1] == 3 || owner.ai[1] == 5 || owner.ai[1] == 7 || owner.ai[1] == 9 || owner.ai[1] == 12 || owner.ai[1] == 14)                  //后跃
            {
                projectile.hide = true;
                projectile.rotation = (owner.direction > 0) ? -MathHelper.Pi / 8 * 5 : -MathHelper.Pi / 8 * 3;
            }

            if (owner.ai[1] == 1)                //大波
            {
                projectile.hide = false;
                if (owner.ai[2] == 20)
                {
                    int protmp = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodLustClusterLarge>(), (int)(projectile.damage * 1.5), 0, default, projectile.whoAmI);
                    Main.projectile[protmp].timeLeft = 50;
                }
                if (owner.ai[2] < 40)
                {
                    projectile.rotation = (owner.direction > 0) ? -MathHelper.Pi / 8 * 5 : -MathHelper.Pi / 8 * 3;
                }
                if (owner.ai[2] >= 40 && owner.ai[2] < 50)
                {
                    if (owner.direction > 0)
                    {
                        projectile.rotation = -MathHelper.Pi / 8 * 5 + MathHelper.Pi / 4 * 3 * (owner.ai[2] - 40) / 10;
                    }
                    else
                    {
                        projectile.rotation = -MathHelper.Pi / 8 * 5 - MathHelper.Pi / 4 * 3 * (owner.ai[2] - 40) / 10;
                    }
                }
                if (owner.ai[2] >= 50 && owner.ai[2] < 80)
                {
                    if (owner.direction > 0)
                    {
                        projectile.rotation = MathHelper.Pi / 8 - MathHelper.Pi / 4 * 3 * (owner.ai[2] - 50) / 25;
                    }
                    else
                    {
                        projectile.rotation = -MathHelper.Pi / 8 * 11 + MathHelper.Pi / 4 * 3 * (owner.ai[2] - 50) / 25;
                    }
                }
                if (owner.ai[2] > 80)
                {
                    projectile.hide = true;
                }
            }

            if (owner.ai[1] == 2)               //雨云砸地
            {

                if (owner.ai[2] > 70)
                {
                    projectile.hide = false;
                    projectile.rotation = MathHelper.Pi / 2;
                }
                else
                {
                    projectile.hide = true;
                }
            }



            if (owner.ai[1] == 4)               //灵液砸地
            {
                projectile.hide = false;
                projectile.rotation = (float)Math.Atan2(owner.velocity.Y, owner.velocity.X) + MathHelper.Pi / 2 * owner.direction;
                if (owner.velocity.Y >= 0)
                {
                    projectile.rotation = MathHelper.Pi / 2;
                }

            }

            if (owner.ai[1] == 6)                 //电锯斩
            {

                if (owner.ai[2] < 20)
                {
                    if (owner.direction > 0)
                    {
                        float d = -MathHelper.Pi / 8 * 5 - projectile.rotation;
                        projectile.rotation += d / 20 * owner.ai[2];
                    }
                    else
                    {
                        float d = -MathHelper.Pi / 8 * 3 - projectile.rotation + MathHelper.TwoPi;
                        projectile.rotation += d / 20 * owner.ai[2];
                    }
                }

                if (owner.ai[2] >= 20 && owner.ai[2] < 40)
                {
                    projectile.rotation = (owner.direction > 0) ? -MathHelper.Pi / 8 * 5 : -MathHelper.Pi / 8 * 3;
                }
                if (owner.ai[2] < 40)
                {
                    projectile.hide = false;
                }
                if (owner.ai[2] == 40)
                {
                    projectile.hide = true;
                }
                if (owner.ai[2] == 135)
                {
                    projectile.hide = false;
                }
            }

            if (owner.ai[1] == 8)                 //平A
            {
                projectile.hide = false;
                if (owner.ai[2] % 20 == 5 && owner.ai[2] > 20)
                {
                    Main.PlaySound(SoundID.Item71, projectile.Center);
                    //float FacingR = (float)Math.Atan2(Facing.Y, Facing.X);
                    int T = (int)(owner.ai[2] / 20);

                    Vector2 ShootVel = Vector2.Normalize(Main.player[owner.target].Center - owner.Center);
                    float ShootR = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                    if (T % 2 == 0)
                    {
                        Projectile.NewProjectile(projectile.Center, ShootVel * 12, ModContent.ProjectileType<CrimsonBolt>(), projectile.damage, 0, default);
                    }
                    else
                    {
                        Projectile.NewProjectile(projectile.Center, (ShootR + MathHelper.Pi / 6).ToRotationVector2() * 10, ModContent.ProjectileType<CrimsonBolt>(), projectile.damage, 0, default);
                        Projectile.NewProjectile(projectile.Center, (ShootR - MathHelper.Pi / 6).ToRotationVector2() * 10, ModContent.ProjectileType<CrimsonBolt>(), projectile.damage, 0, default);
                    }
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

            if (owner.ai[1] == 10)           //后跃
            {
                projectile.hide = true;
            }
            if (owner.ai[1] == 11)            //雨云冲刺
            {
                if (owner.ai[2] == 30)
                {
                    projectile.hide = false;
                }
                projectile.rotation = (owner.direction > 0) ? -MathHelper.Pi / 16 * 7 : -MathHelper.Pi / 16 * 9;
            }

            if (owner.ai[1] == 13)              //大刀
            {
                projectile.hide = false;
                if (owner.ai[2] == 10)
                {
                    int protmp = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodLustClusterLarge>(), (int)(projectile.damage * 1.5f), 0, default, projectile.whoAmI);
                    Main.projectile[protmp].scale = 10;
                }
                if (owner.ai[2] <= 30)
                {
                    projectile.rotation = (owner.direction > 0) ? -MathHelper.Pi / 8 * 5 : -MathHelper.Pi / 8 * 3;
                }
                if (owner.ai[2] > 30)
                {
                    if (owner.velocity.Y != 0)
                    {
                        if (owner.direction > 0)
                        {
                            projectile.rotation = owner.velocity.ToRotation() + MathHelper.Pi / 5 - MathHelper.Pi / 2;
                            if (projectile.rotation > 0) projectile.rotation = 0;
                        }
                        else
                        {
                            projectile.rotation = owner.velocity.ToRotation() - MathHelper.Pi / 5 + MathHelper.Pi / 2;
                            if (projectile.rotation.ToRotationVector2().Y > 0) projectile.rotation = MathHelper.Pi;
                        }
                    }
                    else
                    {
                        projectile.rotation = (owner.direction > 0) ? 0 : MathHelper.Pi;
                    }
                }
                if (owner.ai[2] == 55)
                {
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<BloodLustClusterLarge>() && proj.ai[0] == projectile.whoAmI)
                        {
                            proj.Kill();
                        }
                    }
                }
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //NPC owner = Main.npc[(int)projectile.ai[0]];
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (projectile.spriteDirection > 0)
            {
                spriteBatch.Draw(Tex, projectile.Center + new Vector2(0, 6) - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.Pi / 4, new Vector2(0, Tex.Height), projectile.scale, SP, 0);
            }
            else
            {
                spriteBatch.Draw(Tex, projectile.Center + new Vector2(0, 6) - Main.screenPosition, null, lightColor, MathHelper.Pi / 4 * 3 + projectile.rotation, new Vector2(Tex.Width, Tex.Height), projectile.scale, SP, 0);
            }



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
            return !projectile.hide;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 240);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 240);
            target.AddBuff(BuffID.Ichor, 240);
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}