
using MABBossChallenge.Buffs;
using MABBossChallenge.Items;
using MABBossChallenge.NPCs;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge
{

    public class MABPlayer : ModPlayer
    {
        public bool DamageFlare = false;   //伤害火焰
        public bool LifeFlare = false;     //生命火焰
        public bool ManaFlare = false;     //魔力火焰
        public int SolarFlare = 0;         //日耀层数
        public bool ImprovedCelled = false;   //寄生
        public int CurrentHealth = 10000;    //寄生血量

        public float ShaderTime = 0;


        // public float ShaderTime = 0;

        public override void ResetEffects()
        {

            if (!player.HasBuff(ModContent.BuffType<SolarFlareBuff>())) SolarFlare = 0;
            if (!player.HasBuff(ModContent.BuffType<ImprovedCelledBuff>())) CurrentHealth = player.statLifeMax2;
            DamageFlare = false;
            ManaFlare = false;
            LifeFlare = false;
            ImprovedCelled = false;
        }

        public override void UpdateDead()
        {

            DamageFlare = false;
            ManaFlare = false;
            LifeFlare = false;
            ImprovedCelled = false;
            SolarFlare = 0;

        }



        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            int TowerPunishment = 1;
            if (!MABWorld.DownedNebulaPlayer && !player.ZoneTowerNebula && NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()))
            {
                TowerPunishment = 2;
            }
            if (!MABWorld.DownedSolarPlayer && !player.ZoneTowerSolar && NPC.AnyNPCs(ModContent.NPCType<SolarFighterBoss>()))
            {
                TowerPunishment = 2;
            }
            if (!MABWorld.DownedStardustPlayer && !player.ZoneTowerStardust && NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
            {
                TowerPunishment = 2;
            }
            if (!MABWorld.DownedVortexPlayer && !player.ZoneTowerVortex && NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()))
            {
                TowerPunishment = 2;
            }
            damage *= TowerPunishment;



        }



        public override void PostUpdateMiscEffects()
        {

            if (player.name == "ModTest")
            {
                player.gravity = Player.defaultGravity;
            }
            //Main.NewText(CrazyGCount + " " + CrazyGCD);

            if (MABBossChallenge.AmmoChangeKey.JustReleased)
            {
                Main.PlaySound(SoundID.Item73, player.Center);
                Item itemtmp = player.inventory[54].DeepClone();
                for (int i = 55; i < 58; i++)   //54 55 56 57
                {
                    player.inventory[i - 1] = player.inventory[i].DeepClone();
                }
                player.inventory[57] = itemtmp.DeepClone();

                int chosen = 57;
                for (int i = 57; i > 53; i--)
                {
                    if (player.inventory[i].active && player.inventory[i].stack > 0)
                    {
                        chosen = i;
                    }
                }
                CombatText.NewText(player.getRect(), Color.Red, Lang.GetItemNameValue(player.inventory[chosen].type) + "(" + player.inventory[chosen].stack + ")");
            }



            if (LifeFlare)
            {
                player.bleed = true;
                player.statLifeMax2 = (int)(player.statLifeMax2 * 0.75f);
            }
            if (ManaFlare)
            {
                player.statDefense = (int)(player.statDefense * 0.75f);
                player.endurance = player.endurance * 0.75f;
            }
            if (DamageFlare)
            {
                player.allDamage -= 0.25f;
                player.magicCrit /= 2;
                player.meleeCrit /= 2;
                player.rangedCrit /= 2;
                player.thrownCrit /= 2;
            }
            if (ImprovedCelled)
            {
                if (CurrentHealth > player.statLife) CurrentHealth = player.statLife;
                if (CurrentHealth < player.statLife) player.statLife = CurrentHealth;
            }


            if (NPCUtils.InWall(player.Center, ModContent.WallType<Walls.ArenaWall>()))
            {
                player.controlHook = false;
                player.gravControl = false;
                player.gravControl2 = false;
                player.AddBuff(BuffID.NoBuilding, 2);
            }

        }

        public override void UpdateBadLifeRegen()
        {
            if (SolarFlare > 0)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegen -= 20 * SolarFlare;

            }
        }





        public override void UpdateBiomeVisuals()
        {

            bool IsSolarFighter = false;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<SolarFighterBoss>() && npc.ai[0] > 2)
                {
                    IsSolarFighter = true;
                }
            }
            player.ManageSpecialBiomeVisuals("MABBossChallenge:SolarBossSky", IsSolarFighter, default);
            player.ManageSpecialBiomeVisuals("Solar", IsSolarFighter || player.ZoneTowerSolar, new Vector2(Main.dungeonX, Main.dungeonY));


            bool IsVortexRanger = false;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<VortexRangerBoss>() && npc.ai[0] > 2)
                {
                    IsVortexRanger = true;
                }
            }

            player.ManageSpecialBiomeVisuals("MABBossChallenge:VortexBossSky", IsVortexRanger, default);
            player.ManageSpecialBiomeVisuals("Vortex", IsVortexRanger || player.ZoneTowerVortex, new Vector2(Main.dungeonX, Main.dungeonY));

            bool IsNebulaMage = false;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<NebulaMageBoss>() && npc.ai[0] > 2)
                {
                    IsNebulaMage = true;
                }
            }


            player.ManageSpecialBiomeVisuals("MABBossChallenge:NebulaBossSky", IsNebulaMage, default);
            player.ManageSpecialBiomeVisuals("Nebula", IsNebulaMage || player.ZoneTowerNebula, new Vector2(Main.dungeonX, Main.dungeonY));

        }

        public override void UpdateBiomes()
        {
            if (!MABWorld.DownedMeteorPlayer && player.ZoneMeteor && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerDefender>()) && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerBoss>()))
            {
                NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<MeteorPlayerDefender>());
            }

            bool IsSolarFighter = false;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<SolarFighterBoss>() && npc.ai[0] > 2)
                {
                    IsSolarFighter = true;
                }
            }

            player.ManageSpecialBiomeVisuals("Solar", IsSolarFighter || player.ZoneTowerSolar, new Vector2(Main.dungeonX, Main.dungeonY));

            bool IsVortexRanger = false;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<VortexRangerBoss>() && npc.ai[0] > 2)
                {
                    IsVortexRanger = true;
                }
            }

            player.ManageSpecialBiomeVisuals("Vortex", IsVortexRanger || player.ZoneTowerVortex, new Vector2(Main.dungeonX, Main.dungeonY));

        }






        public override void PreSavePlayer()
        {

            if (NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>()) != -1)
            {
                NPC npc = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())];
                NPC npctmp = (NPC)npc.Clone();
                npc.Transform(ModContent.NPCType<MeteorPlayerNPC1>());
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeless = npctmp.homeless;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeTileX = npctmp.homeTileX;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].homeTileY = npctmp.homeTileY;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeless = npctmp.oldHomeless;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeTileX = npctmp.oldHomeTileX;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].oldHomeTileY = npctmp.oldHomeTileY;
            }
        }






        public override void OnEnterWorld(Player player)
        {
            if (!MABWorld.IsCreated)
            {
                Main.NewText(TranslationUtils.GetTranslation("GenerateBattleWarning"), Color.Green);
                Item.NewItem(player.Hitbox, ModContent.ItemType<ArenaSummon>());
            }

            base.OnEnterWorld(player);
        }









    }
}