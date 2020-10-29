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
    public class PhantasmHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
            DisplayName.AddTranslation(GameCulture.Chinese, "幻象弓");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 54;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 240;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0) projectile.Kill();
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<VortexRangerBoss>()) projectile.Kill();
            projectile.alpha = owner.alpha;
            Player target = Main.player[owner.target];
            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            projectile.Center = owner.Center;
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X);

            projectile.ai[1]++;
            if (projectile.ai[1] % 40 == 39)
            {
                Main.PlaySound(SoundID.Item5, projectile.position);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center + Facing * 25, (projectile.rotation + Main.rand.NextFloat() * MathHelper.Pi / 12 - MathHelper.Pi / 24).ToRotationVector2() * (6 + Main.rand.Next(5)) + Main.player[owner.target].velocity * 0.2f, ModContent.ProjectileType<LunarArrowHostile>(), projectile.damage, projectile.knockBack, Main.myPlayer);
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color27 = Color.White * projectile.Opacity;
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/VortexRangerProj/PhantasmGlow");
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, new Vector2(0, 27), 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Tex2, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(0, 27), 1, SpriteEffects.None, 0);

            if (projectile.ai[1] % 40 >= 39 || projectile.ai[1] % 40 <= 4)
            {
                int frameY = 0;
                if (projectile.ai[1] % 40 <= 4) frameY = (int)projectile.ai[1] % 40 + 1;
                Texture2D texture2D15 = Main.extraTexture[65];
                Main.spriteBatch.Draw(texture2D15, projectile.Center + projectile.rotation.ToRotationVector2() * 35f - Main.screenPosition, new Rectangle(0, 46 * frameY, 36, 46), Color.White * projectile.Opacity, projectile.rotation, new Vector2(18, 23), projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}