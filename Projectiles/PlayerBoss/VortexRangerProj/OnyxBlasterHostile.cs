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
    public class OnyxBlasterHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Blaster");
            DisplayName.AddTranslation(GameCulture.Chinese, "玛瑙爆破枪");
        }
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 26;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 359;
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
            projectile.Center = owner.Center + new Vector2(projectile.spriteDirection * 17, 0);
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
            if (projectile.ai[1] % 40 == 0)
            {
                Main.PlaySound(SoundID.Item36, projectile.position);
                float FacingR = (float)Math.Atan2(Facing.Y, Facing.X);
                int protmp = Projectile.NewProjectile(projectile.Center, Facing * 20, ProjectileID.BlackBolt, projectile.damage, 0, default);
                Main.projectile[protmp].hostile = true;
                Main.projectile[protmp].friendly = false; ;
                Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                Main.projectile[protmp].tileCollide = false;

                for (int i = 0; i < 3; i++)
                {
                    protmp = Projectile.NewProjectile(projectile.Center, (FacingR + MathHelper.Pi / 9 * Main.rand.NextFloat() - MathHelper.Pi / 18).ToRotationVector2() * 15, ProjectileID.IchorBullet, (int)(projectile.damage * 0.9), 0, default);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false; ;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                    Main.projectile[protmp].tileCollide = false;
                    Main.projectile[protmp].scale = 2;
                    protmp = Projectile.NewProjectile(projectile.Center, (FacingR + MathHelper.Pi / 8 * Main.rand.NextFloat() - MathHelper.Pi / 16).ToRotationVector2() * 15, ProjectileID.CursedBullet, (int)(projectile.damage * 0.9), 0, default);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false; ;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                    Main.projectile[protmp].tileCollide = false;
                    Main.projectile[protmp].scale = 2;
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
                spriteBatch.Draw(Tex, projectile.position + new Vector2(10, 10) - Main.screenPosition, null, color27, projectile.rotation, new Vector2(10, 10), 1, SP, 0);
            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spriteBatch.Draw(Tex, projectile.position + new Vector2(50, 10) - Main.screenPosition, null, color27, projectile.rotation, new Vector2(50, 10), 1, SP, 0);
            }
            return false;
        }


    }
}