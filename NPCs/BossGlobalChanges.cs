using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    public class BossGlobalChanges : GlobalNPC
    {

        public override bool InstancePerEntity => true;
        public int CurrentHealth;    //寄生血量
        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.AncientLight && NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
            {
                npc.dontTakeDamage = true;
                for (int i = 0; i < npc.buffImmune.Length; i++)
                {
                    npc.buffImmune[i] = true;
                }
            }
            CurrentHealth = npc.lifeMax;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<LifeFlare>()))
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 240;
            }
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            DebuffDamage(npc, ref damage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            DebuffDamage(npc, ref damage);
        }
        private void DebuffDamage(NPC npc,ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<ManaFlare>()))
            {
                damage = (int)(damage * 1.25f);
            }
            if (npc.HasBuff(ModContent.BuffType<JusticeJudegmentBuff>()))
            {
                if (damage - npc.defense / 2 < npc.lifeMax / 10)
                {
                    damage += npc.defense / 2;
                }
                damage += npc.lifeMax / 10;
            }
        }
        public override void TownNPCAttackStrength(NPC npc, ref int damage, ref float knockback)
        {
            DebuffDamage2(npc, ref damage);
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            DebuffDamage2(npc, ref damage);
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            DebuffDamage2(npc, ref damage);
            if (npc.type == NPCID.AncientLight && NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
            {
                damage /= 2;
            }
        }
        private void DebuffDamage2(NPC npc, ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<DamageFlare>()))
            {
                damage = (int)(damage * 0.66f);
            }
        }
        public override void PostAI(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<ImprovedCelledBuff>()))
            {
                if (npc.life <= CurrentHealth)
                {
                    CurrentHealth = npc.life;
                }
                else
                {
                    npc.life = CurrentHealth;
                }
            }
        }
        public override bool PreAI(NPC npc)
        {

            switch (npc.type)
            {
                case NPCID.AncientLight:
                    if (NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()))
                    {
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
                default:
                    break;
            }

            return base.PreNPCLoot(npc);
        }



        
    }
}


