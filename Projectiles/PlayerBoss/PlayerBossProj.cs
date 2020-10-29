using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss
{
    public class PlayerBossProj : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public bool SpecialProj = false;
        public override bool PreAI(Projectile projectile)
        {
            if (SpecialProj)
            {
                switch (projectile.type)
                {
                    case ProjectileID.Daybreak:
                    case ProjectileID.DD2FlameBurstTowerT3Shot:
                    case ProjectileID.CultistBossFireBall:
                    case ProjectileID.StarWrath:
                        {
                            Player target = Main.player[projectile.owner];
                            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
                            if (projectile.type == ProjectileID.StarWrath) projectile.tileCollide = false;
                            if (projectile.type == ProjectileID.DD2FlameBurstTowerT3Shot) if (projectile.timeLeft > 300) projectile.timeLeft = 300;
                            if (projectile.type == ProjectileID.Daybreak)
                            {
                                if (projectile.timeLeft > 300) projectile.timeLeft = 300;
                                if (projectile.ai[1] >= 45f)
                                {
                                    projectile.velocity.Y -= 0.15f;
                                    projectile.velocity.X /= 0.99f;
                                }
                                if (projectile.scale == 1.987f)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust = Main.dust[Dust.NewDust(projectile.Center, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f)];
                                        dust.noGravity = true;
                                        dust.scale = 1.7f;
                                        dust.fadeIn = 0.5f;
                                        dust.velocity *= 5f;
                                        //dust.shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.ArmorSetDye(), Main.LocalPlayer);
                                    }
                                    projectile.tileCollide = false;
                                }
                            }
                        }

                        break;

                    case ProjectileID.MoonlordArrowTrail:
                    case ProjectileID.MoonlordBullet:
                        {
                            Player target = Main.player[projectile.owner];
                            projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
                        }
                        break;
                    case ProjectileID.Electrosphere:
                        {
                            if (projectile.localAI[1] < 70)
                            {
                                projectile.localAI[1]++;
                            }
                            if (projectile.localAI[1] == 60)
                            {
                                if (Main.rand.NextBool())
                                {
                                    Player target = Main.player[projectile.owner];
                                    Vector2 vector128 = Vector2.Normalize(target.Center - projectile.Center) * 8;
                                    float ai2 = Main.rand.Next(80);
                                    int protmp = Projectile.NewProjectile(projectile.Center.X - vector128.X, projectile.Center.Y - vector128.Y, vector128.X, vector128.Y, ProjectileID.VortexLightning, (int)(projectile.damage * 0.8f), 1f, Main.myPlayer, (float)Math.Atan2(vector128.Y, vector128.X), ai2);
                                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            return true;
        }











        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            if (SpecialProj)
            {
                bool SFP2 = false;
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<SolarFighterBoss>() && npc.ai[0] > 2)
                    {
                        SFP2 = true;
                        break;
                    }
                }


                switch (projectile.type)
                {
                    case ProjectileID.Daybreak:
                        target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
                        target.AddBuff(BuffID.OnFire, 300);
                        break;
                    case ProjectileID.SolarCounter:
                    case ProjectileID.DD2FlameBurstTowerT3Shot:
                        if (SFP2)
                            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
                        target.AddBuff(BuffID.OnFire, 300);
                        break;
                    case ProjectileID.StarWrath:
                        target.AddBuff(BuffID.Dazed, (Main.rand.Next(3) + 3) * 60);
                        target.AddBuff(BuffID.OnFire, 300);
                        break;
                    case ProjectileID.IchorBullet:
                        target.AddBuff(BuffID.Ichor, (Main.rand.Next(3) + 3) * 60);
                        break;
                    case ProjectileID.CursedBullet:
                        target.AddBuff(BuffID.CursedInferno, (Main.rand.Next(3) + 3) * 60);
                        break;
                    case ProjectileID.Electrosphere:
                    case ProjectileID.VortexLightning:
                        target.AddBuff(BuffID.Electrified, (Main.rand.Next(3) + 2) * 60);
                        break;
                    case ProjectileID.HallowStar:
                        target.AddBuff(BuffID.Dazed, 60);
                        break;
                    case ProjectileID.Flames:
                    case ProjectileID.GreekFire2:
                        target.AddBuff(BuffID.OnFire, 300);
                        break;

                    default:
                        break;
                }
            }
            if (SpecialProj)
            {
                switch (projectile.type)
                {
                    case ProjectileID.GreenLaser:
                        target.AddBuff(BuffID.Burning, 60);
                        break;
                    case ProjectileID.MoonlordBullet:
                        break;
                    case ProjectileID.MoonlordArrow:
                        target.AddBuff(BuffID.BrokenArmor, 300);
                        break;
                    default:
                        break;
                }
            }

            if (SpecialProj)
            {
                switch (projectile.type)
                {
                    case ProjectileID.BloodRain:
                        target.AddBuff(BuffID.Bleeding, 180);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}