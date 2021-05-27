using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.EchDestroyer
{
    public class PlayerMark : ModProjectile
    {
        Player DrawPlayer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Player Mark");
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
            if (projectile.ai[0] == 0)
            {
                DrawPlayer = (Player)Main.LocalPlayer.Clone();
                projectile.ai[0] = 1;
            }
        }


        public static Vector2 GetMarkPos()
        {
            int MaxNum = -1;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<PlayerMark>())
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
                if (proj.active && proj.type == ModContent.ProjectileType<PlayerMark>())
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
            foreach(Projectile proj in Main.projectile)
            {
                if(proj.active && proj.type == ModContent.ProjectileType<PlayerMark>())
                {
                    if (proj.ai[1] > num)
                    {
                        num = (int)proj.ai[1];
                    }
                }
            }
            num++;
            Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<PlayerMark>(), 0, 0, default, 0, num);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] == 1)
            {
                Main.instance.DrawPlayer(DrawPlayer, projectile.Center, 0, Vector2.Zero, 0);
            }
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}