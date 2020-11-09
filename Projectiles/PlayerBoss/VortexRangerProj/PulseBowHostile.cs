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
    public class PulseBowHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Bow");
            DisplayName.AddTranslation(GameCulture.Chinese, "脉冲弓");
        }
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 46;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 300;
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
            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            projectile.Center = owner.Center;
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X);

            projectile.ai[1]++;
            if (projectile.ai[1] > 30)
            {
                if (projectile.ai[1] % 20 == 11)
                {
                    Main.PlaySound(SoundID.Item75, projectile.position);
                    Projectile.NewProjectile(projectile.Center, projectile.rotation.ToRotationVector2() * 12, ModContent.ProjectileType<PulseProjHostile>(), projectile.damage, 0, owner.target);
                }
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color27 = Color.White * projectile.Opacity;
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, new Vector2(0, 23), 1, SpriteEffects.None, 0);
            return false;
        }
    }
}