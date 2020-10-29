using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace MABBossChallenge.Sky
{
    public class CustomScreenShaderData : ScreenShaderData
    {
        public CustomScreenShaderData(string passName) : base(passName)
        {
        }
        public CustomScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Apply()
        {
            base.Apply();
        }
    }
}
