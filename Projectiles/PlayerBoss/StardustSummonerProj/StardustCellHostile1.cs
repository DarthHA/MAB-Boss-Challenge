using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustCellHostile1 : ModProjectile
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
            projectile.hostile = false;
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



        private void Movement(Vector2 targetPos, float speed)
        {
            Vector2 MoveVel = targetPos - projectile.Center;
            if (MoveVel.Length() > speed) MoveVel /= MoveVel.Length() / speed;
            projectile.velocity = MoveVel;
        }
        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>())) projectile.Kill();

            Player player = Main.player[projectile.owner];
            float Ro = projectile.ai[0] / 10f * MathHelper.Pi;
            Vector2 Dest = player.Center + ((float)(510 - projectile.timeLeft) / 510 * MathHelper.TwoPi * 2 + Ro - MathHelper.Pi / 3).ToRotationVector2() * 300;
            Movement(Dest, 25f);


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


            //SwarmAI();
            projectile.ai[1]++;
            if (projectile.ai[1] % 100 == 70)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Normalize(player.Center - projectile.Center) * 16, ModContent.ProjectileType<StardustCellHostile2>(), projectile.damage, 0, projectile.owner, 0);
            }
        }


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ImprovedCelledBuff>(), 240);
        }

        public void SwarmAI()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.Hitbox.Intersects(projectile.Hitbox) && proj.active && proj.type == projectile.type && proj.whoAmI != projectile.whoAmI)
                {
                    Vector2 AwayVel = proj.Center - projectile.Center;
                    AwayVel.Normalize();
                    projectile.Center -= AwayVel;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0, 0, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 3;
            }
        }

    }
}