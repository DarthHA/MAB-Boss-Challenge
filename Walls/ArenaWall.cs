using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Walls
{
    public class ArenaWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;

            dustType = MyDustId.BlackGreenGrass;
            drop = ItemID.GrayBrickWall;
            AddMapEntry(Color.Gray);
        }
        public override bool CanExplode(int i, int j)
        {
            return MABWorld.DownedPreEvilFighter;
        }
        public override void KillWall(int i, int j, ref bool fail)
        {
            if (!MABWorld.DownedPreEvilFighter)
            {
                fail = false;
            }
            else
            {
                fail = true;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4f;
            g = 0.4f;
            b = 0.4f;
        }
    }
}