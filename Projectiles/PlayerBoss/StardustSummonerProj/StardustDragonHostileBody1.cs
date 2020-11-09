using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustDragonHostileBody1 : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Dragon");
            DisplayName.AddTranslation(GameCulture.Chinese, "星尘龙");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.penetrate = -1;
            projectile.timeLeft = 440;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.netImportant = true;
            projectile.hide = true;
            cooldownSlot = 1;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles,
            List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), null,
                projectile.GetAlpha(Color.White), projectile.rotation, texture2D13.Size() / 2, projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>())) projectile.Kill();

            int num1038 = 30;

            bool flag67 = false;
            Vector2 value67 = Vector2.Zero;
            float num1052 = 0f;
            if (projectile.ai[1] == 1f)
            {
                projectile.ai[1] = 0f;
            }

            int byUUID = Projectile.GetByUUID(projectile.owner, (int)projectile.ai[0]);
            if (byUUID >= 0 && Main.projectile[byUUID].active)
            {
                flag67 = true;
                value67 = Main.projectile[byUUID].Center;
                num1052 = Main.projectile[byUUID].rotation;
                Main.projectile[byUUID].localAI[0] = projectile.localAI[0] + 1f;
                if (Main.projectile[byUUID].type != ModContent.ProjectileType<StardustDragonHostileHead>()) Main.projectile[byUUID].localAI[1] = projectile.whoAmI;
            }

            if (!flag67) return;

            projectile.alpha -= 42;
            if (projectile.alpha < 0) projectile.alpha = 0;
            projectile.velocity = Vector2.Zero;
            Vector2 vector134 = value67 - projectile.Center;
            if (num1052 != projectile.rotation)
            {
                float num1056 = MathHelper.WrapAngle(num1052 - projectile.rotation);
                vector134 = vector134.RotatedBy(num1056 * 0.1f, default);
            }

            projectile.rotation = vector134.ToRotation() + MathHelper.Pi / 2;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = (int)(num1038 * projectile.scale);
            projectile.Center = projectile.position;
            if (vector134 != Vector2.Zero) projectile.Center = value67 - Vector2.Normalize(vector134) * 12;
            projectile.spriteDirection = vector134.X > 0f ? 1 : -1;
        }

        public override void Kill(int timeLeft)
        {
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, -projectile.velocity.X * 0.2f,
                -projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 2f;
            dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, MyDustId.BlueTrans, -projectile.velocity.X * 0.2f,
                -projectile.velocity.Y * 0.2f, 100);
            Main.dust[dust].velocity *= 2f;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}