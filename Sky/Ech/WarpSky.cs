using MABBossChallenge.NPCs.EchDestroyer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace MABBossChallenge.Sky.Ech
{
    public class WarpSky : CustomSky
	{
		private bool isActive = true;
		private float intensity;

		Line[] lines = new Line[3];

		public override void Update(GameTime gameTime)
		{
            if (!NPC.AnyNPCs(ModContent.NPCType<EchDestroyerHead>()))
            {
				EchDestroyerHead.TransBG = false;
            }
            if (!isActive)
            {
                EchDestroyerHead.TransBG = false;
            }
            //Main.NewText(intensity);
            if (EchDestroyerHead.TransBG)         //开幕
			{
				intensity += 0.03f;
			}
			else
			{
				intensity -= 0.01f;
			}
			intensity = Terraria.Utils.Clamp(intensity, 0f, 1f);

		}



		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
            if (maxDepth >= 0 && minDepth < 0)
            {
                Texture2D tex = MABBossChallenge.Instance.GetTexture("Sky/Ech/WarpSky");
                spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * intensity);

                UpdateLine();
                foreach (Line line in lines)
                {
                    spriteBatch.Draw(Main.magicPixel, new Rectangle((int)line.Pos.X, (int)line.Pos.Y, (int)line.Length, (int)line.Width), Color.White * intensity);
                }
            }
		}


		public override float GetCloudAlpha()
		{
			return 0f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			isActive = true;
		}

		public override void Deactivate(params object[] args)
		{
			isActive = false;
		}

		public override void Reset()
		{
			isActive = false;
		}

		public override bool IsActive()
		{
			return isActive || intensity > 0f;
		}


        public void UpdateLine()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 20)
                {
                    bool flag = false;
                    while (!flag)
                    {
                        float len = Main.rand.Next(450) + 100;
                        float width = Main.rand.Next(4) + 2;
                        Vector2 Pos = new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight));
                        if (Pos.X + len < Main.screenWidth && Pos.Y + width < Main.screenHeight)
                        {
                            lines[i] = new Line(Pos, len, width);
                            flag = true;
                        }
                    }
                }
                else
                {
                    lines[i].Pos.X += lines[i].Length * 0.35f;
                    lines[i].Length *= 0.85f;

                }
            }
            /*
            while (i < lines.Length)
            {
                float len = Main.rand.Next(150) + 25;
                float width = Main.rand.Next(2) + 1;
                Vector2 Pos = new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight));
                if (Pos.X + len < Main.screenWidth && Pos.Y + width < Main.screenHeight)
                {
                    lines[i] = new Line(Pos, len, width);
                    i++;
                }
            }
            */
        }




        private struct Line
        {
            public Vector2 Pos;
            public float Length;
            public float Width;
            public Line(Vector2 vec, float len, float wid)
            {
                Pos = vec;
                Length = len;
                Width = wid;
            }
        }
    }
}