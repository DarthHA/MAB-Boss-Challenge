using Terraria.ID;
using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class HelFireHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hel-Fire Yoyo");
            DisplayName.AddTranslation(GameCulture.Chinese, "狱火球");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.2f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 420;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<SolarFighterBoss>()) 
            {
                projectile.Kill();
                return;
            }
            projectile.rotation += 0.5f;
            if (projectile.timeLeft > 60)
            {
                Vector2 MoveVel = Main.player[projectile.owner].Center - projectile.Center;
                MoveVel.Normalize();
                MoveVel *= 10;
                projectile.velocity = (MoveVel * 12 + projectile.velocity * 195) / 200;
                projectile.Center += owner.position - owner.oldPosition;
                if ((projectile.Center - owner.Center).Length() > 400)
                {
                    Vector2 FacingVel = Vector2.Normalize(owner.Center - projectile.Center);
                    projectile.velocity += 8 * FacingVel;
                }

                projectile.ai[1]++;
                if (projectile.ai[1] % 15 == 5 && projectile.ai[1] > 60)
                {
                    Vector2 ShootVel = Main.player[projectile.owner].Center - projectile.Center;
                    float ShootR = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                    if (projectile.ai[1] % 30 == 5)
                    {
                        for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 3)
                        {
                            Projectile.NewProjectile(projectile.Center, (i + ShootR).ToRotationVector2() * 20, ModContent.ProjectileType<HFProj>(), projectile.damage, 0);
                        }
                    }
                    if (projectile.ai[1] % 30 == 20)
                    {
                        for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 3)
                        {
                            Projectile.NewProjectile(projectile.Center, (i + MathHelper.Pi / 6 + ShootR).ToRotationVector2() * 20, ModContent.ProjectileType<HFProj>(), projectile.damage, 0);
                        }
                    }
                }

            }
            else
            {
                Vector2 MoveVel = owner.Center - projectile.Center;
                if (MoveVel.Length() > 10) MoveVel = MoveVel / MoveVel.Length() * 10;
                projectile.velocity = MoveVel;
                projectile.Center += owner.position - owner.oldPosition;
            }
            Dust dust = Main.dust[Dust.NewDust(projectile.Center, 16, 16, 6, 0f, 0f, 100, default, 1f)];
            dust.noGravity = true;
            dust.scale = 1.7f;
            dust.fadeIn = 0.5f;
            dust.velocity *= 5f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            Vector2 Line = projectile.Center - owner.Center;
            Texture2D Tex1 = Main.projectileTexture[projectile.type];

            for (float i = 0; i <= Line.Length(); i += 1)
            {
                spriteBatch.Draw(Main.magicPixel, owner.Center + Line * i / Line.Length() + Line.Length() / 20 * new Vector2(0, (float)Math.Sin(i / Line.Length() * MathHelper.Pi)) - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.LightGray, 0, new Vector2(1, 1), 2, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Tex1.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);

            return false;
        }


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Burning, 300);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Burning, 300);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}