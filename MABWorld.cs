using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.EchDestroyer;
using MABBossChallenge.Tiles;
using MABBossChallenge.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;


namespace MABBossChallenge
{
    public class MABWorld : ModWorld
    {
        public static float CurrentTime;
        public static int AcutalCurrentTime;
        public static bool AutoPause = true;
        public static bool EchActive = false;


        public static bool IsCreated;
        public static bool DownedMeteorPlayer;
        public static bool DownedMeteorPlayerEX;
        public static bool DownedPreEvilFighter2;
        public static bool DownedPreEvilFighter;
        public static bool DownedSolarPlayer;
        public static bool DownedVortexPlayer;
        public static bool DownedNebulaPlayer;
        public static bool DownedStardustPlayer;
        public static bool DownedEchDestroyer;
        public static bool DownedSolarPlayerEX;
        public static bool DownedVortexPlayerEX;
        public static bool DownedNebulaPlayerEX;
        public override TagCompound Save()
        {
            return new TagCompound {
            {"IsCreated", IsCreated},
            {"DownedMeteorPlayer", DownedMeteorPlayer},
            {"DownedMeteorPlayerEX", DownedMeteorPlayerEX},
            {"DownedPreEvilFighter2", DownedPreEvilFighter2},
            {"DownedPreEvilFighter", DownedPreEvilFighter},
            {"DownedSolarPlayer", DownedSolarPlayer},
            {"DownedVortexPlayer", DownedVortexPlayer},
            {"DownedNebulaPlayer", DownedNebulaPlayer},
            {"DownedStardustPlayer", DownedStardustPlayer},
            {"DownedEchDestroyer",DownedEchDestroyer },
            {"DownedSolarPlayerEX", DownedSolarPlayerEX},
            {"DownedVortexPlayerEX", DownedVortexPlayerEX},
            {"DownedNebulaPlayerEX", DownedNebulaPlayerEX},
            };
        }


        public override void Initialize()
        {
            IsCreated = false;
            DownedPreEvilFighter = false;
            DownedPreEvilFighter2 = false;
            DownedMeteorPlayer = false;
            DownedMeteorPlayerEX = false;
            DownedNebulaPlayer = false;
            DownedSolarPlayer = false;
            DownedStardustPlayer = false;
            DownedEchDestroyer = false;
            DownedVortexPlayer = false;
            DownedSolarPlayerEX = false;
            DownedNebulaPlayerEX = false;
        }
        public override void Load(TagCompound tag)
        {
            IsCreated = tag.GetBool("IsCreated");
            DownedMeteorPlayer = tag.GetBool("DownedMeteorPlayer");
            DownedMeteorPlayerEX = tag.GetBool("DownedMeteorPlayerEX");
            DownedPreEvilFighter = tag.GetBool("DownedPreEvilFighter");
            DownedPreEvilFighter2 = tag.GetBool("DownedPreEvilFighter2");
            DownedSolarPlayer = tag.GetBool("DownedSolarPlayer");
            DownedVortexPlayer = tag.GetBool("DownedVortexPlayer");
            DownedNebulaPlayer = tag.GetBool("DownedNebulaPlayer");
            DownedStardustPlayer = tag.GetBool("DownedStardustPlayer");
            DownedEchDestroyer = tag.GetBool("DownedEchDestroyer");
            DownedSolarPlayerEX = tag.GetBool("DownedSolarPlayerEX");
            DownedVortexPlayerEX = tag.GetBool("DownedVortexPlayerEX");
            DownedNebulaPlayerEX = tag.GetBool("DownedNebulaPlayerEX");

        }

        public override void PostUpdate()
        {
            Main.time += CurrentTime - 1;
            if (!Main.LocalPlayer.HasBuff(ModContent.BuffType<TimeDisort>()))
            {
                CurrentTime = 1;
            }
        }
        public override void PreUpdate()
        {
            if ((int)CurrentTime != CurrentTime)  
            {
                int b = (int)CurrentTime;
                if (Main.rand.NextFloat() <= CurrentTime - b) 
                {
                    AcutalCurrentTime = 1 + b;
                }
                else
                {
                    AcutalCurrentTime = b;
                }
            }
            else
            {
                AcutalCurrentTime = (int)CurrentTime;
            }

            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
                if (EchActive)
                {
                    EchActive = false;
                    Main.autoPause = AutoPause;
                }
            }
            else
            {
                if (!EchActive)
                {
                    AutoPause = Main.autoPause;
                    EchActive = true;
                }
                else
                {
                    Main.autoPause = false;
                }
            }

        }



