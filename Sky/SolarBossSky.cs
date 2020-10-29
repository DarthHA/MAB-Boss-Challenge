using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace MABBossChallenge.Sky
{
    // Token: 0x0200041E RID: 1054
    public class SolarBossSky : CustomSky
    {
        // Token: 0x060024C7 RID: 9415 RVA: 0x00019F7A File Offset: 0x0001817A
        public override void OnLoad()
        {
            _planetTexture = TextureManager.Load("Images/Misc/SolarSky/Planet");
            _bgTexture = TextureManager.Load("Images/Misc/SolarSky/Background");
            _meteorTexture = TextureManager.Load("Images/Misc/SolarSky/Meteor");
        }

        // Token: 0x060024C8 RID: 9416 RVA: 0x0047F8E4 File Offset: 0x0047DAE4
        public override void Update(GameTime gameTime)
        {
            if (_isActive)
            {
                _fadeOpacity = Math.Min(1f, 0.01f + _fadeOpacity);
            }
            else
            {
                _fadeOpacity = Math.Max(0f, _fadeOpacity - 0.01f);
            }
            float num = 1200f;
            for (int i = 0; i < _meteors.Length; i++)
            {
                Meteor[] meteors = _meteors;
                int num2 = i;
                meteors[num2].Position.X = meteors[num2].Position.X - num * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Meteor[] meteors2 = _meteors;
                int num3 = i;
                meteors2[num3].Position.Y = meteors2[num3].Position.Y + num * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_meteors[i].Position.Y > Main.worldSurface * 16.0)
                {
                    _meteors[i].Position.X = _meteors[i].StartX;
                    _meteors[i].Position.Y = -10000f;
                }
            }
        }

        // Token: 0x060024C9 RID: 9417 RVA: 0x0047FA40 File Offset: 0x0047DC40
        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, _fadeOpacity * 0.5f));
        }

        // Token: 0x060024CA RID: 9418 RVA: 0x0047FA74 File Offset: 0x0047DC74
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
            for (int i = 0; i < _meteors.Length; i++)
            {
                float depth = _meteors[i].Depth;
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
            float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
            Vector2 value3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
            for (int j = num; j < num2; j++)
            {
                Vector2 vector = new Vector2(1f / _meteors[j].Depth, 0.9f / _meteors[j].Depth);
                Vector2 vector2 = (_meteors[j].Position - value3) * vector + value3 - Main.screenPosition;
                int num3 = _meteors[j].FrameCounter / 3;
                _meteors[j].FrameCounter = (_meteors[j].FrameCounter + 1) % 12;
                if (rectangle.Contains((int)vector2.X, (int)vector2.Y))
                {
                    spriteBatch.Draw(_meteorTexture, vector2, new Rectangle?(new Rectangle(0, num3 * (_meteorTexture.Height / 4), _meteorTexture.Width, _meteorTexture.Height / 4)), Color.White * scale * _fadeOpacity, 0f, Vector2.Zero, vector.X * 5f * _meteors[j].Scale, SpriteEffects.None, 0f);
                }
            }
        }

        // Token: 0x060024CB RID: 9419 RVA: 0x00019FAC File Offset: 0x000181AC
        public override float GetCloudAlpha()
        {
            return (1f - _fadeOpacity) * 0.3f + 0.7f;
        }

        // Token: 0x060024CC RID: 9420 RVA: 0x0047FE14 File Offset: 0x0047E014
        public override void Activate(Vector2 position, params object[] args)
        {
            _fadeOpacity = 0.002f;
            _isActive = true;
            _meteors = new Meteor[150];
            for (int i = 0; i < _meteors.Length; i++)
            {
                float num = i / (float)_meteors.Length;
                _meteors[i].Position.X = num * (Main.maxTilesX * 16f) + _random.NextFloat() * 40f - 20f;
                _meteors[i].Position.Y = _random.NextFloat() * -((float)Main.worldSurface * 16f + 10000f) - 10000f;
                if (_random.Next(3) != 0)
                {
                    _meteors[i].Depth = _random.NextFloat() * 3f + 1.8f;
                }
                else
                {
                    _meteors[i].Depth = _random.NextFloat() * 5f + 4.8f;
                }
                _meteors[i].FrameCounter = _random.Next(12);
                _meteors[i].Scale = _random.NextFloat() * 0.5f + 1f;
                _meteors[i].StartX = _meteors[i].Position.X;
            }
            Array.Sort<Meteor>(_meteors, new Comparison<Meteor>(SortMethod));
        }

        // Token: 0x060024CD RID: 9421 RVA: 0x00019FC6 File Offset: 0x000181C6
        private int SortMethod(Meteor meteor1, Meteor meteor2)
        {
            return meteor2.Depth.CompareTo(meteor1.Depth);
        }

        // Token: 0x060024CE RID: 9422 RVA: 0x00019FDA File Offset: 0x000181DA
        public override void Deactivate(params object[] args)
        {
            _isActive = false;
        }

        // Token: 0x060024CF RID: 9423 RVA: 0x00019FDA File Offset: 0x000181DA
        public override void Reset()
        {
            _isActive = false;
        }

        // Token: 0x060024D0 RID: 9424 RVA: 0x00019FE3 File Offset: 0x000181E3
        public override bool IsActive()
        {
            return _isActive || _fadeOpacity > 0.001f;
        }

        // Token: 0x04004092 RID: 16530
        private UnifiedRandom _random = new UnifiedRandom();

        // Token: 0x04004093 RID: 16531
        private Texture2D _planetTexture;

        // Token: 0x04004094 RID: 16532
        private Texture2D _bgTexture;

        // Token: 0x04004095 RID: 16533
        private Texture2D _meteorTexture;

        // Token: 0x04004096 RID: 16534
        private bool _isActive;

        // Token: 0x04004097 RID: 16535
        private Meteor[] _meteors;

        // Token: 0x04004098 RID: 16536
        private float _fadeOpacity;

        // Token: 0x0200041F RID: 1055
        private struct Meteor
        {
            // Token: 0x04004099 RID: 16537
            public Vector2 Position;

            // Token: 0x0400409A RID: 16538
            public float Depth;

            // Token: 0x0400409B RID: 16539
            public int FrameCounter;

            // Token: 0x0400409C RID: 16540
            public float Scale;

            // Token: 0x0400409D RID: 16541
            public float StartX;
        }
    }
}
