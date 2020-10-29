using MABBossChallenge.NPCs;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.Tiles;
using MABBossChallenge.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge
{
    public class MABGlobalTile : GlobalTile
    {

        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.Meteorite && !MABWorld.DownedMeteorPlayer)
            {
                SummonMeteorDefender(type, i, j);
                return false;
            }
            return true;
        }


        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (type == TileID.Meteorite && !MABWorld.DownedMeteorPlayer)
            {
                SummonMeteorDefender(type, i, j);
                return false;
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (type == TileID.Meteorite && !MABWorld.DownedMeteorPlayer)
            {
                SummonMeteorDefender(type, i, j);
                return false;
            }

            return true;
        }

        public override bool PreHitWire(int i, int j, int type)
        {
            if (!MABWorld.DownedPreEvilFighter && Main.tile[i, j].type == ModContent.TileType<ArenaTile>()) return false;
            return true;
        }

        public override bool CanPlace(int i, int j, int type)
        {
            if (!MABWorld.DownedPreEvilFighter && Main.tile[i, j].wall == ModContent.WallType<ArenaWall>()) return false;
            return true;
        }





        private void SummonMeteorDefender(int type, int i, int j)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerBoss>()))
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerDefender>()))
                {
                    NPC.NewNPC(i * 16, j * 16, ModContent.NPCType<MeteorPlayerBoss>());
                }
                else
                {
                    int npctmp = -1;
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && npc.type == ModContent.NPCType<MeteorPlayerDefender>())
                        {
                            npctmp = npc.whoAmI;
                            break;
                        }
                    }
                    Main.npc[npctmp].Transform(ModContent.NPCType<MeteorPlayerBoss>());
                }
            }
        }
    }
}