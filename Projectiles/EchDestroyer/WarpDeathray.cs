using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpDeathray : ModProjectile
    {
        readonly int BaseLength = 100;
        List<float> Rot = new List<float>();
        List<float> Length = new List<float>();
        float MaxLength = 0;
        float NodeCount = 20;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Deathray");
            DisplayName.AddTranslation(GameCulture.Chinese, "空间裂缝");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.light = 1f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 560;
            projectile.hostile = true;
            cooldownSlot = 0;
        }

        public override void AI()
        {
            if (projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = PortalUtils.GetRandomUnit() / 1000;
            }
            projectile.rotation = projectile.velocity.ToRotation();
            //Main.NewText(Rot.Count);
            if (Rot.Count == 0)
            {
                for(int i = 0; i < NodeCount; i++)
                {
                    Rot.Add(MathHelper.Pi / 3 * Main.rand.NextFloat() - MathHelper.Pi / 6);
                    int len = BaseLength + Main.rand.Next(-30, 30);
                    Length.Add(len);
                    MaxLength += len;
                }
            }
            if (projectile.timeLeft > 520)
            {
                projectile.localAI[0] = (float)(560 - projectile.timeLeft) / 2;
            }
            else if (projectile.timeLeft < 40)
            {
                projectile.localAI[0] = (float)projectile.timeLeft / 2;
            }
            else
            {
                projectile.localAI[0] = 20;
            }

            if (projectile.soundDelay == 0)
            {
                Main.PlaySound(SoundID.Item15, projectile.position);
                projectile.soundDelay = 40 + Main.rand.Next(60);
            }
            //if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            //{
            //    projectile.active = false;
            //    return;
            //}
        }


        public override bool CanDamage()
        {
            return projectile.localAI[0] >= 20;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity = Vector2.Zero;
            target.AddBuff(ModContent.BuffType<TimeDisort>(), 180);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 RanPos = PortalUtils.GetRandomUnit();
            Texture2D tex = Main.projectileTexture[projectile.type];
            int len = tex.Height - 1;
            if (Rot.Count > 0)
            {
                float Width = projectile.localAI[0] / 20;
                float len2 = 0;
                Vector2 Pos = projectile.Center;
                for(int i = 0; i < Rot.Count; i++)
                {
                    for (int j = 0; j < Length[i]; j += len) 
                    {
                        len2 += len;
                        float actualWidth;
                        if (len2 <= MaxLength / 2) 
                        {
                            actualWidth = Width * (len2 / (MaxLength / 2));
                        }
                        else
                        {
                            actualWidth = Width * ((MaxLength - len2) / (MaxLength / 2));
                        }
                        actualWidth = (int)(actualWidth * tex.Width);
                        Rectangle rectangle = new Rectangle((int)(tex.Width - actualWidth) / 2, 0, (int)actualWidth, tex.Height);
                        rectangle = new Rectangle(0, 0, (int)actualWidth, tex.Height);
                        rectangle.X += (int)(RanPos + Pos + (projectile.rotation + Rot[i]).ToRotationVector2() * j - Main.screenPosition).X;
                        rectangle.Y += (int)(RanPos + Pos + (projectile.rotation + Rot[i]).ToRotationVector2() * j - Main.screenPosition).Y;
                        spriteBatch.Draw(tex, rectangle, null, Color.White, projectile.rotation + MathHelper.Pi / 2, rectangle.Size() / 2, SpriteEffects.None, 0);
                        //spriteBatch.Draw(tex,RanPos + Pos + (projectile.rotation + Rot[i]).ToRotationVector2() * j - Main.screenPosition, rectangle, Color.White, projectile.rotation + MathHelper.Pi / 2, rectangle.Size() / 2, projectile.scale, SpriteEffects.None, 0);
                    }
                    Pos += (projectile.rotation + Rot[i]).ToRotationVector2() * Length[i];
                }
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            bool result = false;
            if (Rot.Count > 0)
            {
                Vector2 Pos = projectile.Center;
                for (int i = 0; i < Rot.Count; i++)
                {
                    if(Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Pos, Pos + (projectile.rotation + Rot[i]).ToRotationVector2() * Length[i], 15, ref point))
                    {
                        result = true;
                    }
                    Pos += (projectile.rotation + Rot[i]).ToRotationVector2() * Length[i];
                }
            }
            return result;
        }

    }
}