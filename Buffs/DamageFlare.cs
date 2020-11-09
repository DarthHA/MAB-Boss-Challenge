using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class DamageFlare : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Damage Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "伤害星云");
            Description.SetDefault("Your damage and crit desreased");
            Description.AddTranslation(GameCulture.Chinese, "你的伤害和暴击降低了");
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
            player.GetModPlayer<MABPlayer>().DamageFlare = true;
        }

    }
}