        private void GenBattlefield(GenerationProgress progress)
        {
            progress.Message = "Generating Battlefield...";
            bool flag = false;
            int x = 0, y = 0;
            int StoneType = WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
            while (!flag)
            {
                x = WorldGen.genRand.Next(0, Main.maxTilesX);
                y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
                if (Main.tile[x, y].type == StoneType)
                {
                    if (Valid(x + 104, y + 44))
                    {
                        flag = true;
                        for (int i = 0; i < 44; i++)
                        {
                            for (int j = 0; j < 104; j++)
                            {
                                if (Main.tile[x + j, y + i].type == TileID.DemonAltar || Main.tile[x + j, y + i].type == TileID.ShadowOrbs ||
                                    Main.tile[x + j, y + i].type == TileID.Grass ||
                                    Main.tile[x + j, y + i].type == TileID.CorruptGrass || Main.tile[x + j, y + i].type == TileID.FleshGrass ||
                                    Main.tile[x + j, y + i].type == TileID.BlueDungeonBrick || Main.tile[x + j, y + i].type == TileID.GreenDungeonBrick || Main.tile[x + j, y + i].type == TileID.PinkDungeonBrick)
                                {
                                    flag = false;
                                }
                            }
                        }

                    }
                }
            }

            Texture2D tex = MABBossChallenge.Instance.GetTexture("Images/BattleFieldMap");
            Byte4[] data = new Byte4[tex.Width * tex.Height];
            tex.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                int _x = i % tex.Width;
                int _y = i / tex.Width;
                Main.tile[x + _x, y + _y].active(false);
                Main.tile[x + _x, y + _y].liquid = 0;
                Main.tile[x + _x, y + _y].wall = 0;
            }
            for (int i = 0; i < data.Length; i++)
            {
                Vector4 vec = data[i].ToVector4();
                Color ctmp = new Color(vec);
                int _x = i % tex.Width;
                int _y = i / tex.Width;

                if (ctmp == Color.Black)
                {
                    Main.tile[x + _x, y + _y].active(true);
                    Main.tile[x + _x, y + _y].type = (ushort)ModContent.TileType<ArenaTile>();
                    WorldGen.SquareTileFrame(x + _x, y + _y, true);
                }
                else if (ctmp == Color.White)
                {
                    Main.tile[x + _x, y + _y].active(false);
                    Main.tile[x + _x, y + _y].liquid = 0;
                }
                else if (ctmp == Color.Red)
                {
                    Main.tile[x + _x, y + _y].active(true);
                    Main.tile[x + _x, y + _y].type = TileID.Torches;
                }

                Main.tile[x + _x, y + _y].wall = (ushort)ModContent.WallType<ArenaWall>();
            }

            for (int i = 0; i < data.Length; i++)
            {
                Vector4 vec = data[i].ToVector4();
                Color ctmp = new Color(vec);
                int _x = i % tex.Width;
                int _y = i / tex.Width;
                if (ctmp == Color.Yellow)
                {
                    Main.tile[x + _x, y + _y + 1].slope(0);
                    Main.tile[x + _x + 1, y + _y + 1].slope(0);
                    Main.tile[x + _x + 2, y + _y + 1].slope(0);
                    WorldGen.PlaceTile(x + _x, y + _y, WorldGen.crimson ? ModContent.TileType<CrimsonAltarSummon>() : ModContent.TileType<DemonAltarSummon>(), true, true);
                }
            }
            IsCreated = true;
        }

        public override void ModifyHardmodeTasks(List<GenPass> list)
        {
            //int Index = list.FindIndex((GenPass match) => match.Name == "Hardmode Good");
            //list.RemoveAt(Index);
            //Index = list.FindIndex((GenPass match) => match.Name == "Hardmode Evil");
            //list.RemoveAt(Index);
            //Index = list.FindIndex((GenPass match) => match.Name == "Hardmode Walls");
            //list.RemoveAt(Index);
            //Main.NewText("蛤?你再说一遍？");

        }

        private bool Valid(int x, int y)
        {
            return x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int Index = tasks.FindIndex((GenPass match) => match.Name == "Final Cleanup");
            if (Index != -1)
            {
                tasks.Insert(Index + 1, new PassLegacy("MABBossChallenge:GenBattleField", GenBattlefield));
            }

        }

    }
}