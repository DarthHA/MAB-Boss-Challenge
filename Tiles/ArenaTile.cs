using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Tiles
{
    public class ArenaTile : ModTile
    {
        public override void SetDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileValue[Type] = 0;
            Main.tileShine2[Type] = false;
            Main.tileShine[Type] = 0;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("地形方块");
            AddMapEntry(Color.DarkGray, name);

            dustType = 84;
            drop = ItemID.GrayBrick;
            soundType = SoundID.ForceRoar;
            soundStyle = 1;
            mineResist = 4f;
            minPick = 50;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return MABWorld.DownedPreEvilFighter;
        }
        public override bool CanExplode(int i, int j)
        {
            return MABWorld.DownedPreEvilFighter;
        }
    }
}