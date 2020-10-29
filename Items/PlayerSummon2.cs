using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class PlayerSummon2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Celestial Rune(Vortex)");
            DisplayName.AddTranslation(GameCulture.Chinese, "残缺的天界符文(星璇)");
            Tooltip.SetDefault("It seems to have only one gem on it\nSummon the Vortex Defender");
            Tooltip.AddTranslation(GameCulture.Chinese, "它看起来只镶嵌着一颗宝石\n召唤星璇守护者");
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.maxStack = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = true;

        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = Color.MediumSeaGreen;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
        public override bool CanUseItem(Player player)
        {
            return (!NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()) && MABWorld.DownedVortexPlayer);
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                Main.PlaySound(SoundID.Roar, player.Center, 0);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<VortexRangerBoss>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}