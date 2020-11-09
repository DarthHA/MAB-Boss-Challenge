using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class IchorDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Dust");
            DisplayName.AddTranslation(GameCulture.Chinese, "灵液尘");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = -1;
            projectile.hide = true;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            projectile.velocity *= .96f;

            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, MyDustId.Ichor);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 2.5f;

            Lighting.AddLight(projectile.position, .3f, .1f, .3f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.Ichor, 120);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}