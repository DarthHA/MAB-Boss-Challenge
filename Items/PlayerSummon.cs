﻿using MABBossChallenge.NPCs.PlayerBoss;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class PlayerSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Rune MK2");
            DisplayName.AddTranslation(GameCulture.Chinese,"天界符文MK2");
            Tooltip.SetDefault("Summon the four defenders at the same time\nUse at your own risk");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤月下四柱男\n后果自负");
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
                    line2.overrideColor = Main.DiscoColor;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
        public override bool CanUseItem(Player player)
        {
            if (!MABWorld.DownedSolarPlayer || !MABWorld.DownedVortexPlayer || !MABWorld.DownedNebulaPlayer || !MABWorld.DownedStardustPlayer)
            {
                return false;
            }
            return (!NPC.AnyNPCs(ModContent.NPCType<SolarFighterBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()));
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                Main.PlaySound(SoundID.Roar, player.Center, 0);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SolarFighterBoss>());
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<VortexRangerBoss>());
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NebulaMageBoss>());
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<StardustSummonerBoss>());
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PlayerSummon1>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PlayerSummon2>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PlayerSummon3>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PlayerSummon4>(), 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}