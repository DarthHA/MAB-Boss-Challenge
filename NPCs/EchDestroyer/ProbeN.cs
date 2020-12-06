using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs.EchDestroyer
{
    public class ProbeN : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("探测器");
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 30;
            npc.aiStyle = -1;
            npc.damage = 120;
            npc.defense = 30;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.knockBackResist = 0.3f;
            npc.noTileCollide = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage /= 2;
        }


        public override void AI()
        {
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            if (Main.player[npc.target].position.X <= npc.position.X)
            {
                npc.direction = -1;
                npc.spriteDirection = -1;
            }
            else
            {
                npc.direction = 1;
                npc.spriteDirection = 1;
            }
            float num = 10f;
            float num2 = 0.05f;
            Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num4 = Main.player[npc.target].position.X + Main.player[npc.target].width / 2;
            float num5 = Main.player[npc.target].position.Y + Main.player[npc.target].height / 2;
            num4 = (int)(num4 / 8f) * 8;
            num5 = (int)(num5 / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (num7 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.023f;
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y - 0.023f;
                }
                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X = npc.velocity.X + 0.023f;
                }
                else
                {
                    npc.velocity.X = npc.velocity.X - 0.023f;
                }
                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }
            }
            if (Main.player[npc.target].dead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X = npc.velocity.X + num2;
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X = npc.velocity.X - num2;
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y = npc.velocity.Y + num2;
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y = npc.velocity.Y - num2;
            }
            npc.localAI[0] += 1f;
            if (npc.justHit)
            {
                npc.localAI[0] = 0f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
            {
                npc.localAI[0] = 0f;
                int num8 = 65;
                if (Main.expertMode)
                {
                    num8 = 32;
                }
                int num9 = 84;
                int protmp = Projectile.NewProjectile(vector.X, vector.Y, num4, num5, num9, num8, 0f, Main.myPlayer, 0f, 0f);
                Main.projectile[protmp].tileCollide = false;
            }
            int num10 = (int)npc.position.X + npc.width / 2;
            int num11 = (int)npc.position.Y + npc.height / 2;
            num10 /= 16;
            num11 /= 16;
            if (!WorldGen.SolidTile(num10, num11))
            {
                Lighting.AddLight((int)((npc.position.X + npc.width / 2) / 16f), (int)((npc.position.Y + npc.height / 2) / 16f), 0.3f, 0.1f, 0.05f);
            }
            if (num4 > 0f)
            {
                npc.rotation = (float)Math.Atan2(num5, num4);
            }
            if (num4 < 0f)
            {
                npc.rotation = (float)Math.Atan2(num5, num4) + 3.14f;
            }
            float num12 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -num12;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5)
                {
                    npc.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < 12f)
                    {
                        npc.velocity.X = npc.velocity.X * 1.05f;
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }
            if (Main.dayTime || Main.player[npc.target].dead)
            {
                npc.velocity.Y = npc.velocity.Y - num2 * 2f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int num5;
            if (npc.life <= 0)
            {
                for (int num652 = 0; num652 < 10; num652 = num5 + 1)
                {
                    int num653 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default, 1.5f);
                    Dust dust = Main.dust[num653];
                    dust.velocity *= 1.4f;
                    num5 = num652;
                }
                for (int num654 = 0; num654 < 5; num654 = num5 + 1)
                {
                    int num655 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 2.5f);
                    Main.dust[num655].noGravity = true;
                    Dust dust = Main.dust[num655];
                    dust.velocity *= 5f;
                    num655 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 1.5f);
                    dust = Main.dust[num655];
                    dust.velocity *= 3f;
                    num5 = num654;
                }
                int num656 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[num656];
                gore.velocity *= 0.4f;
                Gore gore18 = Main.gore[num656];
                gore18.velocity.X += 1f;
                Gore gore19 = Main.gore[num656];
                gore19.velocity.Y += 1f;
                num656 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                gore = Main.gore[num656];
                gore.velocity *= 0.4f;
                Gore gore20 = Main.gore[num656];
                gore20.velocity.X -= 1f;
                Gore gore21 = Main.gore[num656];
                gore21.velocity.Y += 1f;
                num656 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                gore = Main.gore[num656];
                gore.velocity *= 0.4f;
                Gore gore22 = Main.gore[num656];
                gore22.velocity.X += 1f;
                Gore gore23 = Main.gore[num656];
                gore23.velocity.Y -= 1f;
                num656 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default, Main.rand.Next(61, 64), 1f);
                gore = Main.gore[num656];
                gore.velocity *= 0.4f;
                Gore gore24 = Main.gore[num656];
                gore24.velocity.X -= 1f;
                Gore gore25 = Main.gore[num656];
                gore25.velocity.Y -= 1f;
                return;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D ProbeTexture = mod.GetTexture("NPCs/EchDestroyer/ProbeGlow");
            SpriteEffects spriteEffects = (base.npc.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color alpha = npc.GetAlpha(drawColor);
            Color color = Lighting.GetColor((int)(npc.position.X + npc.width * 0.5) / 16, (int)((npc.position.Y + npc.height * 0.5) / 16));
            Texture2D texture2D = Main.npcTexture[npc.type];
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y = num * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y, texture2D.Width, num);
            Vector2 origin = rectangle.Size() / 2f;
            int num2 = 8;
            int num3 = 2;
            int num4 = 1;
            float num5 = 0f;
            int num6 = num4;
            while (((num3 > 0 && num6 < num2) || (num3 < 0 && num6 > num2)) && Lighting.NotRetro)
            {
                Color color2 = npc.GetAlpha(color);
                float num7 = num2 - num6;
                if (num3 < 0)
                {
                    num7 = num4 - num6;
                }
                color2 *= num7 / (NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value = npc.oldPos[num6];
                float rotation = npc.rotation;
                Main.spriteBatch.Draw(texture2D, value + npc.Size / 2f - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(base.npc.frame), color2, rotation + base.npc.rotation * num5 * (float)(num6 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin, npc.scale, spriteEffects, 0f);
                num6 += num3;
            }
            SpriteEffects effects = (npc.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(base.npc.frame), alpha, base.npc.rotation, base.npc.frame.Size() / 2f, base.npc.scale, effects, 0f);
            spriteBatch.Draw(ProbeTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(base.npc.frame), new Color(255, 255, 255, 0), base.npc.rotation, base.npc.frame.Size() / 2f, base.npc.scale, effects, 0f);
            return false;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool CheckDead()
        {
            return true;
        }
        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
