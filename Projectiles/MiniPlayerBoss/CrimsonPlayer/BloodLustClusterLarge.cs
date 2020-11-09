using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class BloodLustClusterLarge : ModProjectile
    {
        readonly float CurrAngle = MathHelper.Pi / 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Cluster");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥屠刀");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 6f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 9999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.hide = true;
            projectile.alpha = 255;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center + projectile.rotation.ToRotationVector2() * 33 * projectile.scale, 250, 100, 100);
            Lighting.AddLight(projectile.Center + projectile.rotation.ToRotationVector2(), 250, 100, 100);
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 50;
            }
            else
            {
                projectile.alpha = 0;
            }
            if (!Main.projectile[(int)projectile.ai[0]].active)
            {
                projectile.Kill();
                return;
            }
            Projectile projowner = Main.projectile[(int)projectile.ai[0]];
            if (!Main.npc[(int)projowner.ai[0]].active)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projowner.ai[0]];
            if (!owner.active) projectile.Kill();
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center;
            projectile.rotation = projowner.rotation;
            if (owner.ai[1] == 13 && owner.ai[2] > 30 && owner.ai[2] < 32)
            {
                if (owner.localAI[2] == 0)
                {
                    if (Main.rand.Next(3) < 2)
                    {
                        Vector2 unit = projectile.rotation.ToRotationVector2();
                        float r1 = Main.rand.Next((int)(66 * projectile.scale));
                        Vector2 unit2 = (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2();
                        float r2 = Main.rand.Next(-40, 40);
                        Vector2 Pos = projectile.Center + unit * r1 + unit2 * r2;
                        Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<IchorDust>(), (int)(projectile.damage * 0.8), 0, default);
                    }
                }
                else
                {
                    if (Main.rand.Next(3) < 1)
                    {
                        Vector2 unit = projectile.rotation.ToRotationVector2();
                        float r1 = Main.rand.Next((int)(66 * projectile.scale));
                        Vector2 unit2 = (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2();
                        float r2 = Main.rand.Next(-40, 40);
                        Vector2 Pos = projectile.Center + unit * r1 + unit2 * r2;
                        Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<IchorDust>(), (int)(projectile.damage * 0.8), 0, default);
                    }
                }
            }
            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item109, projectile.Center);
                projectile.localAI[0] = 1;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 unit = projectile.rotation.ToRotationVector2();
                    float r1 = Main.rand.Next((int)(66 * projectile.scale));
                    Vector2 unit2 = (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2();
                    float r2 = Main.rand.Next(-40, 40);
                    Vector2 Pos = projectile.Center + unit * r1 + unit2 * r2;
                    Dust dust = Main.dust[Dust.NewDust(Pos, 2, 2, MyDustId.RedBlood, 0f, 0f, 100, default, 1f)];
                    dust.noGravity = true;
                    dust.scale = 1.7f;
                    dust.fadeIn = 0.5f;
                }
            }

        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Projectile projowner = Main.projectile[(int)projectile.ai[0]];
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projowner.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color a = lightColor * 0.7f * projectile.Opacity;
            if (projowner.spriteDirection > 0)
            {
                spriteBatch.Draw(Tex, projectile.Center + new Vector2(0, 6) - Main.screenPosition, null, a, projectile.rotation + CurrAngle, new Vector2(0, Tex.Height), projectile.scale, SP, 0);
            }
            else
            {
                spriteBatch.Draw(Tex, projectile.Center + new Vector2(0, 6) - Main.screenPosition, null, a, MathHelper.Pi - CurrAngle + projectile.rotation, new Vector2(Tex.Width, Tex.Height), projectile.scale, SP, 0);
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 unit = (projectile.rotation + CurrAngle).ToRotationVector2();
                float r1 = Main.rand.Next((int)(66 * projectile.scale));
                Vector2 unit2 = (projectile.rotation + CurrAngle + MathHelper.Pi / 2).ToRotationVector2();
                float r2 = Main.rand.Next(-50, 50);
                Vector2 Pos = projectile.Center + unit * r1 + unit2 * r2;
                Dust dust = Main.dust[Dust.NewDust(Pos, 2, 2, MyDustId.RedBlood, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
            }
        }
        public override bool CanDamage()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            return !owner.hide;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = (projectile.rotation + CurrAngle).ToRotationVector2();
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + unit * 60 * projectile.scale, 20, ref point))
            {
                return true;
            }
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return projectile.alpha == 0;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.alpha == 0)
            {
                return null;
            }
            return false;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 240);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 240);
            target.AddBuff(BuffID.Dazed, 60);
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}