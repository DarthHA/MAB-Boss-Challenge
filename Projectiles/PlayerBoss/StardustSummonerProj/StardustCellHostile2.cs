using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustCellHostile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smaller Stardust Cell");
            DisplayName.AddTranslation(GameCulture.Chinese, "小星尘细胞");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;

            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>())) projectile.Kill();

            if (projectile.ai[0] == 0)
            {

                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = Main.player[projectile.owner].Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.03f));
                if (projectile.timeLeft <= 150) projectile.Kill();
            }
            if (projectile.ai[0] == 1)
            {
                /*
                if (projectile.width == 18 && projectile.height == 18)
                {
                    projectile.scale = 1.5f;
                    projectile.Center -= new Vector2(9, 9);
                    projectile.width = 27;
                    projectile.height = 27;
                }
                */
                if (projectile.scale == 1)
                {
                    projectile.scale = 1.3f;
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, -projectile.velocity.X * 0.2f,
-projectile.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 2f;
                }
                if (projectile.timeLeft >= 120)
                {
                    projectile.velocity *= 0.95f;
                }
                if (projectile.timeLeft == 120)
                {
                    projectile.velocity = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * 20 + Main.projectile[projectile.owner].velocity * 0.7f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ImprovedCelledBuff>(), 240);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ImprovedCelledBuff>(), 240);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D Tex2 = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/StardustSummonerProj/StardustCellGlow2");
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3) projectile.frame = 0;
            }
            Rectangle Frame = new Rectangle(0, 18 * projectile.frame, 18, 18);
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, Frame, Color.White, 0, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(Tex2, projectile.Center - Main.screenPosition, Frame, Color.White, 0, Frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);


            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}