using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Utils
{
    public static class NPCUtils
    {

        public static bool AnyBosses()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool InTemple(Vector2 Pos)
        {
            return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null &&
                        Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)].wall == WallID.LihzahrdBrickUnsafe;
        }

        public static bool AnyProj(int type)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AnyProj(int type, int owner)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type && proj.owner == owner)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool InWall(Vector2 Pos, int type)
        {
            return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null &&
                        Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)].wall == type;
        }

        public static bool TileCollision(Vector2 Position, int Width, int Height, int TileType)
        {
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + Height) / 16f) + 2;
            num = Terraria.Utils.Clamp(num, 0, Main.maxTilesX - 1);
            num2 = Terraria.Utils.Clamp(num2, 0, Main.maxTilesX - 1);
            num3 = Terraria.Utils.Clamp(num3, 0, Main.maxTilesY - 1);
            num4 = Terraria.Utils.Clamp(num4, 0, Main.maxTilesY - 1);
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && Main.tile[i, j].type == TileType)
                    {
                        Vector2 vector;
                        vector.X = i * 16;
                        vector.Y = j * 16;
                        int num5 = 16;
                        if (Main.tile[i, j].halfBrick())
                        {
                            vector.Y += 8f;
                            num5 -= 8;
                        }
                        if (Position.X + Width > vector.X && Position.X < vector.X + 16f && Position.Y + Height > vector.Y && Position.Y < vector.Y + num5)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static float SinEX(float x)
        {
            while (x >= MathHelper.TwoPi || x < 0)
            {
                if (x >= MathHelper.TwoPi) x -= MathHelper.TwoPi;
                if (x < 0) x += MathHelper.TwoPi;
            }
            if (x < MathHelper.Pi / 2)
            {
                return x / (MathHelper.Pi / 2);
            }
            if (x >= MathHelper.Pi / 2 && x < MathHelper.Pi)
            {
                return (MathHelper.Pi - x) / (MathHelper.Pi / 2);
            }
            if (x >= MathHelper.Pi && x < MathHelper.Pi / 2 * 3)
            {
                return (MathHelper.Pi - x) / (MathHelper.Pi / 2);
            }
            if (x >= MathHelper.Pi / 2 * 3)
            {
                return (x - MathHelper.TwoPi) / (MathHelper.Pi / 2);
            }
            return -114514;
        }

        public static Vector2 Rot(Vector2 vec, float r)
        {
            if (vec == Vector2.Zero) return vec;
            float R = (float)Math.Atan2(vec.Y, vec.X);
            R += r;
            return R.ToRotationVector2() * vec.Length();
        }

        public static Vector2 RotPos(Vector2 TargetPos, Vector2 CenterPos, float r)
        {
            return CenterPos + Rot(TargetPos - CenterPos, r);
        }

        public static bool BuffedEvilFighter()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.localAI[2] == 1)
                {
                    if (npc.type == ModContent.NPCType<ShadowPlayerBoss>() || npc.type == ModContent.NPCType<CrimsonPlayerBoss>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void Movement(NPC npc, Vector2 targetPos, float speedModifier, bool fastX = true, float XLimit = 24, float YLimit = 24)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * (fastX ? 2 : 1);
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > XLimit)
                npc.velocity.X = XLimit * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > YLimit)
                npc.velocity.Y = YLimit * Math.Sign(npc.velocity.Y);


        }


        public static void MovementX(NPC npc, float TargetX, float speedModifier, bool fastX = true, float XLimit = 24)
        {
            if (npc.Center.X < TargetX)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * (fastX ? 2 : 1);
            }

            if (Math.Abs(npc.velocity.X) > XLimit)
                npc.velocity.X = XLimit * Math.Sign(npc.velocity.X);

        }

        public static int HomeOnTarget(Projectile projectile, int Range)
        {

            float homingMaximumRangeInPixels = Range;
            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile))
                {
                    float distance = projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }

    }
}