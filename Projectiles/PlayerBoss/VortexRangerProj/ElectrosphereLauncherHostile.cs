using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class ElectrosphereLauncherHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrosphere Launcher");
            DisplayName.AddTranslation(GameCulture.Chinese, "线圈发射器");
        }
        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 32;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 260;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<VortexRangerBoss>())
            {
                projectile.Kill();
                return;
            }
            projectile.alpha = owner.alpha;
            Player target = Main.player[owner.target];
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center + new Vector2(projectile.spriteDirection * 2, 0);



            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            if (projectile.spriteDirection > 0)
            {
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X);
            }
            else
            {
                projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi;
            }

            projectile.ai[1]++;
            if (projectile.ai[1] >= 60)
            {
                if (projectile.ai[1] % 50 == 11)
                {
                    Vector2 ShootVel = Vector2.Normalize(target.Center - projectile.Center) * 20;
                    float R = (float)Math.Atan2(ShootVel.Y, ShootVel.X);
                    Main.PlaySound(SoundID.Item92, projectile.position);
                    Projectile.NewProjectile(projectile.Center, R.ToRotationVector2() * 20, ModContent.ProjectileType<BulletCenter>(), projectile.damage, 0, target.whoAmI, (target.Center - projectile.Center).Length() / 20 + 3);
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color27 = Color.White * projectile.Opacity;
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (SP == SpriteEffects.None)
            {
                spriteBatch.Draw(Tex, projectile.position + new Vector2(20, 25) - Main.screenPosition, null, color27, projectile.rotation, new Vector2(20, 25), 1, SP, 0);

            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spriteBatch.Draw(Tex, projectile.position + new Vector2(54, 25) - Main.screenPosition, null, color27, projectile.rotation, new Vector2(54, 25), 1, SP, 0);

            }
            return false;
        }
    }
}