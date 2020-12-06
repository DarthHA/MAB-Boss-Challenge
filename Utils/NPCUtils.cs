using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Utils
{

    public static class NPCUtils
    {
        /// <summary>
        /// 世界里是否有Boss存活
        /// </summary>
        /// <returns></returns>
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



        /// <summary>
        /// 是否存在指定类型射弹
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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




        /// <summary>
        /// 判断该世界坐标是否在指定墙背后，type为空时判断是否在墙后
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool InWall(Vector2 Pos, int? type = null)
        {
            if (type == null)
            {
                return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null;
            }
            else
            {
                return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null &&
                            Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)].wall == (int)type;
            }
        }


        /// <summary>
        /// 判断该世界坐标是否与指定物块发生碰撞
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="TileType"></param>
        /// <returns></returns>
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

        public static bool AnyPillarBosses()
        {
            return NPC.AnyNPCs(ModContent.NPCType<SolarFighterBoss>()) || NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()) ||
    NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()) || NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>());
        }


        public static bool AnyThisModBosses()
        {
            foreach(NPC npc in Main.npc)
            {
                if (npc.active)
                {
                    if (npc.modNPC != null)
                    {
                        if (npc.modNPC.mod == MABBossChallenge.Instance)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}