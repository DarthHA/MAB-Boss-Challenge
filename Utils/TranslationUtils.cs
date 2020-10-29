
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Utils
{
    public static class TranslationUtils
    {
        public static void AddTranslation(string En, string Zh)
        {
            string temp = En.Replace(" ", "_");
            ModTranslation CustomText = MABBossChallenge.Instance.CreateTranslation(temp);
            CustomText.SetDefault(En);
            CustomText.AddTranslation(GameCulture.Chinese, Zh);
            MABBossChallenge.Instance.AddTranslation(CustomText);
        }
        public static void AddTranslation(string key ,string En, string Zh)
        {
            ModTranslation CustomText = MABBossChallenge.Instance.CreateTranslation(key);
            CustomText.SetDefault(En);
            CustomText.AddTranslation(GameCulture.Chinese, Zh);
            MABBossChallenge.Instance.AddTranslation(CustomText);
        }
        public static string GetTranslation(string key)
        {
            return Language.GetTextValue("Mods.MABBossChallenge." + key);
        }
    }
}