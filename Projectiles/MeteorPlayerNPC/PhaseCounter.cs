using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MeteorPlayerNPC
{
    public class PhaseCounter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PhaseCounter");
            DisplayName.AddTranslation(GameCulture.Chinese, "碰撞判断");
        }
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.scale = 1.0f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 100;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.hide = true;
            projectile.idStaticNPCHitCooldown = 0;
            projectile.usesIDStaticNPCImmunity = true;
        }
        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active)
            {
                projectile.active = false;
                return;
            }

            NPC owner = Main.npc[(int)projectile.ai[0]];
            projectile.Center = owner.Center;
            projectile.damage = (int)(owner.damage * 0.4f);
            projectile.penetrate = -1;
            if (!owner.dontTakeDamageFromHostiles)
            {
                projectile.active = false;
            }
        }
        public override bool PreKill(int timeLeft)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (owner.dontTakeDamageFromHostiles)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
            }
            return base.PreKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (MABWorld.DownedMeteorPlayerEX && NPC.downedMoonlord)
            {
                damage *= 3;
            }
            damage += target.defense / 2;

        }

    }
}