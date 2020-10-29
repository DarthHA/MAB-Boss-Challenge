using MABBossChallenge.Buffs;
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
    public class SunFuryHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunfury");
            DisplayName.AddTranslation(GameCulture.Chinese,"阳炎之怒");
        }
        public override void SetDefaults()
        {
            projectile.width = 35;
            projectile.height = 35;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<SolarFighterBoss>()) projectile.Kill();
            if (projectile.ai[1] == 0)          //旋转状态
            {
                projectile.localAI[1]++;
                if (projectile.localAI[1] < 120)
                {
                    projectile.Center = owner.Center + ((projectile.localAI[1] % 15) / 15 * MathHelper.TwoPi).ToRotationVector2() * 120;
                }
                else
                {
                    projectile.Kill();
                }
                if (projectile.localAI[1] % 3 == 2)
                {
                    Vector2 Vel = Vector2.Normalize(projectile.Center - owner.Center);
                    int protmp = Projectile.NewProjectile(projectile.Center, Vel * 15, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, Main.player[owner.target].whoAmI);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].scale = 2.0f;
                }
            }

            if (projectile.ai[1] == 1)           //投掷状态
            {
                projectile.localAI[1]++;

                if (projectile.localAI[1] == 30)
                {

                    Main.PlaySound(SoundID.Item, projectile.Center, 14);

                    int protmp = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, projectile.damage, 4, Main.myPlayer);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                    for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 16)
                    {
                        float r = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                        int protmp1 = Projectile.NewProjectile(projectile.Center, (i + r).ToRotationVector2() * 15, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, Main.player[owner.target].whoAmI);
                        Main.projectile[protmp1].hostile = true;
                        Main.projectile[protmp1].friendly = false;
                        Main.projectile[protmp1].scale = 2.0f;
                        Main.projectile[protmp].tileCollide = false;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                        protmp1 = Projectile.NewProjectile(projectile.Center, (i + r + MathHelper.Pi / 32).ToRotationVector2() * 10, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, Main.player[owner.target].whoAmI);
                        Main.projectile[protmp1].hostile = true;
                        Main.projectile[protmp1].friendly = false;
                        Main.projectile[protmp1].scale = 2.0f;
                        Main.projectile[protmp].tileCollide = false;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                        protmp1 = Projectile.NewProjectile(projectile.Center, (i + r).ToRotationVector2() * 5, ProjectileID.DD2FlameBurstTowerT3Shot, projectile.damage, 0, Main.player[owner.target].whoAmI);
                        Main.projectile[protmp1].hostile = true;
                        Main.projectile[protmp1].friendly = false;
                        Main.projectile[protmp1].scale = 2.0f;
                        Main.projectile[protmp].tileCollide = false;
                        Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;

                    }
                }
                if (projectile.localAI[1] > 30)
                {
                    Vector2 ReturnVel = Vector2.Normalize(owner.Center - projectile.Center);
                    projectile.velocity = ReturnVel * 30;
                }
                projectile.Center += owner.position - owner.oldPosition;
                if (projectile.localAI[1] > 30 && (projectile.Center - owner.Center).Length() < 50)
                {
                    projectile.Kill();
                }

            }


            Vector2 Line = Vector2.Normalize(projectile.Center - owner.Center);
            projectile.rotation = (float)Math.Atan2(Line.Y, Line.X);
            if (Main.rand.Next(2) == 1)
            {
                Dust dust = Main.dust[Dust.NewDust(owner.Center + Line * Main.rand.Next((int)(projectile.Center - owner.Center).Length()), 10, 10, 6, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity *= 5f;
            }
            if (projectile.ai[1] > 240)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.Center, 10, 10, 6, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity *= 5f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            Vector2 Line = Vector2.Normalize(projectile.Center - owner.Center);
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/SolarFighterProj/SunFuryChain");
            Color color27 = Color.White;
            color27.A /= 2;
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, Tex1.Size() * 0.5f, 1, SpriteEffects.None, 0);
            for (int i = 0; i < (projectile.Center - owner.Center).Length() - 17; i += 9)
            {
                spriteBatch.Draw(Tex2, owner.Center + Line * i - Main.screenPosition, null, color27, projectile.rotation + MathHelper.Pi / 2, Tex2.Size() * 0.5f, 1.0f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (projectile.ai[1] == 1)
                damage = (int)(damage * 1.5f);
        }
    }
}