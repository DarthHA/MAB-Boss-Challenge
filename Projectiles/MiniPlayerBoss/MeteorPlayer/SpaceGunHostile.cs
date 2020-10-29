using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.Projectiles.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer
{
    public class SpaceGunHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Space Gun");
            DisplayName.AddTranslation(GameCulture.Chinese, "空间枪");
        }
        public override void SetDefaults()
        {
            projectile.width = 35;
            projectile.height = 20;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<MeteorPlayerBoss>())
            {
                projectile.Kill(); return;
            }
            Player target = Main.player[owner.target];
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center;
            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            if (projectile.spriteDirection > 0)
            {
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi / 2;
            }
            else
            {
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) - MathHelper.Pi / 2;
            }

            if (owner.ai[2] > 1)
            {
                if (owner.ai[2] % 12 == 2 && owner.ai[2] < 60 && owner.ai[2] > 12)
                {
                    float PredictIndex = 1 / 5;
                    if (owner.life < owner.lifeMax / 3 * 2) PredictIndex = 1 / 4;
                    if (owner.life < owner.lifeMax / 3) PredictIndex = 1 / 3;
                    if (Main.hardMode && MABWorld.DownedMeteorPlayer) PredictIndex = 1 / 2;
                    Main.PlaySound(SoundID.Item12, projectile.Center);
                    int protmp = Projectile.NewProjectile(projectile.Center, Facing * 25 + target.velocity * PredictIndex, ProjectileID.GreenLaser, projectile.damage, 0);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].tileCollide = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                }
                if (owner.ai[2] >= 99)
                {
                    projectile.Kill();
                    return;
                }
            }
            if (owner.ai[1] > 5)
            {
                projectile.Kill();
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (SP == SpriteEffects.None)
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation - MathHelper.Pi / 2, new Vector2(0, Tex.Height / 2), projectile.scale, SP, 0);

            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, lightColor, MathHelper.Pi / 2 * 3 + projectile.rotation, new Vector2(Tex.Width, Tex.Height / 2), projectile.scale, SP, 0);

            }
            return false;
        }

    }
}