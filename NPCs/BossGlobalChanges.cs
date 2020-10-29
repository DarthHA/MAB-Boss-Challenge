using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    public class BossGlobalChanges : GlobalNPC
    {

        public override bool InstancePerEntity => true;


        public override bool PreAI(NPC npc)
        {

            switch (npc.type)
            {
                case NPCID.AncientLight:
                    if (NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
                    {
                        npc.dontTakeDamage = true;
                        npc.position += npc.velocity * 0.75f;
                    }
                    break;

                #region LunarTower

                case NPCID.LunarTowerNebula:
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.ZoneTowerNebula && !MABWorld.DownedNebulaPlayer && NPC.ShieldStrengthTowerNebula <= 5 && !NPC.AnyNPCs(ModContent.NPCType<NebulaMageBoss>()))
                    {
                        NPC.ShieldStrengthTowerNebula = 5;
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 300, ModContent.NPCType<NebulaMageBoss>());
                        //Main.NewText("你惊扰了星云守护者", 175, 75, 255);
                    }
                    break;
                case NPCID.LunarTowerSolar:
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.ZoneTowerSolar && !MABWorld.DownedSolarPlayer && NPC.ShieldStrengthTowerSolar <= 5 && !NPC.AnyNPCs(ModContent.NPCType<SolarFighterBoss>()))
                    {
                        NPC.ShieldStrengthTowerSolar = 5;
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 300, ModContent.NPCType<SolarFighterBoss>());
                       //Main.NewText("你惊扰了日耀守护者", 175, 75, 255);
                    }
                    break;
                case NPCID.LunarTowerStardust:
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.ZoneTowerStardust && !MABWorld.DownedStardustPlayer && NPC.ShieldStrengthTowerStardust <= 5 && !NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
                    {
                        NPC.ShieldStrengthTowerStardust = 5;
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 300, ModContent.NPCType<StardustSummonerBoss>());
                        //Main.NewText("你惊扰了星尘守护者", 175, 75, 255);
                    }
                    break;
                case NPCID.LunarTowerVortex:
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.ZoneTowerVortex && !MABWorld.DownedVortexPlayer && NPC.ShieldStrengthTowerVortex <= 5 && !NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()))
                    {
                        NPC.ShieldStrengthTowerVortex = 5;
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 300, ModContent.NPCType<VortexRangerBoss>());
                        //Main.NewText("你惊扰了星璇守护者", 175, 75, 255);
                    }

                    break;

                #endregion

                default:
                    break;
            }

            return true;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (NPCUtils.AnyBosses())
            {
                spawnRate *= 5;
                maxSpawns = (int)(maxSpawns * 0.001f);
            }
        }







        public override bool PreNPCLoot(NPC npc)
        {

            switch (npc.type)
            {

                case NPCID.MeteorHead:
                    {
                        return MABWorld.DownedMeteorPlayer;
                    }
                    break;
                default:
                    break;
            }

            return base.PreNPCLoot(npc);
        }



        
    }
}


