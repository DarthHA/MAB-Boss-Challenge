using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class TPCircleIndicator : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("TP Circle Indicator");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 0;
            projectile.timeLeft = 9999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }
        public override void AI()
        {
            if (PortalUtils.FindHoleByNum((int)projectile.ai[0] + 1) == -1)
            {
                projectile.Kill();
                return;
            }
            projectile.localAI[0]++;
            if (projectile.localAI[0] > 30)
            {
                projectile.Kill();
                return;
            }
        }


        

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (PortalUtils.FindHoleByNum((int)projectile.ai[0] + 1) == -1)
            { 
                return false;
            }
            Vector2 A = Main.projectile[PortalUtils.FindHoleByNum((int)projectile.ai[0])].Center;
            Vector2 B = Main.projectile[PortalUtils.FindHoleByNum((int)projectile.ai[0] + 1)].Center;
            if (Vector2.Distance(A, B) < 10)
            {
                return false;
            }
            float k = (float)Math.Sin(projectile.localAI[0] / 30 * MathHelper.Pi);
            Terraria.Utils.DrawLine(spriteBatch, A, B, Color.Cyan * k, Color.White * k, 2);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}