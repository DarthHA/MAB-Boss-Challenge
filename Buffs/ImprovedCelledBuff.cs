using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class ImprovedCelledBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cell Parasitism");
            DisplayName.AddTranslation(GameCulture.Chinese, "细胞寄生");
            Description.SetDefault("Cells block the way to heal");
            Description.AddTranslation(GameCulture.Chinese, "细胞阻碍你进行治疗");
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
            this.canBeCleared = false;
            Main.lightPet[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            this.longerExpertDebuff = true;
            Main.pvpBuff[Type] = true;
            Main.vanityPet[Type] = false;

        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MABPlayer>().ImprovedCelled = true;
        }

    }
}