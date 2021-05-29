using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class RamMarker : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ram Marker");
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
        }

        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
                projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }


        public static Vector2 GetMarkPos()
        {
            int MaxNum = -1;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<RamMarker>())
                {
                    if (MaxNum == -1)
                    {
                        MaxNum = proj.whoAmI;
                    }
                    else
                    {
                        if (proj.ai[1] < Main.projectile[MaxNum].ai[1])
                        {
                            MaxNum = proj.whoAmI;
                        }
                    }

                }
            }
            if (MaxNum != -1)
            {
                return Main.projectile[MaxNum].Center;
            }
            return Vector2.Zero;
        }

        public static void KillMark()
        {
            int MaxNum = -1;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<RamMarker>())
                {
                    if (MaxNum == -1)
                    {
                        MaxNum = proj.whoAmI;
                    }
                    else
                    {
                        if (proj.ai[1] < Main.projectile[MaxNum].ai[1])
                        {
                            MaxNum = proj.whoAmI;
                        }
                    }

                }
            }
            if (MaxNum != -1)
            {
                Main.projectile[MaxNum].Kill();
            }
        }

        public static void SummonMark(Vector2 Pos)
        {
            int num = 0;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<RamMarker>())
                {
                    if (proj.ai[1] > num)
                    {
                        num = (int)proj.ai[1];
                    }
                }
            }
            num++;
            Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<RamMarker>(), 0, 0, default, 0, num);

        }


    }
}