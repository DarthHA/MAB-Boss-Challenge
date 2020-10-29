using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class LifeFlare : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Life Fare");
            DisplayName.AddTranslation(GameCulture.Chinese, "生命星云");
            Description.SetDefault("Your max health decreased");
            Description.AddTranslation(GameCulture.Chinese, "你的最大生命降低了");
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
            player.GetModPlayer<MABPlayer>().LifeFlare = true;
        }

    }
}