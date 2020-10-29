using MABBossChallenge.Items;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MABBossChallenge.Tiles
{
    public class CrimsonAltarSummon : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = false;
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("奇怪的血腥祭坛");
            AddMapEntry(new Color(144, 144, 144), name);
            adjTiles = new int[] { TileID.DemonAltar };

        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {

        }
        public override bool NewRightClick(int i, int j)
        {
            if (Main.LocalPlayer.HeldItem.type == ItemID.TissueSample)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<CrimsonPlayerBoss>()))
                {
                    Main.LocalPlayer.HeldItem.stack--;
                    if (Main.LocalPlayer.HeldItem.stack == 0) Main.LocalPlayer.HeldItem.TurnToAir();

                    int SummonX = i;
                    int SummonY = j;
                    SummonX += (18 - Main.tile[i, j].frameX) / 18;
                    SummonY -= Main.tile[i, j].frameY / 18 + 1;
                    NPC.NewNPC(SummonX * 16, SummonY * 16, ModContent.NPCType<CrimsonPlayerBoss>());
                    Main.NewText("猩红守护者已苏醒！", 175, 75, 255);
                    Main.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
                    return false;
                }
            }
            if (Main.LocalPlayer.HeldItem.type == ItemID.ShadowScale)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<ShadowPlayerBoss>()))
                {
                    Main.LocalPlayer.HeldItem.stack--;
                    if (Main.LocalPlayer.HeldItem.stack == 0) Main.LocalPlayer.HeldItem.TurnToAir();

                    int SummonX = i;
                    int SummonY = j;
                    SummonX += (18 - Main.tile[i, j].frameX) / 18;
                    SummonY -= Main.tile[i, j].frameY / 18 + 1;
                    NPC.NewNPC(SummonX * 16, SummonY * 16, ModContent.NPCType<ShadowPlayerBoss>());
                    Main.NewText("暗影守护者已苏醒！", 175, 75, 255);
                    Main.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
                    return false;
                }
            }

            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ShadowSample>())
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<CrimsonPlayerBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<ShadowPlayerBoss>()))
                {
                    int SummonX = i;
                    int SummonY = j;
                    SummonX += (18 - Main.tile[i, j].frameX) / 18;
                    SummonY -= Main.tile[i, j].frameY / 18 + 1;
                    int npcmtp = NPC.NewNPC(SummonX * 16, SummonY * 16, ModContent.NPCType<ShadowPlayerBoss>());
                    Main.npc[npcmtp].lifeMax = 40000;
                    Main.npc[npcmtp].life = 40000;
                    Main.npc[npcmtp].damage = 90;
                    Main.npc[npcmtp].defense = 20;
                    Main.npc[npcmtp].localAI[2] = 1;
                    npcmtp = NPC.NewNPC(SummonX * 16, SummonY * 16, ModContent.NPCType<CrimsonPlayerBoss>());
                    Main.npc[npcmtp].lifeMax = 48000;
                    Main.npc[npcmtp].life = 48000;
                    Main.npc[npcmtp].damage = 100;
                    Main.npc[npcmtp].defense = 40;
                    Main.npc[npcmtp].localAI[2] = 1;
                    Main.NewText("守护者兄弟已苏醒！", 175, 75, 255);
                    Main.LocalPlayer.GetModPlayer<SMPlayer>().Set(new Vector2(SummonX * 16, SummonY * 16), "守护者兄弟", "来自邪恶之地的二重奏");
                    Main.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
                    return false;
                }
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = player.HeldItem.type == ItemID.ShadowScale ? ItemID.ShadowScale : ItemID.TissueSample;
            if (player.HeldItem.type == ModContent.ItemType<ShadowSample>()) player.showItemIcon2 = ModContent.ItemType<ShadowSample>();
        }
    }
}