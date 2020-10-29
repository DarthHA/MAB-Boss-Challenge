using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.CrimsonPlayer
{
    public class BladeSwing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Cluster");
            DisplayName.AddTranslation(GameCulture.Chinese, "血腥屠刀");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.scale = 4f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            cooldownSlot = 1;
        }
        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active)
            {
                projectile.Kill();
                return;
            }

            NPC owner = Main.npc[(int)projectile.ai[0]];
            projectile.spriteDirection = Math.Sign(owner.velocity.X);
            projectile.direction = Math.Sign(owner.velocity.X);
            projectile.Center = owner.Center;
            projectile.rotation += 0.3f * Math.Sign(owner.velocity.X + owner.direction * 0.01f);
            if (owner.ai[2] > 135)
            {
                projectile.Kill();
            }
            projectile.localAI[0]++;
            if (projectile.localAI[0] >= 30)
            {
                projectile.localAI[0] = 0;
                Main.PlaySound(SoundID.DD2_SkyDragonsFurySwing, Main.player[owner.target].Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color a = Color.White * 0.9f;
            if (projectile.spriteDirection > 0)
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, a, projectile.rotation, Tex.Size() / 2, projectile.scale, SP, 0);
            }
            else
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, a, projectile.rotation, Tex.Size() / 2, projectile.scale, SP, 0);
            }

            return false;
        }



        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(projectile.Center) <= 35 * projectile.scale;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 240);
            if (target.buffImmune[BuffID.BrokenArmor] && Utils.NPCUtils.BuffedEvilFighter()) target.AddBuff(BuffID.WitheredArmor, 240);
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.Bleeding, 240);
        }
    }
}