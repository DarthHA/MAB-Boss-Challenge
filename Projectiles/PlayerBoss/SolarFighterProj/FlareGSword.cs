using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class FlareGSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Greatsword");
            DisplayName.AddTranslation(GameCulture.Chinese, "熔岩巨剑");
        }
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.scale = 1.3f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {


            projectile.width = (int)(54 * projectile.scale);
            projectile.height = (int)(54 * projectile.scale);
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<SolarFighterBoss>())  
            {
                projectile.Kill();
                return;
            }

            Dust dust = Main.dust[Dust.NewDust(projectile.position + new Vector2(owner.direction * 30, 0), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f)];
            dust.noGravity = true;
            dust.scale = 1.7f;
            dust.fadeIn = 0.5f;
            dust.velocity *= 5f;

            if (owner.ai[1] != 7 && owner.ai[1] != 8)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = owner.Center;
            Player target = Main.player[owner.target];
            Vector2 Facing = target.Center - projectile.Center;
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X);
            if (owner.ai[1] == 7)
            {
                if (owner.ai[2] > 180)
                {
                    projectile.rotation = (float)Math.Atan2(owner.velocity.Y, owner.velocity.X);
                }
            }

            if (owner.ai[1] == 8)
            {
                if (owner.ai[2] > 30)
                {
                    if (owner.ai[2] <= 40)
                    {
                        projectile.rotation -= owner.direction * (owner.ai[2] - 30) / 10 * MathHelper.Pi / 4 * 3;
                    }
                    if (owner.ai[2] == 40)
                    {
                        Main.PlaySound(SoundID.Item71, projectile.Center);
                    }
                    if (owner.ai[2] > 40 && owner.ai[2] < 50)
                    {
                        projectile.rotation -= owner.direction * MathHelper.Pi / 4 * 3;
                        projectile.rotation += owner.direction * (owner.ai[2] - 40) / 10 * MathHelper.Pi / 2 * 3;
                        int protmp = Projectile.NewProjectile(projectile.Center, (projectile.rotation + MathHelper.Pi / 30).ToRotationVector2() * 15, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, owner.target);
                        Main.projectile[protmp].hostile = true;
                        Main.projectile[protmp].friendly = false;
                        Main.projectile[protmp].scale = 2;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                        protmp = Projectile.NewProjectile(projectile.Center, (projectile.rotation - MathHelper.Pi / 30).ToRotationVector2() * 15, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, owner.target);
                        Main.projectile[protmp].hostile = true;
                        Main.projectile[protmp].friendly = false;
                        Main.projectile[protmp].scale = 2;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                    }
                    if (owner.ai[2] > 50)
                    {
                        projectile.rotation += owner.direction * MathHelper.Pi / 4 * 3;
                    }
                    if (owner.ai[2] > 70)
                    {
                        projectile.Kill();
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];

            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation + MathHelper.Pi / 4, new Vector2(0, Tex.Height), projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override bool CanDamage()
        {
            return false;
        }
    }
}