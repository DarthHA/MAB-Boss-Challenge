using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    /// <summary>
    /// ���Ը���ԭ���NPC�����Ժ���Ϊ��
    /// TypeΪ���Ƶ�NPC���࣬
    /// OverrideTexture�����Զ�����ʣ�
    /// OverrideName�����Զ�������
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