using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MABBossChallenge.Buffs
{
    public class TimeDisort : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Time Disort");
            DisplayName.AddTranslation(GameCulture.Chinese, "时间扰乱");
            Description.SetDefault("The flow rate of time has increased");
            Description.AddTranslation(GameCulture.Chinese, "时间的流速增加了");
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            this.canBeCleared = false;
            Main.lightPet[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            this.longerExpertDebuff = true;
            Main.pvpBuff[Type] = true;
            Main.vanityPet[Type] = false;

        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Cyan;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MABWorld.CurrentTime = 2;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            if (player.buffTime[buffIndex] < 600)
            {
                player.buffTime[buffIndex] += time;
            }
            return false;
        }

    }
}