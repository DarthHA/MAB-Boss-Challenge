using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class ShadowSample : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Tissue");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影组织");
            Tooltip.SetDefault("Brothers always fight together!");
            Tooltip.AddTranslation(GameCulture.Chinese, "‘兄弟总是一同奋战！’");
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Red;
            item.maxStack = 1;
            item.consumable = false;
            item.value = 100;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ShadowScale);
            recipe.AddIngredient(ItemID.TissueSample);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(ModContent.ItemType<ShadowSample>());
            recipe.AddRecipe();
        }

    }
}