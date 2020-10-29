using MABBossChallenge.NPCs.PlayerBoss;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Items
{
    public class PlayerSummonEX : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Rune MK2 EX");
            DisplayName.AddTranslation(GameCulture.Chinese, "天界符文MK2 EX");
            Tooltip.SetDefault("Summon the four awakened defenders at the same time\nAlthough by now there are only two lol\nNot consumed");
            Tooltip.AddTranslation(GameCulture.Chinese, "直接召唤觉醒的月下四柱男\n尽管目前只有两个（笑）\n该物品不消耗");
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
        }

        public override void SetDefaults()
        {
            item.width = 20;
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
                    line2.overrideColor = Main.DiscoColor;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
        public override bool CanUseItem(Player player)
        {
            return (!NPC.AnyNPCs(ModContent.NPCType<SolarFighterBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()));
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                Main.PlaySound(SoundID.Roar, player.Center, 0);
                Main.NewText("觉醒守护者们已苏醒！", 175, 75, 255);
                NPC.NewNPC((int)player.Center.X + 200, (int)player.Center.Y - 200, ModContent.NPCType<SolarFighterBoss>(), default, 4, default, default, default, default);
                NPC.NewNPC((int)player.Center.X - 200, (int)player.Center.Y - 200, ModContent.NPCType<VortexRangerBoss>(), default, 4, default, default, default, default);
            }
            return true;
        }

    }
}