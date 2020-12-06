using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;


namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class SolarArena : ModProjectile
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.5f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
            if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<SolarFighterBoss>()) projectile.Kill();
            //projectile.Center = Main.npc[(int)projectile.ai[0]].Center;
            projectile.ai[1] = (projectile.ai[1] + 1) % 320;
            projectile.rotation += 0.1f;
            if (projectile.localAI[0] < 60)
            {
                projectile.localAI[0]++;
            }
            projectile.scale = 4f / 60 * projectile.localAI[0];


            foreach (Player player in Main.player)
            {
                if ((player.Center - projectile.Center).Length() > 1000 && (player.Center - projectile.Center).Length() < 5000)
                {
                    Vector2 DragVel = Vector2.Normalize(projectile.Center - player.Center);
                    player.velocity = DragVel * 10;
                    player.position += DragVel * 10;
                    player.controlJump = false;
                    player.controlHook = false;
                    player.controlMount = false;
                    player.controlUseItem = false;
                    player.controlLeft = false;
                    player.controlRight = false;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 8)
            {
                float r = projectile.ai[1] / 320 * MathHelper.TwoPi + i;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White * 0.2f, projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
                r += MathHelper.Pi / 100;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White * 0.4f, projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
                r += MathHelper.Pi / 100;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White, projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            }
            for (float i = MathHelper.Pi / 16; i <= MathHelper.Pi / 16 * 33; i += MathHelper.Pi / 8)
            {
                float r = projectile.ai[1] / 320 * MathHelper.TwoPi + i;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White * 0.2f, -projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
                r += MathHelper.Pi / 100;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White * 0.4f, -projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
                r += MathHelper.Pi / 100;
                spriteBatch.Draw(Tex, projectile.Center + r.ToRotationVector2() * 50 / 3 * projectile.localAI[0] - Main.screenPosition, null, Color.White, -projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }


        public override void Kill(int timeLeft)
        {
            for (int num15 = 0; num15 < 16; num15++)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity *= 5f;
                //dust.shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.ArmorSetDye(), Main.LocalPlayer);
            }
        }
    }
}