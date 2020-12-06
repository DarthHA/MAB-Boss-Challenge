using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Volknet
{
    /// <summary>
    /// 作者：网络核心
    /// 比较适合写波斯用
    /// </summary>
    public static class VolknetUtils
    {
    
    /// <summary>
    ///将射弹贴图绘制到中心，后面参数默认为默认值，颜色默认为原色 
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="sb"></param>
    /// <param name="color"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
        public static void DrawCenter(this Projectile projectile, SpriteBatch sb, Color? color = null, float? rotation = null, float? scale = null, int? direction = null)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, tex.Height / Main.projFrames[projectile.type] * projectile.frame, tex.Width, tex.Height / Main.projFrames[projectile.type]);
            if (scale == null)
            {
                scale = projectile.scale;
            }
            if (rotation == null)
            {
                rotation = projectile.rotation;
            }
            SpriteEffects SP;
            if (direction == null)
            {
                if (projectile.direction < 0)
                {
                    SP = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    SP = SpriteEffects.None;
                }
            }
            else
            {
                if (direction < 0)
                {
                    SP = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    SP = SpriteEffects.None;
                }
            }
            if (color == null)
            {
                color = Color.White;
            }

            sb.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), Frame,(Color)color, (float)rotation, Frame.Size() / 2, (int)scale, SP, 0);
        }

        /// <summary>
        /// 将NPC贴图绘制到中心，后面参数默认为默认值，颜色默认为原色 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="sb"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="direction"></param>
        public static void DrawCenter(this NPC npc, SpriteBatch sb, Color? color = null, float? rotation = null, float? scale = null, int? direction = null)
        {
            Texture2D tex = Main.npcTexture[npc.type];
            Rectangle Frame = npc.frame;
            if (scale == null)
            {
                scale = npc.scale;
            }
            if (rotation == null)
            {
                rotation = npc.rotation;
            }
            SpriteEffects SP;
            if (direction == null)
            {
                if (npc.direction < 0)
                {
                    SP = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    SP = SpriteEffects.None;
                }
            }
            else
            {
                if (direction < 0)
                {
                    SP = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    SP = SpriteEffects.None;
                }
            }
            if (color == null)
            {
                color = Color.White;
            }
            sb.Draw(tex, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), Frame,(Color)color, (float)rotation, Frame.Size() / 2, (int)scale, SP, 0);
        }


        /// <summary>
        /// 世界里是否有Boss存活
        /// </summary>
        /// <returns></returns>
        public static bool AnyBosses()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 是否存在指定类型射弹
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AnyProj(int type)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 使指定NPC向目标发射指定射弹
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="type"></param>
        /// <param name="speed"></param>
        /// <param name="damage"></param>
        /// <param name="IsHostile"></param>
        /// <param name="IsTileCollision"></param>
        public static void ShootProj(this NPC npc, int type, float speed, int? damage = null, bool? IsHostile = null, bool? IsTileCollision = null)
        {
            if (npc.HasValidTarget)
            {
                Vector2 TargetCenter = npc.Center;
                if (npc.HasNPCTarget)
                {
                    TargetCenter = Main.npc[npc.target].Center;
                }
                if (npc.HasPlayerTarget)
                {
                    TargetCenter = Main.player[npc.target].Center;
                }
                if (damage == null)
                {
                    damage = Main.expertMode ? npc.damage / 4 : npc.damage;
                }
                ShootProj(npc.Center, TargetCenter, type, speed, (int)damage, IsHostile, IsTileCollision);
            }
        }


        /// <summary>
        /// 在指定位置向指定位置发射射弹
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Target"></param>
        /// <param name="type"></param>
        /// <param name="speed"></param>
        /// <param name="damage"></param>
        /// <param name="IsHostile"></param>
        /// <param name="IsTileCollision"></param>
        public static void ShootProj(Vector2 Source,Vector2 Target,int type,float speed,int damage = 10, bool? IsHostile = null, bool? IsTileCollision = null)
        {

            Vector2 ShootVel;
            if (Source == Target)
            {
                ShootVel = new Vector2(0, speed);
            }
            else
            {
                ShootVel = Vector2.Normalize(Target - Source) * speed;
            }

            int protmp = Projectile.NewProjectile(Source, ShootVel, type, damage, 0, default);
            if (IsHostile != null)
            {
                Main.projectile[protmp].hostile = (bool)IsHostile;
                Main.projectile[protmp].friendly = !(bool)IsHostile;
            }
            if (IsTileCollision != null)
            {
                Main.projectile[protmp].tileCollide = (bool)IsTileCollision;
            }
        }






        /// <summary>
        /// 判断该世界坐标是否在指定墙背后，type为空时判断是否在墙后
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool InWall(Vector2 Pos, int? type = null)
        {
            if (type == null)
            {
                return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null;
            }
            else
            {
                return Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)] != null &&
                            Main.tile[(int)(Pos.X / 16), (int)(Pos.Y / 16)].wall == (int)type;
            }
        }



        /// <summary>
        /// 加速度移动，speedModifierX和speedModifierY决定加速度，fastX和fastY决定是否加速转向，XLimit和YLimit决定最大速度
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="targetPos"></param>
        /// <param name="speedModifierX"></param>
        /// <param name="speedModifierY"></param>
        /// <param name="fastX"></param>
        /// <param name="fastY"></param>
        /// <param name="XLimit"></param>
        /// <param name="YLimit"></param>
        public static void Movement(this NPC npc, Vector2 targetPos, float speedModifierX, float speedModifierY, bool fastX = true, bool fastY = true, float XLimit = 24, float YLimit = 24)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifierX;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifierX * (fastX ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifierX;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifierX * (fastX ? 2 : 1);
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifierY;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifierY * (fastY ? 2 : 1);
            }
            else
            {
                npc.velocity.Y -= speedModifierY;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifierY * (fastY ? 2 : 1);
            }
            if (Math.Abs(npc.velocity.X) > XLimit)
                npc.velocity.X = XLimit * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > YLimit)
                npc.velocity.Y = YLimit * Math.Sign(npc.velocity.Y);
        }



        /// <summary>
        /// 加速度移动，speedModifier决定加速度，fast决定是否加速转向，Limit决定最大速度
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="targetPos"></param>
        /// <param name="speedModifier"></param>
        /// <param name="fast"></param>
        /// <param name="Limit"></param>
        public static void Movement(this NPC npc, Vector2 targetPos, float speedModifier, bool fast = true, float Limit = 24)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * (fast ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * (fast ? 2 : 1);
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * (fast ? 2 : 1);
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * (fast ? 2 : 1);
            }
            if (Math.Abs(npc.velocity.X) > Limit)
                npc.velocity.X = Limit * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > Limit)
                npc.velocity.Y = Limit * Math.Sign(npc.velocity.Y);
        }



        /// <summary>
        /// 使弹幕（通常为仆从）锁定范围内的敌对NPC
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="Range"></param>
        /// <returns></returns>
        public static int HomeOnTarget(this Projectile projectile, int Range)
        {

            float homingMaximumRangeInPixels = Range;
            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile))
                {
                    float distance = projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }

        /// <summary>
        /// 点乘，不用过多解释
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float PointMulti(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
    }
}