using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.StardustSummonerProj
{
    public class StardustGuardianHostile : ModProjectile
    {
        public int AttackTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Jotaro");
            DisplayName.AddTranslation(GameCulture.Chinese, "星承守卫");
        }
        public override void SetDefaults()
        {
            projectile.width = 108;
            projectile.height = 92;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            Main.projFrames[projectile.type] = 19;   //前8帧静息，9-12聚怪，13-19欧拉
            projectile.netImportant = true;
            projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }
        private void Movement(Vector2 targetPos, float speedModifier, bool fastX = true)
        {
            if (projectile.Center.X < targetPos.X)
            {
                projectile.velocity.X += speedModifier;
                if (projectile.velocity.X < 0)
                    projectile.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                projectile.velocity.X -= speedModifier;
                if (projectile.velocity.X > 0)
                    projectile.velocity.X -= speedModifier * (fastX ? 2 : 1);
            }
            if (projectile.Center.Y < targetPos.Y)
            {
                projectile.velocity.Y += speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                projectile.velocity.Y -= speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(projectile.velocity.X) > 24)
                projectile.velocity.X = 24 * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > 24)
                projectile.velocity.Y = 24 * Math.Sign(projectile.velocity.Y);


        }

        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<StardustSummonerBoss>()) projectile.Kill();
            NPC owner = Main.npc[(int)projectile.ai[0]];
            Player target = Main.player[projectile.owner];
            if (projectile.ai[1] == 0)              //跟随召唤师
            {
                projectile.direction = owner.direction;
                projectile.spriteDirection = owner.spriteDirection;
                Vector2 Pos = owner.Center - new Vector2(20 * owner.direction, 30) - (owner.velocity * 1.5f);
                Vector2 MoveVel = Pos - projectile.Center;
                if (MoveVel.Length() > 20) MoveVel = MoveVel / MoveVel.Length() * 27;
                projectile.velocity = MoveVel;

                if (projectile.Distance(owner.Center) > 2000)
                {
                    projectile.Center = owner.Center;
                }
            }
            if (projectile.ai[1] == 1)   //锁定目标
            {
                AttackTimer++;

                projectile.direction = Math.Sign(target.Center.X - projectile.Center.X);
                projectile.spriteDirection = Math.Sign(target.Center.X - projectile.Center.X);
                Movement(target.Center - new Vector2(projectile.spriteDirection * 300, 0), 0.5f, true);
                if (AttackTimer > 120)
                {
                    projectile.ai[1] = 2;
                    AttackTimer = 0;
                }
            }
            if (projectile.ai[1] == 2)              //欧拉
            {
                AttackTimer++;
                projectile.direction = Math.Sign(target.Center.X - projectile.Center.X);
                projectile.spriteDirection = Math.Sign(target.Center.X - projectile.Center.X);
                Movement(target.Center, 0.2f, false);
                if (AttackTimer % 20 == 10)
                {
                    Main.PlaySound(SoundID.Item14, projectile.Center);
                    Vector2 vector18 = projectile.Center + new Vector2(projectile.direction * 30, 12f);
                    float scaleFactor = 8f;
                    float num56 = 0.251327425f;
                    int num57 = 0;
                    while (num57 < 3f)
                    {

                        Vector2 vec5 = Vector2.Normalize(target.Center - projectile.Center + target.velocity * 20f);
                        Vector2 vector19 = vec5 * scaleFactor;
                        vector19 = vector19.RotatedBy(num56 * num57 - (1.2566371f - num56) / 2f, default);
                        float ai = (Main.rand.NextFloat() - 0.5f) * 0.3f * MathHelper.TwoPi / 60f;
                        int num58 = NPC.NewNPC((int)vector18.X, (int)vector18.Y + 7, 522, 0, 0f, ai, vector19.X, vector19.Y, 255);
                        Main.npc[num58].velocity = vector19;
                        Main.npc[num58].damage = (int)(Main.npc[num58].damage * 1.2f);
                        Main.npc[num58].dontTakeDamage = true;
                        num57++;
                    }
                }

                if (AttackTimer > 240 - 1)
                {
                    projectile.ai[1] = 3;
                    AttackTimer = 0;
                }
            }
            if (projectile.ai[1] == 3)              //爆发攻击
            {
                AttackTimer++;
                projectile.velocity = Vector2.Zero;
                if (AttackTimer == 12)
                {
                    Main.PlaySound(SoundID.Item14, projectile.Center);
                    int protmp = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.StardustGuardianExplosion, projectile.damage, 0, Main.myPlayer);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                    foreach (Player player in Main.player)
                    {
                        player.immuneTime = 0;
                        player.immune = false;
                    }
                    Vector2 vector18 = projectile.Center + new Vector2(projectile.direction * 30, 12f);
                    float scaleFactor = 8f;
                    float num56 = 0.251327425f;
                    int num57 = 0;
                    while (num57 < 30f)
                    {

                        Vector2 vec5 = Vector2.Normalize(target.Center - projectile.Center + target.velocity * 20f);
                        Vector2 vector19 = vec5 * scaleFactor;
                        vector19 = vector19.RotatedBy(num56 * num57 - (1.2566371f - num56) / 2f, default);
                        float ai = (Main.rand.NextFloat() - 0.5f) * 0.3f * MathHelper.TwoPi / 60f;
                        int num58 = NPC.NewNPC((int)vector18.X, (int)vector18.Y + 7, 522, 0, 0f, ai, vector19.X, vector19.Y, 255);
                        Main.npc[num58].velocity = vector19;
                        Main.npc[num58].damage = (int)(Main.npc[num58].damage * 1.2f);
                        Main.npc[num58].dontTakeDamage = true;
                        num57++;
                    }

                }
                if (AttackTimer >= 120)
                {
                    projectile.ai[1] = 0;
                    AttackTimer = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            if (projectile.ai[1] < 2)
            {
                if (projectile.frame > 7) projectile.frame = 0;
                projectile.frameCounter++;
                if (projectile.frameCounter >= 8)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 7) projectile.frame = 0;
                }
            }
            if (projectile.ai[1] == 2)
            {
                if (projectile.frame < 12) projectile.frame = 12;
                projectile.frameCounter++;
                if (projectile.frameCounter >= 1)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 18) projectile.frame = 12;
                }
            }

            if (projectile.ai[1] == 3)
            {
                if (AttackTimer < 1)
                {
                    projectile.frame = 8;
                    projectile.frameCounter = 0;
                }
                else
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 4)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;
                        if (projectile.frame > 11) projectile.frame = 8;
                    }
                }
                if (AttackTimer > 16) projectile.frame = 8;
            }

            Color color29 = Color.White;
            color29.A = 100;
            Rectangle Frame = new Rectangle(0, 92 * projectile.frame, 108, 92);
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //if (projectile.ai[1] == 1) Frame = new Rectangle(0, 92 * (projectile.frame + 11), 108, 92);
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, Frame, color29, 0, new Vector2(54, 46), projectile.scale, SP, 0f);
            return false;
        }
    }
}