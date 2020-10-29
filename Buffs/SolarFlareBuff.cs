using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class SolarFlareBuff : ModBuff
    {
        
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Daybreak");
            DisplayName.AddTranslation(GameCulture.Chinese, "破晓");
            Description.SetDefault("Incenerated by solar rays");
            Description.AddTranslation(GameCulture.Chinese, "被太阳光线焚烧");
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
            tip = Utils.TranslationUtils.GetTranslation("DayBreakDescription") + Main.LocalPlayer.GetModPlayer<MABPlayer>().SolarFlare;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<MABPlayer>().SolarFlare == 0) player.GetModPlayer<MABPlayer>().SolarFlare = 1;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            if (player.GetModPlayer<MABPlayer>().SolarFlare < 4) { player.GetModPlayer<MABPlayer>().SolarFlare++; }
            return default;
        }

    }
}