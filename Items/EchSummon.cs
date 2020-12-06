using MABBossChallenge.NPCs.EchDestroyer;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class EchSummon : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warp Worm");
            DisplayName.AddTranslation(GameCulture.Chinese, "跃迁蠕虫");
            Tooltip.SetDefault("Summons the Warp Destroyer");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤跃迁毁灭者");
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
        }
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.maxStack = 1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = false;
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

        public override bool CanUseItem(Player player)
        {
            return !Utils.NPCUtils.AnyBosses();
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                Main.PlaySound(SoundID.Roar, player.Center, 0);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<EchDestroyerHead>());

            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.Nanites, 300);
            recipe.AddIngredient(ItemID.MechanicalWorm, 1);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

    }
}