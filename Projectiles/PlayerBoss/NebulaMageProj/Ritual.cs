using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class Ritual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Ritual");
            DisplayName.AddTranslation(GameCulture.Chinese, "星云仪式");
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
            projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }
        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<NebulaMageBoss>())
            {
                projectile.ai[1] = 1;
            }
            projectile.Center = Main.npc[(int)projectile.ai[0]].Center;
            projectile.rotation += 0.05f;
            projectile.localAI[0] = (projectile.localAI[0] + 1) % 30;

            if (projectile.ai[1] == 1)
            {
                projectile.scale -= 0.05f;
                if (projectile.scale < 0.1f)
                {
                    projectile.Kill();
                }
            }
            else
            {
                if (projectile.scale < 1)
                {
                    projectile.scale += 0.05f;
                }

            }

            foreach (Player player in Main.player)
            {
                if(player.active && !player.dead)
                {
                    if (player.Distance(projectile.Center) > 1050)
                    {
                        player.position += Vector2.Normalize(projectile.Center - player.Center) * 15;
                        player.velocity = Vector2.Normalize(projectile.Center - player.Center) * 15;
                    }
                }
            }
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Color.MediumPurple;
            color25 *= 0.2f;
            Vector2 ScreenPos = projectile.Center - Main.screenPosition;
            int R = (int)(2100 * projectile.scale);
            if (projectile.active)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                //float num146 = (float)Math.Sin(projectile.localAI[0] / 30 * MathHelper.TwoPi) / 2 + 0.5f;
                //DrawData value10 = new DrawData(TextureManager.Load("Images/Misc/Perlin"), ScreenPos, new Rectangle?(new Rectangle(0, 0, R * 2, R * 2)), color25, projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                DrawData value10 = new DrawData(Main.magicPixel, ScreenPos, new Rectangle?(new Rectangle(0, 0, R * 2, R * 2)), color25, 0, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(new Vector3(2));
                GameShaders.Misc["ForceField"].Apply(new DrawData?(value10));
                value10.Draw(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                return;
            }
        }


        public override bool CanDamage()
        {
            return false;
        }
    }
}