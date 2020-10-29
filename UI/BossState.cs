using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace MABBossChallenge.UI
{
    internal class BossState : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
        private UIElement area;

        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding. You can use a UIPanel if you wish for a background.
            area = new UIElement();
            area.Left.Set(20, 1f); // Place the resource bar to the left of the hearts.
            area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
            area.Width.Set(20, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
            area.Height.Set(20, 0f);

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // This prevents drawing unless we are using an ExampleDamageItem
            if (Main.LocalPlayer.GetModPlayer<SMPlayer>().ScreenEffectsOn)
            {
            }
            else
            {
                return;
            }

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            SMPlayer modplayer = Main.LocalPlayer.GetModPlayer<SMPlayer>();
            int ScreenWidth = modplayer.ScreenWidth;
            int ScreenHeight = modplayer.ScreenHeight;
            int Timer = Terraria.Utils.Clamp(300 - modplayer.Timer, 0, 60);
            if (modplayer.Timer < 30)
            {
                Timer = modplayer.Timer * 2;
            }
            spriteBatch.Draw(Main.magicPixel, new Vector2(0, 0), new Rectangle(0, 0, ScreenWidth, ScreenHeight * Timer / 180), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Main.magicPixel, new Vector2(0, ScreenHeight - ScreenHeight * Timer / 180), new Rectangle(0, 0, ScreenWidth, ScreenHeight * Timer / 180), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (modplayer.Timer < 220 && modplayer.Timer > 60)
            {
                float alpha;
                if (modplayer.Timer > 190)
                {
                    alpha = (float)(220 - modplayer.Timer) / 30;
                }
                else
                {
                    alpha = 1;
                }
                Color TextColor = new Color(255, 255, 255) * alpha;
                Terraria.Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, modplayer.Text, 200, ScreenHeight / 6 * 5, TextColor, Color.Black, Vector2.Zero, 2);
                Terraria.Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, modplayer.SubText, 200, ScreenHeight / 6 * 5 + 100, TextColor, Color.Black, Vector2.Zero, 1.2f);
            }
        }

    }
}
