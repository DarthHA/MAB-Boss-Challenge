using MABBossChallenge.Tiles;
using MABBossChallenge.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class ArenaSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battlefield Generator");
            DisplayName.SetDefault("战场生成器");
            Tooltip.SetDefault("Generate a battlefield in the evils");
            Tooltip.AddTranslation(GameCulture.Chinese,"在世界的邪恶地形处生成一座战场！");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 1;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = true;
            item.autoReuse = false;
        }


        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                if (MABWorld.IsCreated)
                {
                    Main.NewText("该世界已经有一个战场了！", Color.Green);
                    return false;
                }
                else
                {
                    GenBattlefield();
                }
            }
            return true;
        }


        private void GenBattlefield()
        {
            bool flag = false;
            int x = 0, y = 0;
            int StoneType = WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
            int t = 0;
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
                t++;
                if (t > 5000 && !flag)
                {
                    Main.NewText("生成失败！，该世界可能缺少适合的邪恶地形！", Color.Red);
                    return;
                }
            }

            Texture2D tex = mod.GetTexture("Images/BattleFieldMap");
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
                    WorldGen.PlaceTile(x + _x, y + _y, WorldGen.crimson ? ModContent.TileType<CrimsonAltarSummon>() : ModContent.TileType<DemonAltarSummon>(), false, true);
                }
            }
            Main.NewText("已成功生成战场！", Color.Green);
            MABWorld.IsCreated = true;
        }


        private bool Valid(int x, int y)
        {
            return x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;
        }

    }
}