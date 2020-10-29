using MABBossChallenge.NPCs;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer
{
    public class EZCannonMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Cannon");
            DisplayName.AddTranslation(GameCulture.Chinese, "星星炮");
        }
        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()                //r=900
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerBoss>()) || !NPC.AnyNPCs(ModContent.NPCType<MeteorHeadMinion>()))
            {
                projectile.Kill();
                return;
            }
            Vector2 SpawnPos = new Vector2(projectile.localAI[0], projectile.localAI[1]);
            Vector2 Facing = Vector2.Normalize(SpawnPos - projectile.Center);
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X);
            if (projectile.Distance(SpawnPos) < 880)
            {
                MoveToVector2(SpawnPos + projectile.ai[0].ToRotationVector2() * 900, 20);
            }
            else
            {
                projectile.velocity *= 0.8f;
                projectile.Center = Utils.NPCUtils.RotPos(projectile.Center, SpawnPos, MathHelper.Pi / 360);

                projectile.ai[1] = (projectile.ai[1] + 1) % 200;
                if (projectile.ai[1] % 5 == 1 && projectile.ai[1] > 100)
                {
                    int protmp = Projectile.NewProjectile(projectile.Center, projectile.rotation.ToRotationVector2() * 10, ModContent.ProjectileType<EZStarHostile>(), projectile.damage, 0);
                    Main.projectile[protmp].timeLeft = 180;
                }

            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);


            return false;
        }

        public void MoveToVector2(Vector2 p, float Vel)
        {
            float moveSpeed = Vel;
            float velMultiplier = 1f;
            Vector2 dist = p - projectile.Center;
            float length = (dist == Vector2.Zero) ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }
            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }
            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }
            projectile.velocity = ((length == 0f) ? Vector2.Zero : Vector2.Normalize(dist));
            projectile.velocity *= moveSpeed;
            projectile.velocity *= velMultiplier;
        }

    }
}