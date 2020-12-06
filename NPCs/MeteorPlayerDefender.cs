using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.NPCs
{
    public class MeteorPlayerDefender : ModNPC
    {
        private Vector2 SpawnPos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Guardian");
            DisplayName.AddTranslation(GameCulture.Chinese, "陨石守卫");
            Main.npcFrameCount[npc.type] = 20;
        }
        public override void SetDefaults()
        {
            npc.chaseable = false;
            npc.width = 36;
            npc.height = 52;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 7000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.hide = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax /= 2;
        }
        public override void AI()
        {
            if (SpawnPos == Vector2.Zero)
            {
                int XPos = (int)(npc.Center.X / 16);
                int YPos = (int)(npc.Center.Y / 16);
                int x1 = -150, x2 = 150;
                if (XPos + x1 < 0) x1 = 1 - XPos;
                if (XPos + x2 > Main.maxTilesX) x2 = Main.maxTilesX - XPos - 1;
                while (Math.Abs(x1 - x2) > 20)
                {
                    int px = (x1 + x2) / 2;
                    if (MeteorCount(x1 + XPos, YPos - 75, px + XPos, YPos + 75) >= MeteorCount(px + XPos, YPos - 75, x2 + XPos, YPos + 75))
                    {
                        x2 = px;
                    }
                    else
                    {
                        x1 = px;
                    }
                }
                int TruePosX = (x1 + x2) / 2;
                int TruePosY = 114514;
                for (int j = -100; j <= 100; j++)
                {
                    for (int i = x1; i <= x2; i++)
                    {
                        if (Valid(XPos + i, YPos + j))
                        {
                            if (Main.tile[XPos + i, YPos + j].type == TileID.Meteorite && Main.tile[XPos + i, YPos + j].active())
                            {
                                TruePosY = j;
                                break;
                            }
                        }
                    }
                    if (TruePosY != 114514) break;
                }

                SpawnPos = new Vector2((XPos + TruePosX) * 16, (YPos + TruePosY) * 16 - 500);
                npc.ai[0]++;
                npc.Center = SpawnPos;
                npc.localAI[0] = (Main.rand.Next(2) * 2 - 1);
                npc.hide = false;
                npc.dontTakeDamage = false;
            }
            if (npc.ai[0] == 1)
            {
                if (npc.ai[1] == 0)           //移动状态
                {
                    npc.velocity.X = npc.localAI[0] * 5;
                    if (npc.Center.X > SpawnPos.X + 800 && npc.velocity.X > 0)
                    {
                        npc.ai[1] = 1;
                    }
                    if (npc.Center.X < SpawnPos.X - 800 && npc.velocity.X < 0)
                    {
                        npc.ai[1] = 1;
                    }
                }
                if (npc.ai[1] == 1)
                {
                    npc.velocity.X = 0.0001f;
                    npc.ai[2]++;
                    if (npc.ai[2] > 240)
                    {
                        npc.ai[2] = 0;
                        npc.localAI[0] = -npc.localAI[0];
                        npc.ai[1] = 0;
                    }
                }
            }

            if (npc.life < npc.lifeMax * 0.9f)
            {
                npc.Transform(ModContent.NPCType<MeteorPlayerBoss>());
            }

            if (npc.velocity.Length() > 3 && Main.rand.Next(3) > 0)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
            }
            if (Main.rand.Next(3) == 1)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position + new Vector2(0, npc.height), npc.width, 10, 6, 0f, 0f, 100, default, 2f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity.Y += 5f;
            }

            npc.direction = npc.spriteDirection = Math.Sign(npc.localAI[0]);
            npc.velocity.Y = 0;
        }

        public override bool CanChat()
        {
            return true;
        }
        /*
        public override string GetChat()
        {
            if (!MABWorld.DownedMeteorPlayer)
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                        return "我存在的意义就是为了保护陨石！";
                        break;
                    case 1:
                        return "外来人，不要企图用你的十字镐或者炸药去碰那陨石！";
                        break;
                    case 2:
                        return "我不希望任何一块陨石落入别人的手中";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                return "外来人，你证明了你的实力，现在我不会阻碍你了。";
            }
            return "";
        }
        */

        public override string GetChat()
        {
            return "... ...";
        }
        public override void NPCLoot()
        {
            int protmp = Projectile.NewProjectile(npc.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 5, ProjectileID.Tombstone, 0, 0, Main.myPlayer);
            //Main.projectile[protmp].miscText = "陨石守护者 被击败了，凶手是" + Main.LocalPlayer.name + "。";
        }
        private void DP(SpriteBatch spritebatch, Vector2 Pos, Color a)
        {

            Rectangle Frame = new Rectangle(0, 56 * 5, 40, 56);
            SpriteEffects SP = npc.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Legs");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Frame = new Rectangle(0, 56 * 6, 40, 56);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Body");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);
            Frame = new Rectangle(0, 56 * 5, 40, 56);
            Tex = MABBossChallenge.Instance.GetTexture("NPCs/MiniPlayerBoss/MeteorPlayerBoss_Head");
            spritebatch.Draw(Tex, Pos, Frame, a, 0, Frame.Size() / 2, 1, SP, 0);

            Tex = Main.itemTexture[ItemID.SpaceGun];
            if (SP == SpriteEffects.None)
            {
                spritebatch.Draw(Tex, npc.Center - Main.screenPosition, null, a, MathHelper.Pi / 6, new Vector2(0, Tex.Height / 2), npc.scale, SP, 0);
            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spritebatch.Draw(Tex, npc.Center - Main.screenPosition, null, a, -MathHelper.Pi / 6, new Vector2(Tex.Width, Tex.Height / 2), npc.scale, SP, 0);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            DP(spriteBatch, npc.Center - Main.screenPosition, drawColor);
            return false;
        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 1;
            crit = false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = 1;
            crit = false;

        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            npc.Transform(ModContent.NPCType<MeteorPlayerBoss>());
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            npc.Transform(ModContent.NPCType<MeteorPlayerBoss>());
            if (!projectile.minion)
            {
                projectile.timeLeft = 0;
            }
        }


        private bool Valid(int x, int y)
        {
            return x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;
        }


        private int MeteorCount(int x1, int y1, int x2, int y2)
        {
            int X1 = Math.Max(x1, x2);
            int X2 = Math.Min(x1, x2);
            int Y1 = Math.Max(y1, y2);
            int Y2 = Math.Min(y1, y2);
            int num = 0;
            for (int i = X2; i <= X1; i++)
            {
                for (int j = Y2; j <= Y1; j++)
                {
                    if (Main.tile[i, j].type == TileID.Meteorite)
                    {
                        num++;
                    }
                }
            }
            return num;
        }
    }
}