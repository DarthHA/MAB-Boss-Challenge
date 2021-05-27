
using MABBossChallenge.Utils;
using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace MABBossChallenge
{
    public class MABConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [Label("$Mods.MABBossChallenge.MeteorGuardianConfig1")]
        //[Tooltip("开启之后陨石守卫会在Boss战中参与攻击")]
        public bool NPCAttackBoss;

        [DefaultValue(true)]
        [Label("$Mods.MABBossChallenge.MeteorGuardianConfig2")]
        //[Tooltip("开启之后陨石守卫NPC会在战斗中使用自己的BGM")]
        public bool NPCAttackBGM;

        [DefaultValue(true)]
        [Label("$Mods.MABBossChallenge.UseFiltersConfig")]
        //[Tooltip("Boss战是否启用滤镜")]
        public bool BossFightFilters;


        public override ModConfig Clone()
        {
            var clone = (MABConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            MABBossChallenge.mabconfig = this;
            TranslationUtils.AddTranslation("MeteorGuardianConfig1", "Meteor Guardian(NPC) willjoin in Boss fight", "陨石守卫NPC参与Boss战");
            TranslationUtils.AddTranslation("MeteorGuardianConfig2", "Meteor Guardian(NPC) will use his own BGM when fighting", "陨石守卫NPC战斗时使用BGM");
            TranslationUtils.AddTranslation("UseFiltersConfig", "Initialize filters when fighting bosses", "Boss战启用滤镜");
        }


        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string messageline)
        {
            string message = "";
            string messagech = "";

            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                messageline = messagech;
            }
            else
            {
                messageline = message;
            }

            if (whoAmI == 0)
            {
                //message = "Changes accepted!";
                //messagech = "设置改动成功!";
                return true;
            }
            if (whoAmI != 0)
            {
                //message = "You have no rights to change config.";
                //messagech = "你没有设置改动权限.";
                return false;
            }
            return false;
        }
    }
}