﻿using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class PlayerSummon3 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Celestial Rune(Nebula)");
            DisplayName.AddTranslation(GameCulture.Chinese, "残缺的天界符文(星云)");
            Tooltip.SetDefault("It seems to have only one gem on it\nSummon the Nebula Defender");
            Tooltip.AddTranslation(GameCulture.Chinese, "它看起来只镶嵌着一颗宝石\n召唤星云守护者");
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
                    line2.overrideColor = Color.MediumPurple;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
        public override bool CanUseItem(Player player)
        {
            return (!NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()) && MABWorld.DownedNebulaPlayer);
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                Main.PlaySound(SoundID.Roar, player.Center, 0);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NebulaMageBoss>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FragmentNebula, 10);

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}