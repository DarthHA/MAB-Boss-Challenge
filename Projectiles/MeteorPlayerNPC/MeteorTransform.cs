using MABBossChallenge.NPCs;
using MABBossChallenge.NPCs.MeteorPlayerNPC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MeteorPlayerNPC
{
    public class MeteorTransform : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.hide = true;
        }
        public override void AI()
        {
            projectile.velocity = Vector2.Zero;

        }
        public override void Kill(int timeLeft)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC1>()) && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC2>()))
            {
                NPC npctmp = (NPC)(Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].Clone());

                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC1>())].Transform(ModContent.NPCType<MeteorPlayerNPC2>());
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].homeless = npctmp.homeless;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].homeTileX = npctmp.homeTileX;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].homeTileY = npctmp.homeTileY;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].oldHomeless = npctmp.oldHomeless;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].oldHomeTileX = npctmp.oldHomeTileX;
                Main.npc[NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerNPC2>())].oldHomeTileY = npctmp.oldHomeTileY;

            }
        }

    }
}