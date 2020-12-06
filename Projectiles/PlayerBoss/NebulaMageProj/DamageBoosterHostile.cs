using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class DamageBoosterHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Damage Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "伤害强化焰");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            cooldownSlot = 1;

        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3) projectile.frame = 0;
            }
            projectile.ai[1]++;
            if (projectile.ai[1] <= 40)
            {
                projectile.alpha = (byte)(255 - projectile.ai[1] / 40 * 255);
            }

            if (projectile.ai[1] > 40)
            {
                if (projectile.ai[0] == 0)
                {
                    Vector2 MoveVel = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center);
                    MoveVel *= 5;
                    projectile.velocity = (projectile.velocity * 75 + MoveVel * 6) / 80;
                }
                else
                {
                    Vector2 Target = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                    Vector2 MoveVel = Vector2.Normalize(Target - projectile.Center);
                    MoveVel *= 5;
                    projectile.velocity = (projectile.velocity * 75 + MoveVel * 6) / 80;
                    if (projectile.Distance(Target) < 5)
                    {
                        projectile.Kill();
                    }
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<DamageFlare>(), 300);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<DamageFlare>(), 300);
        }
        public override bool CanDamage()
        {
            if (projectile.ai[0] == 0)
            {
                return projectile.ai[1] >= 40;
            }
            else
            {
                return projectile.ai[1] >= 10;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Rectangle TexFrame = new Rectangle(0, 28 * projectile.frame, 18, 28);
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, TexFrame, Color.White * projectile.Opacity, 0, TexFrame.Size() * 0.5f, 1, SpriteEffects.None, 0);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}