using MABBossChallenge.Items;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Sky;
using MABBossChallenge.UI;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace MABBossChallenge
{
    public class MABBossChallenge : Mod
    {
        internal static ModHotKey AmmoChangeKey;
        public static MABBossChallenge Instance;
        public static MABConfig mabconfig;
        private UserInterface _BossStateInterface;
        internal BossState _BossState;
        public MABBossChallenge()
        {
            Instance = this;
        }

        public override void Load()
        {
            AmmoChangeKey = RegisterHotKey("Swap Ammo Key", "Z");

            Filters.Scene["MABBossChallenge:VortexBossSky"] = new Filter(new CustomScreenShaderData("FilterMiniTower").UseColor(0.255f, 0f, 0f).UseOpacity(0.2f), EffectPriority.VeryHigh);
            SkyManager.Instance["MABBossChallenge:VortexBossSky"] = new VortexBossSky();

            Filters.Scene["MABBossChallenge:SolarBossSky"] = new Filter(new CustomScreenShaderData("FilterMiniTower").UseColor(0.255f, 0f, 0f).UseOpacity(0.2f), EffectPriority.VeryHigh);
            SkyManager.Instance["MABBossChallenge:SolarBossSky"] = new SolarBossSky();

            Filters.Scene["MABBossChallenge:NebulaBossSky"] = new Filter(new CustomScreenShaderData("FilterMiniTower").UseColor(0.255f, 0f, 0f).UseOpacity(0.2f), EffectPriority.VeryHigh);
            SkyManager.Instance["MABBossChallenge:NebulaBossSky"] = new NebulaBossSky();

            _BossState = new BossState();
            _BossStateInterface = new UserInterface();
            _BossStateInterface.SetState(_BossState);
        }

        public override void PostSetupContent()
        {
            Main.versionNumber = "Ech";

            //联动BCL
            AddToBCL();

            //联动Fargo
            AddToFargo();


            AddTrans();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            int BossStateIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
            if (BossStateIndex != -1)
            {
                layers.Insert(BossStateIndex, new LegacyGameInterfaceLayer(
                    "MABBossChallenge: BossState",
                    delegate
                    {
                        _BossStateInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void Unload()
        {
            SkyManager.Instance["MABBossChallenge:SolarBossSky"].Deactivate();
            SkyManager.Instance["MABBossChallenge:VortexBossSky"].Deactivate();
            SkyManager.Instance["MABBossChallenge:NebulaBossSky"].Deactivate();

            //TabKey = null;
            Instance = null;            //用于正常卸载
            mabconfig = null;
            //Environment.Exit(0);
        }

        private void AddTrans()
        {
            ModTranslation CustomText = MABBossChallenge.Instance.CreateTranslation("DayBreakDescription");
            CustomText.SetDefault("Incenerated by solar rays\nThe more hit you get from solar attack, the worse of this debuff\nLevel: ");
            CustomText.AddTranslation(GameCulture.Chinese, "被太阳光线焚烧\n受到日耀的攻击越多，该效果将会变得更严重\n层数：");
            MABBossChallenge.Instance.AddTranslation(CustomText);

            TranslationUtils.AddTranslation("GenerateBattleWarning", "Detecting that this world hasn't generate Battlefield. Try using the Battlefield Generator to generate one!", "检测到该世界没有生成战场！尝试使用战场生成器来创建一个！");
            TranslationUtils.AddTranslation("MeteorGuardianNPCName", "No-Name", "无名");
            TranslationUtils.AddTranslation("TrueFight", "The real battle has just begun...", "真正的战斗才刚刚开始...");

            TranslationUtils.AddTranslation("GuardianBro", "Evil Guardian Borthers", "邪恶守护者兄弟");
            TranslationUtils.AddTranslation("GuardianBroDescription", "Duo from the Evils", "来自邪恶之地的二重奏");

            TranslationUtils.AddTranslation("SolarFlareDeath", " was melted by the power of solar flare.", "被日耀之炎融化了。");
            TranslationUtils.AddTranslation("JJDeath", "'s soul received the fair result of the Judgement.", "灵魂得到了公正的的判决。");
        }

        private void AddToBCL()
        {
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            {
                Mod mod1 = bossChecklist;
                object[] array = new object[12];


                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 2.5f;
                array[2] = ModContent.NPCType<MeteorPlayerBoss>();
                array[3] = Instance;
                array[4] = "Meteor Guardian";
                array[5] = new Func<bool>(() => MABWorld.DownedMeteorPlayer);
                array[6] = ItemID.CopperPickaxe;
                array[7] = new List<int>
                {

                };
                array[8] = new List<int>
                {
                    //掉落物
                };
                array[9] = "Spawn on the Meteorite. Become hostile when you hurt him or mining the meteorite.";
                array[11] = "MABBossChallenge/BossChecklist/MeteorPlayerBoss_BCL";
                mod1.Call(array);



                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 5.5f;
                array[2] = ModContent.NPCType<ShadowPlayerBoss>();
                array[3] = Instance;
                array[4] = "Shadow Guardian";
                array[5] = new Func<bool>(() => MABWorld.DownedPreEvilFighter);
                array[6] = ItemID.ShadowScale;
                array[7] = new List<int>
                {

                };
                array[8] = new List<int>
                {
                    //掉落物
                };
                array[9] = "Putting a [i:" + ItemID.ShadowScale + "] into the Strange Demon Altar in the Battlefield on Corruption.";
                array[11] = "MABBossChallenge/BossChecklist/ShadowPlayer_BCL";
                mod1.Call(array);


                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 5.5f;
                array[2] = ModContent.NPCType<CrimsonPlayerBoss>();
                array[3] = Instance;
                array[4] = "Crimson Guardian";
                array[5] = new Func<bool>(() => MABWorld.DownedPreEvilFighter2);
                array[6] = ItemID.TissueSample;
                array[7] = new List<int>
                {

                };
                array[8] = new List<int>
                {
                    //掉落物
                };
                array[9] = "Putting a [i:" + ItemID.TissueSample + "] into the Strange Crimson Altar in the Battlefield on Crimson.";
                array[11] = "MABBossChallenge/BossChecklist/CrimsonPlayer_BCL";
                mod1.Call(array);



                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 13.5f;
                array[2] = ModContent.NPCType<SolarFighterBoss>();
                array[3] = Instance;
                array[4] = "Solar Defender";
                array[5] = new Func<bool>(() => MABWorld.DownedSolarPlayer);
                array[6] = ModContent.ItemType<PlayerSummon1>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentSolar,
                    ItemID.DayBreak,
                    ItemID.SolarEruption,
                };
                array[9] = "Killing enough Solar Pillar enemy or using [i:" + ModContent.ItemType<PlayerSummon1>() + "] to summon after defeating him once.";
                array[11] = "MABBossChallenge/BossChecklist/SolarFighter_BCL";
                mod1.Call(array);


                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 13.5f;
                array[2] = ModContent.NPCType<VortexRangerBoss>();
                array[3] = Instance;
                array[4] = "Vortex Defender";
                array[5] = new Func<bool>(() => MABWorld.DownedVortexPlayer);
                array[6] = ModContent.ItemType<PlayerSummon2>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentVortex,
                    ItemID.VortexBeater,
                    ItemID.Phantasm,
                };
                array[9] = "Killing enough Vortex Pillar enemy or using [i:" + ModContent.ItemType<PlayerSummon2>() + "] to summon after defeating him once.";
                array[11] = "MABBossChallenge/BossChecklist/VortexRanger_BCL";
                mod1.Call(array);



                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 13.5f;
                array[2] = ModContent.NPCType<NebulaMageBoss>();
                array[3] = Instance;
                array[4] = "Nebula Defender";
                array[5] = new Func<bool>(() => MABWorld.DownedNebulaPlayer);
                array[6] = ModContent.ItemType<PlayerSummon3>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentNebula,
                    ItemID.NebulaBlaze,
                    ItemID.NebulaArcanum,
                };
                array[9] = "Killing enough Nebula Pillar enemy or using [i:" + ModContent.ItemType<PlayerSummon3>() + "] to summon after defeating him once.";
;                array[11] = "MABBossChallenge/BossChecklist/NebulaMage_BCL";
                mod1.Call(array);




                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 13.5f;
                array[2] = ModContent.NPCType<StardustSummonerBoss>();
                array[3] = Instance;
                array[4] = "Stardust Defender";
                array[5] = new Func<bool>(() => MABWorld.DownedStardustPlayer);
                array[6] = ModContent.ItemType<PlayerSummon4>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentStardust,
                    ItemID.StardustCellStaff,
                    ItemID.StardustDragonStaff,
                };
                array[9] = "Killing enough Stardust Pillar enemy or using [i:" + ModContent.ItemType<PlayerSummon4>() + "] to summon after defeating him once.";
                array[11] = "MABBossChallenge/BossChecklist/StardustSummoner_BCL";
                mod1.Call(array);





                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 14.4f;
                array[2] = ModContent.NPCType<SolarFighterBoss>();
                array[3] = Instance;
                array[4] = "Solar Warrior";
                array[5] = new Func<bool>(() => MABWorld.DownedSolarPlayerEX);
                array[6] = ModContent.ItemType<PlayerSummon1>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentSolar,
                    ItemID.DayBreak,
                    ItemID.SolarEruption,
                    ItemID.Meowmere,
                    ItemID.StarWrath,
                    ItemID.Terrarian,
                    ItemID.SolarFlareHelmet,
                    ItemID.SolarFlareBreastplate,
                    ItemID.SolarFlareLeggings

                };
                array[9] = "Using [i:" + ModContent.ItemType<PlayerSummon1>() + "] after defeating Solar Defender and Moon Lord.";
                array[11] = "MABBossChallenge/BossChecklist/SolarFighter_BCL";
                mod1.Call(array);


                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 14.4f;
                array[2] = ModContent.NPCType<VortexRangerBoss>();
                array[3] = Instance;
                array[4] = "Vortex Ranger";
                array[5] = new Func<bool>(() => MABWorld.DownedVortexPlayerEX);
                array[6] = ModContent.ItemType<PlayerSummon2>();
                array[7] = new List<int>
                {
                    //ItemID.DestroyerMask,
                    //ItemID.DestroyerTrophy,
                };
                array[8] = new List<int>
                {
                    ItemID.FragmentVortex,
                    ItemID.VortexBeater,
                    ItemID.Phantasm,
                    ItemID.FireworksLauncher,
                    ItemID.SDMG,
                    ItemID.MoonlordBullet,
                    ItemID.VortexHelmet,
                    ItemID.VortexBreastplate,
                    ItemID.VortexLeggings

                };
                array[9] = array[9] = "Using [i:" + ModContent.ItemType<PlayerSummon2>() + "] after defeating Vortex Defender and Moon Lord.";
                array[11] = "MABBossChallenge/BossChecklist/VortexRanger_BCL";
                mod1.Call(array);


                
                mod1 = bossChecklist;
                array = new object[12];
                array[0] = "AddBoss";
                array[1] = 14.4f;
                array[2] = ModContent.NPCType<NebulaMageBoss>();
                array[3] = Instance;
                array[4] = "Nebula Mage";
                array[5] = new Func<bool>(() => MABWorld.DownedNebulaPlayerEX);
                array[6] = ModContent.ItemType<PlayerSummon3>();
                array[7] = new List<int>
                {

                };
                array[8] = new List<int>
                {
                    ItemID.FragmentNebula,
                    ItemID.NebulaArcanum,
                    ItemID.NebulaBlaze,
                    ItemID.LastPrism,
                    ItemID.LunarFlareBook,
                    ItemID.NebulaBreastplate,
                    ItemID.NebulaHelmet,
                    ItemID.NebulaLeggings

                };
                array[9] = array[9] = "Using [i:" + ModContent.ItemType<PlayerSummon3>() + "] after defeating Nebula Defender and Moon Lord.";
                array[11] = "MABBossChallenge/BossChecklist/NebulaMage_BCL";
                mod1.Call(array);

                
            }
        }

        private void AddToFargo()
        {
            Mod mod2 = ModLoader.GetMod("Fargowiltas");
            if (mod2 != null)
            {
                mod2.Call("AddSummon", 13.5f, "MABBossChallenge", "PlayerSummon1", (Func<bool>)(() => MABWorld.DownedSolarPlayer), Item.buyPrice(0, 8, 0, 0)); //1f is a dummy value and not used
                mod2.Call("AddSummon", 13.5f, "MABBossChallenge", "PlayerSummon2", (Func<bool>)(() => MABWorld.DownedVortexPlayer), Item.buyPrice(0, 8, 0, 0)); //1f is a dummy value and not used
                mod2.Call("AddSummon", 13.5f, "MABBossChallenge", "PlayerSummon3", (Func<bool>)(() => MABWorld.DownedNebulaPlayer), Item.buyPrice(0, 8, 0, 0)); //1f is a dummy value and not used
                mod2.Call("AddSummon", 13.5f, "MABBossChallenge", "PlayerSummon4", (Func<bool>)(() => MABWorld.DownedStardustPlayer), Item.buyPrice(0, 8, 0, 0)); //1f is a dummy value and not used
                mod2.Call("AddSummon", 13.6f, "MABBossChallenge", "PlayerSummon", (Func<bool>)(() => MABWorld.DownedStardustPlayer && MABWorld.DownedNebulaPlayer && MABWorld.DownedVortexPlayer && MABWorld.DownedSolarPlayer), Item.buyPrice(0, 32, 0, 0)); //1f is a dummy value and not used
                mod2.Call("AddSummon", 17f, "MABBossChallenge", "PlayerSummonEX", (Func<bool>)(() => (MABWorld.DownedSolarPlayerEX && MABWorld.DownedVortexPlayerEX)), Item.buyPrice(1, 0, 0, 0)); //1f is a dummy value and not used
            }
        }
    }
}