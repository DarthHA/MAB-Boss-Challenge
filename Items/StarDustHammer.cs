using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace MABBossChallenge.Items
{
    public class StarDustHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test");
            DisplayName.AddTranslation(GameCulture.Chinese, "永恒星神");
            Tooltip.SetDefault("一把足以毁灭世界的神器");
        }

        public override void SetDefaults()
        {

            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = false;
            item.damage = 666;
            item.melee = true;
            item.crit = 666;
            item.pick = 666;
            item.axe = 666 / 5;
            item.hammer = 666;
            item.mana = 6;
        }



        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                bool IsBossAlive = false;

                foreach (NPC test in Main.npc)
                {
                    if (test.active && test.boss)
                    {
                        IsBossAlive = true;
                    }
                }
                if (!IsBossAlive)
                {

                    for (int i = 0; i <= NPCLoader.NPCCount; i++)
                    {
                        NPC n = new NPC();
                        n.SetDefaults(i);
                        if (n.boss)
                        {
                            NPC.NewNPC((int)player.Center.X, (int)player.Center.Y - 200, i);
                        }
                    }

                }
                else
                {
                    NPC.downedAncientCultist = false;
                    NPC.downedBoss1 = false;
                    NPC.downedBoss2 = false;
                    NPC.downedBoss3 = false;
                    NPC.downedFishron = false;
                    NPC.downedGolemBoss = false;
                    NPC.downedMechBoss1 = false;
                    NPC.downedMechBoss2 = false;
                    NPC.downedMechBoss3 = false;
                    NPC.downedMechBossAny = false;
                    NPC.downedMoonlord = false;
                    NPC.downedPlantBoss = false;
                    NPC.downedQueenBee = false;
                    NPC.downedSlimeKing = false;
                    NPC.downedTowerNebula = false;
                    NPC.downedTowerSolar = false;
                    NPC.downedTowerStardust = false;
                    NPC.downedTowerVortex = false;
                    MABWorld.DownedMeteorPlayer = false;
                    MABWorld.DownedMeteorPlayerEX = false;
                    MABWorld.DownedPreEvilFighter = false;
                    MABWorld.DownedPreEvilFighter2 = false;
                    MABWorld.DownedSolarPlayerEX = false;
                    MABWorld.DownedVortexPlayerEX = false;
                    MABWorld.DownedNebulaPlayer = false;
                    MABWorld.DownedSolarPlayer = false;
                    MABWorld.DownedStardustPlayer = false;
                    MABWorld.DownedVortexPlayer = false;

                }
            }
            return true;
        }
    }
}