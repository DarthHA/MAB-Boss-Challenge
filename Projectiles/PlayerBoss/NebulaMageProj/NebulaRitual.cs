using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class NebulaRitual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 0.1f;
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
            if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<NebulaMageBoss>()) projectile.Kill();
            projectile.Center = Main.npc[(int)projectile.ai[0]].Center;
            projectile.rotation += 0.05f;
            projectile.localAI[0]++;
            if (projectile.localAI[0] < 30)
            {
                projectile.ai[1]++;
            }
            if (projectile.localAI[0] > 190)
            {
                projectile.ai[1]--;
            }
            if (projectile.ai[1] < 0) projectile.ai[1] = 0;
            projectile.scale = projectile.ai[1] / 30 * 0.8f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool CanDamage()
        {
            return false;
        }
    }
}