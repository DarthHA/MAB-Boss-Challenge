using MABBossChallenge.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class SolarRamFireball : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "日耀火球");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = -1;
            projectile.timeLeft = 120;
            projectile.hostile = true;
            projectile.scale = 1.0f;
            cooldownSlot = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {

            projectile.velocity *= 0.95f;
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 4;
            }
        }
        public override bool CanDamage()
        {
            return projectile.timeLeft < 100;
        }
        public override void Kill(int timeLeft)
        {
            int protmp = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SolarCounter, projectile.damage, 0);
            Main.projectile[protmp].Kill();
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];

            int num156 = texture2D13.Height / 4; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}