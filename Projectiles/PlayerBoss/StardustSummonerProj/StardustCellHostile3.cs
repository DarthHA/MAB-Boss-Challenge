using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustCellHostile3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Cell");
            DisplayName.AddTranslation(GameCulture.Chinese, "星尘细胞");
        }
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 510;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            Main.projFrames[projectile.type] = 4;
            projectile.netImportant = true;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/StardustSummonerProj/StardustCellGlow1");
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3) projectile.frame = 0;
            }
            Rectangle Frame = new Rectangle(0, 24 * projectile.frame, 24, 24);
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, Frame, Color.White, 0, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(Tex2, projectile.Center - Main.screenPosition, Frame, Color.White, 0, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);


            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, bool fastX = true)
        {
            if (projectile.Center.X < targetPos.X)
            {
                projectile.velocity.X += speedModifier;
                if (projectile.velocity.X < 0)
                    projectile.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                projectile.velocity.X -= speedModifier;
                if (projectile.velocity.X > 0)
                    projectile.velocity.X -= speedModifier * (fastX ? 2 : 1);
            }
            if (projectile.Center.Y < targetPos.Y)
            {
                projectile.velocity.Y += speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                projectile.velocity.Y -= speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(projectile.velocity.X) > 24)
                projectile.velocity.X = 24 * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > 24)
                projectile.velocity.Y = 24 * Math.Sign(projectile.velocity.Y);


        }


        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>())) projectile.Kill();

            projectile.ai[1]++;
            float Ro = MathHelper.TwoPi / 3 * projectile.ai[0];
            Vector2 Dest = Main.player[projectile.owner].Center + Ro.ToRotationVector2() * 500;
            Movement(Dest, 0.5f, true);

            if (projectile.ai[1] >= 15 && (projectile.Center - Dest).Length() > 300)
            {
                float x, y;
                x = projectile.Center.X;
                y = projectile.Center.Y;
                projectile.Center = Dest;//Vector2.Normalize(projectile.Center - player.Center) * 200;
                Vector2 Len = (projectile.Center - new Vector2(x, y));
                for (float i = 0; i <= Len.Length(); i += 5)
                {
                    int dust = Dust.NewDust(new Vector2(x, y) + Vector2.Normalize(Len) * i, 0, 0, 229, 0, 0, 0, default, i / Len.Length() * 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                    Main.dust[dust].velocity = Vector2.Zero;

                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ImprovedCelledBuff>(), 240);
        }


        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0, 0, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 3;
            }
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 4)
            {
                Projectile.NewProjectile(projectile.Center, i.ToRotationVector2() * 8, ModContent.ProjectileType<StardustCellHostile2>(), projectile.damage, 0, projectile.owner, 1);
            }
        }

    }
}