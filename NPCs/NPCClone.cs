using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    /// <summary>
    /// 可以复制原版的NPC的属性和行为，
    /// Type为复制的NPC种类，
    /// OverrideTexture可以自定义材质，
    /// OverrideName可以自定义名字
    /// </summary>
    public abstract class NPCClone : ModNPC
    {

        public virtual int Type => NPCID.None;
        public virtual string OverrideTexture => "Terraria/NPC_" + Type;
        public virtual string OverrideName => Lang.GetNPCNameValue(Type);
        public override string Texture => OverrideTexture;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(OverrideName);
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[Type];
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(Type);
            aiType = Type;
            animationType = Type;
        }
    }
}