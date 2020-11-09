
using MABBossChallenge.NPCs.PlayerBoss;
using MABBossChallenge.Walls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge
{
    public class MABItemProjectile : GlobalProjectile
    {




        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.IceBlock ||
                projectile.type == ProjectileID.SandBallFalling ||
                projectile.type == ProjectileID.SandBallGun ||
                projectile.type == ProjectileID.PearlSandBallFalling ||
                projectile.type == ProjectileID.EbonsandBallFalling ||
                projectile.type == ProjectileID.EbonsandBallGun ||
                projectile.type == ProjectileID.PearlSandBallGun ||
                projectile.type == ProjectileID.CrimsandBallFalling ||
                projectile.type == ProjectileID.CrimsandBallGun)
            {
                int PosX = (int)(projectile.position.X / 16);
                int PosY = (int)(projectile.position.Y / 16);
                if (Main.tile[PosX, PosY].wall == ModContent.WallType<ArenaWall>())
                {
                    projectile.active = false;
                }
            }


            if (NPC.AnyNPCs(ModContent.NPCType<VortexRangerBoss>()) && projectile.type == ProjectileID.VortexVortexPortal)
            {
                projectile.active = false;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<StardustSummonerBoss>()) && projectile.type == ProjectileID.StardustTowerMark)
            {
                projectile.active = false;
            }

            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.type == ProjectileID.Electrosphere)
            {
                target.AddBuff(BuffID.Electrified, 120);
            }
        }


    }

}


