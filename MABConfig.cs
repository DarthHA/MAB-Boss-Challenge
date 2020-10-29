
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
        [Label("Meteor Guardian will join in Boss fight")]
        //[Tooltip("开启之后陨石守卫会在Boss战中参与攻击，默认为否")]
        public bool NPCAttackBoss;

        [DefaultValue(true)]
        [Label("Meteor Guardian will use its own BGM when fighting")]
        //[Tooltip("开启之后陨石守卫NPC会在战斗中使用BGM，默认为是")]
        public bool NPCAttackBGM;


        public override ModConfig Clone()
        {
            var clone = (MABConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            MABBossChallenge.mabconfig = this;
            TranslationUtils.AddTranslation("MeteorGuardianConfig1", "Meteor Guardian joins in Boss fight", "陨石守卫参与Boss战");
            TranslationUtils.AddTranslation("MeteorGuardianConfig2", "Meteor Guardian uses its own BGM when fighting", "陨石守卫战斗时使用BGM");
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