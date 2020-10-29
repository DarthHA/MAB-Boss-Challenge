using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class ManaFlare : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mana Fare");
            DisplayName.AddTranslation(GameCulture.Chinese, "魔力星云");
            Description.SetDefault("Your defense and DR decreased");
            Description.AddTranslation(GameCulture.Chinese, "你的防御和减伤降低了");
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
            player.GetModPlayer<MABPlayer>().ManaFlare = true;
        }

    }
}