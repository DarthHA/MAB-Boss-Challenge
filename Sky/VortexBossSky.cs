using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace MABBossChallenge.Sky
{
    // Token: 0x02000422 RID: 1058
    public class VortexBossSky : CustomSky
    {
        // Token: 0x060024DD RID: 9437 RVA: 0x0048067C File Offset: 0x0047E87C
        public override void OnLoad()
        {
            _planetTexture = TextureManager.Load("Images/Misc/VortexSky/Planet");
            _bgTexture = TextureManager.Load("Images/Misc/VortexSky/Background");
            _boltTexture = TextureManager.Load("Images/Misc/VortexSky/Bolt");
            _flashTexture = TextureManager.Load("Images/Misc/VortexSky/Flash");
        }

        // Token: 0x060024DE RID: 9438 RVA: 0x004806CC File Offset: 0x0047E8CC
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
            if (_ticksUntilNextBolt <= 0)
            {
                _ticksUntilNextBolt = _random.Next(1, 5);
                int num = 0;
                while (_bolts[num].IsAlive && num != _bolts.Length - 1)
                {
                    num++;
                }
                _bolts[num].IsAlive = true;
                _bolts[num].Position.X = _random.NextFloat() * (Main.maxTilesX * 16f + 4000f) - 2000f;
                _bolts[num].Position.Y = _random.NextFloat() * 500f;
                _bolts[num].Depth = _random.NextFloat() * 8f + 2f;
                _bolts[num].Life = 30;
            }
            _ticksUntilNextBolt--;
            for (int i = 0; i < _bolts.Length; i++)
            {
                if (_bolts[i].IsAlive)
                {
                    Bolt[] bolts = _bolts;
                    int num2 = i;
                    bolts[num2].Life = bolts[num2].Life - 1;
                    if (_bolts[i].Life <= 0)
                    {
                        _bolts[i].IsAlive = false;
                    }
                }
            }
        }

        // Token: 0x060024DF RID: 9439 RVA: 0x00480890 File Offset: 0x0047EA90
        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, _fadeOpacity * 0.5f));
        }

        // Token: 0x060024E0 RID: 9440 RVA: 0x004808C4 File Offset: 0x0047EAC4
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * _fadeOpacity);
                spriteBatch.Draw(_bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * _fadeOpacity);
                Vector2 value = new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                Vector2 value2 = 0.01f * (new Vector2(Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
                spriteBatch.Draw(_planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * _fadeOpacity, 0f, new Vector2(_planetTexture.Width >> 1, _planetTexture.Height >> 1), 1f, SpriteEffects.None, 1f);
            }
            float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
            Vector2 value3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
            for (int i = 0; i < _bolts.Length; i++)
            {
                if (_bolts[i].IsAlive && _bolts[i].Depth > minDepth && _bolts[i].Depth < maxDepth)
                {
                    Vector2 vector = new Vector2(1f / _bolts[i].Depth, 0.9f / _bolts[i].Depth);
                    Vector2 vector2 = (_bolts[i].Position - value3) * vector + value3 - Main.screenPosition;
                    if (rectangle.Contains((int)vector2.X, (int)vector2.Y))
                    {
                        Texture2D texture = _boltTexture;
                        int life = _bolts[i].Life;
                        if (life > 26 && life % 2 == 0)
                        {
                            texture = _flashTexture;
                        }
                        float scale2 = life / 30f;
                        spriteBatch.Draw(texture, vector2, null, Color.White * scale * scale2 * _fadeOpacity, 0f, Vector2.Zero, vector.X * 5f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        // Token: 0x060024E1 RID: 9441 RVA: 0x0001A072 File Offset: 0x00018272
        public override float GetCloudAlpha()
        {
            return (1f - _fadeOpacity) * 0.3f + 0.7f;
        }

        // Token: 0x060024E2 RID: 9442 RVA: 0x00480C30 File Offset: 0x0047EE30
        public override void Activate(Vector2 position, params object[] args)
        {
            _fadeOpacity = 0.002f;
            _isActive = true;
            _bolts = new Bolt[500];
            for (int i = 0; i < _bolts.Length; i++)
            {
                _bolts[i].IsAlive = false;
            }
        }

        // Token: 0x060024E3 RID: 9443 RVA: 0x0001A08C File Offset: 0x0001828C
        public override void Deactivate(params object[] args)
        {
            _isActive = false;
        }

        // Token: 0x060024E4 RID: 9444 RVA: 0x0001A08C File Offset: 0x0001828C
        public override void Reset()
        {
            _isActive = false;
        }

        // Token: 0x060024E5 RID: 9445 RVA: 0x0001A095 File Offset: 0x00018295
        public override bool IsActive()
        {
            return _isActive || _fadeOpacity > 0.001f;
        }

        // Token: 0x040040AB RID: 16555
        private UnifiedRandom _random = new UnifiedRandom();

        // Token: 0x040040AC RID: 16556
        private Texture2D _planetTexture;

        // Token: 0x040040AD RID: 16557
        private Texture2D _bgTexture;

        // Token: 0x040040AE RID: 16558
        private Texture2D _boltTexture;

        // Token: 0x040040AF RID: 16559
        private Texture2D _flashTexture;

        // Token: 0x040040B0 RID: 16560
        private bool _isActive;

        // Token: 0x040040B1 RID: 16561
        private int _ticksUntilNextBolt;

        // Token: 0x040040B2 RID: 16562
        private float _fadeOpacity;

        // Token: 0x040040B3 RID: 16563
        private Bolt[] _bolts;

        // Token: 0x02000423 RID: 1059
        private struct Bolt
        {
            // Token: 0x040040B4 RID: 16564
            public Vector2 Position;

            // Token: 0x040040B5 RID: 16565
            public float Depth;

            // Token: 0x040040B6 RID: 16566
            public int Life;

            // Token: 0x040040B7 RID: 16567
            public bool IsAlive;
        }
    }
}
