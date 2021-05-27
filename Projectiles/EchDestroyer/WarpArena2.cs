using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class WarpArena2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arena Calibration Point");
            DisplayName.AddTranslation(GameCulture.Chinese, "场地校准点");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.hostile = false;
            projectile.alpha = 255;
            projectile.scale = 1f;
            projectile.timeLeft = 9999999;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (projectile.ai[1] > 0)
            {
                Drag();
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()) && projectile.ai[1] > 0) 
            {
                projectile.ai[1] = -41; 
            }
            if (!ShouldAlive() && projectile.ai[1] > 0) 
            {
                projectile.ai[1] = -41;
            }
            projectile.velocity = Vector2.Zero;
            if (projectile.ai[1] <= 41)
            {
                projectile.ai[1]++;
            }
            if (projectile.ai[1] == -1) 
            {
                projectile.Kill();
                return;
            }

        }

        public void Drag()
        {
            foreach(Player player in Main.player)
            {
                if (player.Distance(projectile.Center) > 680 && !player.dead && player.active)
                {
                    Vector2 DragVel = Vector2.Normalize(projectile.Center - player.Center);
                    player.velocity += DragVel;
                    player.position += DragVel * 10;
                    player.controlDown = false;
                    player.controlHook = false;
                    player.controlJump = false;
                    player.controlLeft = false;
                    player.controlMount = false;
                    player.controlRight = false;
                    player.controlThrow = false;
                    player.controlUp = false;
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                    if (player.mount.Active)
                    {
                        player.mount.Dismount(player);
                    }
                    if (player.Distance(projectile.Center) > 680)
                    {
                        player.Center = projectile.Center + Vector2.Normalize(player.Center - projectile.Center) * 679;
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float r = projectile.ai[1] * 14f;
            if (projectile.ai[1] < 40 && projectile.ai[1] >= 0)
            {
                r = projectile.ai[1] * 17;
            }
            if (projectile.ai[1] >= 40) r = 680;
            if (projectile.ai[1] < 0)
            {
                r = (-projectile.ai[1] - 1) * 17;
            }
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 750)
            {
                spriteBatch.Draw(Main.magicPixel, projectile.Center + i.ToRotationVector2() * r - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.White, i, Vector2.Zero, 3, SpriteEffects.None, 0);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public bool ShouldAlive()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
                return false;
            }
            NPC head = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<EchDestroyerHead>())];
            if((head.modNPC as EchDestroyerHead).AIState == (int)EchDestroyerHead.WrapAIState.FlashBack)
            {
                return true;
            }
            return false;
        }
    }
}