using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer
{
    public class EZCannonHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Cannon");
            DisplayName.AddTranslation(GameCulture.Chinese,"星星炮");
        }
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 15;
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
                projectile.Kill();
                return;
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

            if (owner.ai[1] == 4)
            {
                if (owner.ai[2] == 40)
                {
                    int diff = 0;
                    if (owner.life <= owner.lifeMax / 3 * 2) diff = 1;
                    if (owner.life < owner.lifeMax / 3) diff = 2;
                    if (Main.hardMode && MABWorld.DownedMeteorPlayer) diff = 2;
                    Projectile.NewProjectile(projectile.Center, Facing * 15, ModContent.ProjectileType<EZStarHostileS>(), projectile.damage, 0, default, diff);
                }
            }
            else
            {
                projectile.Kill();
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