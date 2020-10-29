using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace MABBossChallenge.Sky
{
    // Token: 0x02000417 RID: 1047
    public class NebulaBossSky : CustomSky
    {
        // Token: 0x06002496 RID: 9366 RVA: 0x0047E218 File Offset: 0x0047C418
        public override void OnLoad()
        {
            _planetTexture = TextureManager.Load("Images/Misc/NebulaSky/Planet");
            _bgTexture = TextureManager.Load("Images/Misc/NebulaSky/Background");
            _beamTexture = TextureManager.Load("Images/Misc/NebulaSky/Beam");
            _rockTextures = new Texture2D[3];
            for (int i = 0; i < _rockTextures.Length; i++)
            {
                _rockTextures[i] = TextureManager.Load("Images/Misc/NebulaSky/Rock_" + i);
            }
        }

        // Token: 0x06002497 RID: 9367 RVA: 0x0047E294 File Offset: 0x0047C494
        public override void Update(GameTime gameTime)
        {
            if (_isActive)
            {
                _fadeOpacity = Math.Min(1f, 0.01f + _fadeOpacity);
                return;
            }
            _fadeOpacity = Math.Max(0f, _fadeOpacity - 0.01f);
        }

        // Token: 0x06002498 RID: 9368 RVA: 0x0047E2E4 File Offset: 0x0047C4E4
        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, _fadeOpacity * 0.5f));
        }

        // Token: 0x06002499 RID: 9369 RVA: 0x0047E318 File Offset: 0x0047C518
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * _fadeOpacity);
                spriteBatch.Draw(_bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * _fadeOpacity));
                Vector2 value = new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Vector2 value2 = 0.01f * (new Vector2(Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
                spriteBatch.Draw(_planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * _fadeOpacity, 0f, new Vector2(_planetTexture.Width >> 1, _planetTexture.Height >> 1), 1f, SpriteEffects.None, 1f);
            }
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < _pillars.Length; i++)
            {
                float depth = _pillars[i].Depth;
                if (num == -1 && depth < maxDepth)
                {
                    num = i;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                num2 = i;
            }
            if (num == -1)
            {
                return;
            }
            Vector2 value3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
            float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
            for (int j = num; j < num2; j++)
            {
                Vector2 vector = new Vector2(1f / _pillars[j].Depth, 0.9f / _pillars[j].Depth);
                Vector2 vector2 = _pillars[j].Position;
                vector2 = (vector2 - value3) * vector + value3 - Main.screenPosition;
                if (rectangle.Contains((int)vector2.X, (int)vector2.Y))
                {
                    float num3 = vector.X * 450f;
                    spriteBatch.Draw(_beamTexture, vector2, null, Color.White * 0.2f * scale * _fadeOpacity, 0f, Vector2.Zero, new Vector2(num3 / 70f, num3 / 45f), SpriteEffects.None, 0f);
                    int num4 = 0;
                    for (float num5 = 0f; num5 <= 1f; num5 += 0.03f)
                    {
                        float num6 = 1f - (num5 + Main.GlobalTime * 0.02f + (float)Math.Sin((float)j)) % 1f;
                        spriteBatch.Draw(_rockTextures[num4], vector2 + new Vector2((float)Math.Sin(num5 * 1582f) * (num3 * 0.5f) + num3 * 0.5f, num6 * 2000f), null, Color.White * num6 * scale * _fadeOpacity, num6 * 20f, new Vector2(_rockTextures[num4].Width >> 1, _rockTextures[num4].Height >> 1), 0.9f, SpriteEffects.None, 0f);
                        num4 = (num4 + 1) % _rockTextures.Length;
                    }
                }
            }
        }

        // Token: 0x0600249A RID: 9370 RVA: 0x00019D3B File Offset: 0x00017F3B
        public override float GetCloudAlpha()
        {
            return (1f - _fadeOpacity) * 0.3f + 0.7f;
        }

        // Token: 0x0600249B RID: 9371 RVA: 0x0047E764 File Offset: 0x0047C964
        public override void Activate(Vector2 position, params object[] args)
        {
            _fadeOpacity = 0.002f;
            _isActive = true;
            _pillars = new LightPillar[40];
            for (int i = 0; i < _pillars.Length; i++)
            {
                _pillars[i].Position.X = (float)i / _pillars.Length * (Main.maxTilesX * 16f + 20000f) + _random.NextFloat() * 40f - 20f - 20000f;
                _pillars[i].Position.Y = _random.NextFloat() * 200f - 2000f;
                _pillars[i].Depth = _random.NextFloat() * 8f + 7f;
            }
            Array.Sort<LightPillar>(_pillars, new Comparison<LightPillar>(SortMethod));
        }

        // Token: 0x0600249C RID: 9372 RVA: 0x00019D55 File Offset: 0x00017F55
        private int SortMethod(LightPillar pillar1, LightPillar pillar2)
        {
            return pillar2.Depth.CompareTo(pillar1.Depth);
        }

        // Token: 0x0600249D RID: 9373 RVA: 0x00019D69 File Offset: 0x00017F69
        public override void Deactivate(params object[] args)
        {
            _isActive = false;
        }

        // Token: 0x0600249E RID: 9374 RVA: 0x00019D69 File Offset: 0x00017F69
        public override void Reset()
        {
            _isActive = false;
        }

        // Token: 0x0600249F RID: 9375 RVA: 0x00019D72 File Offset: 0x00017F72
        public override bool IsActive()
        {
            return _isActive || _fadeOpacity > 0.001f;
        }

        // Token: 0x04004060 RID: 16480
        private LightPillar[] _pillars;

        // Token: 0x04004061 RID: 16481
        private UnifiedRandom _random = new UnifiedRandom();

        // Token: 0x04004062 RID: 16482
        private Texture2D _planetTexture;

        // Token: 0x04004063 RID: 16483
        private Texture2D _bgTexture;

        // Token: 0x04004064 RID: 16484
        private Texture2D _beamTexture;

        // Token: 0x04004065 RID: 16485
        private Texture2D[] _rockTextures;

        // Token: 0x04004066 RID: 16486
        private bool _isActive;

        // Token: 0x04004067 RID: 16487
        private float _fadeOpacity;

        // Token: 0x02000418 RID: 1048
        private struct LightPillar
        {
            // Token: 0x04004068 RID: 16488
            public Vector2 Position;

            // Token: 0x04004069 RID: 16489
            public float Depth;
        }
    }
}
