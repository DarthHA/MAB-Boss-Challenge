using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Buffs
{
    public class JusticeJudegmentBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Justice Judgement");
            DisplayName.AddTranslation(GameCulture.Chinese, "正义裁决");
            Description.SetDefault("Getting hit will get extra damage, based on your damage and max health.");
            Description.AddTranslation(GameCulture.Chinese, "受伤时会受到更多伤害，基于你的伤害和最大生命值。");
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
            player.GetModPlayer<MABPlayer>().JJEffect = true;
        }

    }
}