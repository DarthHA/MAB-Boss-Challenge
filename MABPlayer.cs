using MABBossChallenge.Buffs;
using MABBossChallenge.Items;
using MABBossChallenge.NPCs;
using MABBossChallenge.NPCs.EchDestroyer;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge
{

    public class MABPlayer : ModPlayer
    {
        public bool JJEffect = false;      //正裁效果
        public bool DamageFlare = false;   //伤害火焰
        public bool LifeFlare = false;     //生命火焰
        public bool ManaFlare = false;     //魔力火焰
        public int SolarFlare = 0;         //日耀层数
        public bool ImprovedCelled = false;   //寄生
        public int CurrentHealth = 10000;    //寄生血量


        public override void ResetEffects()
        {
            if (!player.HasBuff(ModContent.BuffType<SolarFlareBuff>())) SolarFlare = 0;
            if (!player.HasBuff(ModContent.BuffType<ImprovedCelledBuff>())) CurrentHealth = player.statLifeMax2;
            DamageFlare = false;
            ManaFlare = false;
            LifeFlare = false;
            ImprovedCelled = false;
            JJEffect = false;
        }

        public override void UpdateDead()
        {
            JJEffect = false;
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


            JJEffectDamage(ref damage);

        }


        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            JJEffectDamage(ref damage);
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyPillarBossResist(target, ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            ModifyPillarBossResist(target, ref damage);
        }
        public override void PostUpdateMiscEffects()
        {

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

            PillarBossDebuffMiscEffects();
            
            if (NPCUtils.InWall(player.Center, ModContent.WallType<Walls.ArenaWall>()))
            {
                player.controlHook = false;
                player.gravControl = false;
                player.gravControl2 = false;
                player.AddBuff(BuffID.NoBuilding, 2);
            }

        }

        public override void PostUpdateEquips()
        {
            if (NPCUtils.InWall(player.Center, ModContent.WallType<Walls.ArenaWall>()))
            {
                player.wingTime = 0;
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (ImprovedCelled)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;

                player.lifeRegenTime = 0;
            }

            if (SolarFlare > 0)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegen -= 20 * SolarFlare;

            }


        }




        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (damage == 10.0 && SolarFlare > 0)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + TranslationUtils.GetTranslation("SolarFlareDeath"));
            }
            if (JJEffect && damage != 10.0)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + TranslationUtils.GetTranslation("JJDeath"));
            }
            return true;
        }

        public override void PreUpdateMovement()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
                if (player.velocity.Length() > 18)
                {
                    player.velocity = Vector2.Normalize(player.velocity) * 18;
                }
            }
        }

        public override void UpdateBiomeVisuals()
        {
            //player.ManageSpecialBiomeVisuals("MABBossChallenge:EchBossSky", ExistEch, default);

            if (MABBossChallenge.mabconfig.BossFightFilters)
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


                bool IsStardustBoss = false;

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<StardustSummonerBoss>() && npc.ai[0] > 2)
                    {
                        IsStardustBoss = true;
                    }
                }

                player.ManageSpecialBiomeVisuals("MABBossChallenge:StardustBossSky", IsStardustBoss, default);
                player.ManageSpecialBiomeVisuals("Stardust", IsStardustBoss || player.ZoneTowerStardust, new Vector2(Main.dungeonX, Main.dungeonY));


                player.ManageSpecialBiomeVisuals("MABBossChallenge:WarpSky", true, default);
            }
        }

        public override void UpdateBiomes()
        {
            if (!MABWorld.DownedMeteorPlayer && player.ZoneMeteor && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerDefender>()) && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerBoss>()))
            {
                NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<MeteorPlayerDefender>());
            }

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



        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            if (NPCUtils.AnyThisModBosses())
            {
                price = 1145141919;
            }
        }
        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (NPCUtils.AnyThisModBosses())
            {
                removeDebuffs = false;
                health = 1;
            }
            return true;
        }

        public override void OnEnterWorld(Player player)
        {
            if (!MABWorld.IsCreated)
            {
                Main.NewText(TranslationUtils.GetTranslation("GenerateBattleWarning"), Color.Green);
                Item.NewItem(player.Hitbox, ModContent.ItemType<ArenaSummon>());
            }
        }



        public void JJEffectDamage(ref int damage)
        {
            if (JJEffect)
            {
                float[] DamageBoost = new float[5];
                DamageBoost[0] = player.meleeDamage + (float)player.meleeCrit / 100 + player.allDamage;
                DamageBoost[1] = player.rangedDamage + (float)player.rangedCrit / 100 + player.allDamage;
                DamageBoost[2] = player.magicDamage + (float)player.magicCrit / 100 + player.allDamage;
                DamageBoost[3] = player.minionDamage + (float)player.maxMinions / 5 + player.allDamage;
                DamageBoost[4] = player.thrownDamage + (float)player.thrownCrit / 100 + player.allDamage;

                int dmg = (int)(player.statLifeMax2 * 0.02f * (1 + DamageBoost.Max()) / 4);
                if (damage - player.statDefense / 8 < dmg)
                {
                    damage += player.statDefense / 8;
                }
                damage += dmg;
            }
        }

        public void PillarBossDebuffMiscEffects()
        {

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
                if (player.statLife > CurrentHealth)
                    player.statLife = CurrentHealth;
                else
                    CurrentHealth = player.statLife;
            }
            else
            {
                CurrentHealth = player.statLife;
            }
        }

        public void ModifyPillarBossResist(NPC target,ref int damage)
        {
            if (target.type == ModContent.NPCType<SolarFighterBoss>())
            {
                if (MABWorld.DownedSolarPlayer && !player.ZoneTowerSolar)
                {
                    damage /= 2;
                }
            }
            if (target.type == ModContent.NPCType<VortexRangerBoss>())
            {
                if (MABWorld.DownedVortexPlayer && !player.ZoneTowerVortex)
                {
                    damage /= 2;
                }
            }
            if (target.type == ModContent.NPCType<NebulaMageBoss>())
            {
                if (MABWorld.DownedNebulaPlayer && !player.ZoneTowerNebula)
                {
                    damage /= 2;
                }
            }
            if (target.type == ModContent.NPCType<StardustSummonerBoss>())
            {
                if (MABWorld.DownedStardustPlayer && !player.ZoneTowerStardust)
                {
                    damage /= 2;
                }
            }
        }

    }
}