using Terraria.ID;
using MABBossChallenge.NPCs;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class SolarEGOBuff : ModBuff
    {
        
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Solar EGO");
            //DisplayName.AddTranslation(GameCulture.Chinese, "破晓");
            Description.SetDefault("A HA");
            //Description.AddTranslation(GameCulture.Chinese, "被太阳光线焚烧");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            this.canBeCleared = false;
            Main.lightPet[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            this.longerExpertDebuff = false;
            Main.pvpBuff[Type] = false;
            Main.vanityPet[Type] = false;

        }


    }
}