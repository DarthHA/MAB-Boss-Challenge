using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class DayBreakHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.Kill();
        }
        public override void Kill(int timeLeft)
        {
            int protmp = Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileID.Daybreak, projectile.damage, projectile.knockBack, Main.myPlayer);
            Main.projectile[protmp].hostile = true;
            Main.projectile[protmp].friendly = false;
            Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
        }


    }
}