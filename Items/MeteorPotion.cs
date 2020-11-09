using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    class MeteorPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Potion");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨石药水");
            Tooltip.SetDefault("Land a meteorite instantly when lacking meteor");
            Tooltip.AddTranslation(GameCulture.Chinese, "当世界缺少陨石时可以立刻降下一颗陨石！");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.consumable = true;
            item.maxStack = 1;
            item.useAnimation = 40;
            item.useTime = 40;
            item.value = 50000;

        }

        public override bool UseItem(Player player)
        {
            bool flag = true;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    flag = false;
                    break;
                }
            }
            int num = 0;
            float num2 = Main.maxTilesX / 4200;
            int num3 = (int)(400f * num2);
            for (int j = 5; j < Main.maxTilesX - 5; j++)
            {
                int num4 = 5;
                while (num4 < Main.worldSurface)
                {
                    if (Main.tile[j, num4].active() && Main.tile[j, num4].type == 37)
                    {
                        num++;
                        if (num > num3)
                        {
                            return false;
                        }
                    }
                    num4++;
                }
            }
            float num5 = 600f;
            while (!flag)
            {
                float num6 = Main.maxTilesX * 0.08f;
                int num7 = Main.rand.Next(150, Main.maxTilesX - 150);
                while (num7 > Main.spawnTileX - num6 && num7 < Main.spawnTileX + num6)
                {
                    num7 = Main.rand.Next(150, Main.maxTilesX - 150);
                }
                int k = (int)(Main.worldSurface * 0.3);
                while (k < Main.maxTilesY)
                {
                    if (Main.tile[num7, k].active() && Main.tileSolid[Main.tile[num7, k].type])
                    {
                        int num8 = 0;
                        int num9 = 15;
                        for (int l = num7 - num9; l < num7 + num9; l++)
                        {
                            for (int m = k - num9; m < k + num9; m++)
                            {
                                if (WorldGen.SolidTile(l, m))
                                {
                                    num8++;
                                    if (Main.tile[l, m].type == 189 || Main.tile[l, m].type == 202)
                                    {
                                        num8 -= 100;
                                    }
                                }
                                else if (Main.tile[l, m].liquid > 0)
                                {
                                    num8--;
                                }
                            }
                        }
                        if (num8 < num5)
                        {
                            num5 -= 0.5f;
                            break;
                        }
                        flag = WorldGen.meteor(num7, k);
                        if (flag)
                        {
                            break;
                        }
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
                if (num5 < 100f)
                {
                    return false;
                }
            }
            return true;
        }

    }
}