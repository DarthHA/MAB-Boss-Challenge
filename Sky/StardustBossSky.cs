using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria;

namespace MABBossChallenge.Sky
{
	// Token: 0x02000420 RID: 1056
	public class StardustBossSky : CustomSky
	{
		// Token: 0x060024D2 RID: 9426 RVA: 0x0047FFC4 File Offset: 0x0047E1C4
		public override void OnLoad()
		{
			_planetTexture = TextureManager.Load("Images/Misc/StarDustSky/Planet");
			_bgTexture = TextureManager.Load("Images/Misc/StarDustSky/Background");
			_starTextures = new Texture2D[2];
			for (int i = 0; i < _starTextures.Length; i++)
			{
				_starTextures[i] = TextureManager.Load("Images/Misc/StarDustSky/Star " + i);
			}
		}

		// Token: 0x060024D3 RID: 9427 RVA: 0x00480030 File Offset: 0x0047E230
		public override void Update(GameTime gameTime)
		{
			if (_isActive)
			{
				_fadeOpacity = Math.Min(1f, 0.01f + _fadeOpacity);
				return;
			}
			_fadeOpacity = Math.Max(0f, _fadeOpacity - 0.01f);
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x00480080 File Offset: 0x0047E280
		public override Color OnTileColor(Color inColor)
		{
			Vector4 value = inColor.ToVector4();
			return new Color(Vector4.Lerp(value, Vector4.One, _fadeOpacity * 0.5f));
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x004800B4 File Offset: 0x0047E2B4
		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
			{
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * _fadeOpacity);
				spriteBatch.Draw(_bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * _fadeOpacity));
				Vector2 value = new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
				Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
				spriteBatch.Draw(_planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * _fadeOpacity, 0f, new Vector2((float)(_planetTexture.Width >> 1), (float)(_planetTexture.Height >> 1)), 1f, SpriteEffects.None, 1f);
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < _stars.Length; i++)
			{
				float depth = _stars[i].Depth;
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
			Vector2 value3 = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				Vector2 vector = new Vector2(1f / _stars[j].Depth, 1.1f / _stars[j].Depth);
				Vector2 vector2 = (_stars[j].Position - value3) * vector + value3 - Main.screenPosition;
				if (rectangle.Contains((int)vector2.X, (int)vector2.Y))
				{
					float num3 = (float)Math.Sin((double)(_stars[j].AlphaFrequency * Main.GlobalTime + _stars[j].SinOffset)) * _stars[j].AlphaAmplitude + _stars[j].AlphaAmplitude;
					float num4 = (float)Math.Sin((double)(_stars[j].AlphaFrequency * Main.GlobalTime * 5f + _stars[j].SinOffset)) * 0.1f - 0.1f;
					num3 = MathHelper.Clamp(num3, 0f, 1f);
					Texture2D texture2D = _starTextures[_stars[j].TextureIndex];
					spriteBatch.Draw(texture2D, vector2, null, Color.White * scale * num3 * 0.8f * (1f - num4) * _fadeOpacity, 0f, new Vector2((float)(texture2D.Width >> 1), (float)(texture2D.Height >> 1)), (vector.X * 0.5f + 0.5f) * (num3 * 0.3f + 0.7f), SpriteEffects.None, 0f);
				}
			}
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x0001A00F File Offset: 0x0001820F
		public override float GetCloudAlpha()
		{
			return (1f - _fadeOpacity) * 0.3f + 0.7f;
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x004804E8 File Offset: 0x0047E6E8
		public override void Activate(Vector2 position, params object[] args)
		{
			_fadeOpacity = 0.002f;
			_isActive = true;
			int num = 200;
			int num2 = 10;
			_stars = new Star[num * num2];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				float num4 = (float)i / (float)num;
				for (int j = 0; j < num2; j++)
				{
					float num5 = (float)j / (float)num2;
					_stars[num3].Position.X = num4 * (float)Main.maxTilesX * 16f;
					_stars[num3].Position.Y = num5 * ((float)Main.worldSurface * 16f + 2000f) - 1000f;
					_stars[num3].Depth = _random.NextFloat() * 8f + 1.5f;
					_stars[num3].TextureIndex = _random.Next(_starTextures.Length);
					_stars[num3].SinOffset = _random.NextFloat() * 6.28f;
					_stars[num3].AlphaAmplitude = _random.NextFloat() * 5f;
					_stars[num3].AlphaFrequency = _random.NextFloat() + 1f;
					num3++;
				}
			}
			Array.Sort<Star>(_stars, new Comparison<Star>(SortMethod));
		}

		// Token: 0x060024D8 RID: 9432 RVA: 0x0001A029 File Offset: 0x00018229
		private int SortMethod(Star meteor1, Star meteor2)
		{
			return meteor2.Depth.CompareTo(meteor1.Depth);
		}

		// Token: 0x060024D9 RID: 9433 RVA: 0x0001A03D File Offset: 0x0001823D
		public override void Deactivate(params object[] args)
		{
			_isActive = false;
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x0001A03D File Offset: 0x0001823D
		public override void Reset()
		{
			_isActive = false;
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x0001A046 File Offset: 0x00018246
		public override bool IsActive()
		{
			return _isActive || _fadeOpacity > 0.001f;
		}

		// Token: 0x0400409E RID: 16542
		private UnifiedRandom _random = new UnifiedRandom();

		// Token: 0x0400409F RID: 16543
		private Texture2D _planetTexture;

		// Token: 0x040040A0 RID: 16544
		private Texture2D _bgTexture;

		// Token: 0x040040A1 RID: 16545
		private Texture2D[] _starTextures;

		// Token: 0x040040A2 RID: 16546
		private bool _isActive;

		// Token: 0x040040A3 RID: 16547
		private Star[] _stars;

		// Token: 0x040040A4 RID: 16548
		private float _fadeOpacity;

		// Token: 0x02000421 RID: 1057
		private struct Star
		{
			// Token: 0x040040A5 RID: 16549
			public Vector2 Position;

			// Token: 0x040040A6 RID: 16550
			public float Depth;

			// Token: 0x040040A7 RID: 16551
			public int TextureIndex;

			// Token: 0x040040A8 RID: 16552
			public float SinOffset;

			// Token: 0x040040A9 RID: 16553
			public float AlphaFrequency;

			// Token: 0x040040AA RID: 16554
			public float AlphaAmplitude;
		}
	}
}
