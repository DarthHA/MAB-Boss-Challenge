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
    public class SolarEruptionHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Eruption");
            DisplayName.AddTranslation(GameCulture.Chinese,"日耀喷发剑");
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 120;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<SolarFighterBoss>()) projectile.Kill();
            bool P2 = false;
            if (owner.ai[0] > 2) P2 = true;
            //Player player = Main.player[Main.npc[(int)projectile.ai[0]].target];
            //projectile.spriteDirection = (projectile.ai[1].ToRotationVector2().X > 0) ? 1 :- 1;
            Vector2 Facing = owner.Center - projectile.Center;
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi;
            if (!P2) projectile.Center = owner.Center + (projectile.ai[1] + projectile.localAI[0] * ((float)projectile.timeLeft - 60) / 60 * MathHelper.Pi / 6).ToRotationVector2() * 600 * (float)Math.Sin((float)projectile.timeLeft / 120 * MathHelper.Pi);
            if (P2) projectile.Center = owner.Center + (projectile.ai[1] + projectile.localAI[0] * ((float)projectile.timeLeft - 60) / 60 * MathHelper.Pi / 3).ToRotationVector2() * 600 * (float)Math.Sin((float)projectile.timeLeft / 120 * MathHelper.Pi);
            if (P2 && projectile.timeLeft % 20 == 10)
            {
                int protmp = Projectile.NewProjectile(projectile.Center, projectile.rotation.ToRotationVector2() * 10, ProjectileID.CultistBossFireBall, projectile.damage, 0, owner.target);
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
            }
            Vector2 Line = Vector2.Normalize(projectile.Center - owner.Center);

            Dust dust = Main.dust[Dust.NewDust(owner.Center + Line * Main.rand.Next((int)(projectile.Center - owner.Center).Length()), 10, 10, 6, 0f, 0f, 100, default, 1f)];
            dust.noGravity = true;
            dust.scale = 1.7f;
            dust.fadeIn = 0.5f;
            dust.velocity *= 5f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            Vector2 Line = Vector2.Normalize(projectile.Center - owner.Center);
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/SolarFighterProj/SolarEruption1");
            Texture2D Tex3 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/SolarFighterProj/SolarEruption2");
            Color color27 = Color.White;
            color27.A /= 2;
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, Tex1.Size() * 0.5f, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Tex2, owner.Center + Line * 28 - Main.screenPosition, null, color27, projectile.rotation, Tex2.Size() * 0.5f, 1.0f, SpriteEffects.None, 0);
            for (int i = 56 + 15; i < (projectile.Center - owner.Center).Length() - 25; i += 29)
            {
                spriteBatch.Draw(Tex3, owner.Center + Line * i - Main.screenPosition, null, color27, projectile.rotation, Tex3.Size() * 0.5f, 1.0f, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, projectile.Center);

        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            if (Main.rand.Next(2) == 0)
            {
                int protmp = Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, projectile.damage, 0, Main.myPlayer);
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false;
            }
        }
    }
}