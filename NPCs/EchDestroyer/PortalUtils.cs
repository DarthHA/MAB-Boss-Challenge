using MABBossChallenge.Projectiles;
using MABBossChallenge.Projectiles.EchDestroyer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.EchDestroyer
{
    public static class PortalUtils
    {
        public static int FindHoleByNum(int num)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<WormHoleEX>() && proj.timeLeft > 120) 
                {
                    if (num == 0)
                    {
                        return proj.whoAmI;
                    }
                    else
                    {
                        num--;
                    }
                }
            }
            return -1;
        }



        public static int HoleCount()
        {
            int num = 0;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<WormHoleEX>() && proj.timeLeft > 120) 
                {
                    num++;
                }
            }
            return num;
        }

        public static float GetHoleA(Vector2 Pos)
        {
            float result = 1;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<WormHoleEX>())
                {
                    if (proj.Distance(Pos) < 60)
                    {
                        result = proj.Distance(Pos) / 60;
                    }
                }
            }
            return result;
        }

        public static void DirectMovement(this ModNPC modnpc, Vector2 TargetPos, float Vel)
        {
            Vector2 MoveVel = TargetPos - modnpc.npc.Center;
            if (MoveVel.Length() < Vel)
            {
                modnpc.npc.velocity = MoveVel;
            }
            else
            {
                modnpc.npc.velocity = Vector2.Normalize(MoveVel) * Vel;
            }
        }

        public static Vector2 GetRandomUnit()
        {
            return (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2();
        }


        public static void DirectMovementSeg(this ModNPC modnpc, Vector2 TargetPos, float Vel)
        {
            Vector2 MoveVel = TargetPos - modnpc.npc.Center;
            if (MoveVel.Length() < Vel)
            {
                modnpc.npc.position = TargetPos;
            }
            else
            {
                modnpc.npc.position += Vector2.Normalize(MoveVel) * Vel;
            }
        }


        public static List<int> ProjList = new List<int>
        { 
            ModContent.ProjectileType<WormHoleEX>(),
            ModContent.ProjectileType<WormHoleEXFake>(),
            ModContent.ProjectileType<PortalBolt>(),
            ModContent.ProjectileType<WarpDeathray>(),
            ModContent.ProjectileType<WarpArena>(),
            ModContent.ProjectileType<WarpBolt>(),
            ModContent.ProjectileType<WarpLaser>(),
            ModContent.ProjectileType<WarpSphere>(),
        };

        public static List<int> NPCList = new List<int>
        {
            ModContent.NPCType<EchDestroyerBody>(),
            ModContent.NPCType<EchDestroyerHead>(),
            ModContent.NPCType<EchDestroyerTail>()
        };

        public static List<int> DustList = new List<int>
        {
            229,
            240
        };

    }


}