using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MABBossChallenge
{
    public class SMPlayer : ModPlayer
    {
        public int ScreenWidth, ScreenHeight;
        public Vector2 ScreenCenter;
        public bool ScreenEffectsOn = false;
        public int Timer;
        public string Text;
        public string SubText;
        public override void Initialize()                 //300帧（5秒）时间，前60帧黑幕和视角形成，20帧出文字，最后60帧削除文字和黑幕，但是不消除视角
        {
            ScreenEffectsOn = false;
            ScreenCenter = player.Center;
            Timer = 0;
            Text = "";
            SubText = "";
            ScreenHeight = Main.screenHeight;
            ScreenWidth = Main.screenWidth;
        }
        public override void UpdateDead()
        {
            Initialze1();
        }
        public override void PreUpdateMovement()
        {
            if (ScreenEffectsOn)
            {
                player.velocity = Vector2.Zero;
                player.direction = Math.Sign(ScreenCenter.X - player.Center.X);
            }
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return !ScreenEffectsOn;
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            return !ScreenEffectsOn;
        }
        public override void ModifyScreenPosition()
        {
            if (ScreenEffectsOn && Timer > 60)
            {
                if (Timer > 240 && Timer <= 300)
                {

                    Vector2 TargetPos = ScreenCenter - new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    Vector2 SourcePos = player.Center - new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    Main.screenPosition = TargetPos + (SourcePos - TargetPos) * (Timer - 240) / 60;
                }
                if (Timer <= 240)
                {
                    Vector2 TargetPos = ScreenCenter - new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                    Main.screenPosition = TargetPos;
                }

            }
        }
        public override void SetControls()
        {
            if (ScreenEffectsOn)
            {
                player.controlUp = false;
                player.controlDown = false;
                player.controlLeft = false;
                player.controlRight = false;
                player.controlUseItem = false;
                player.gravControl = false;
                player.gravControl2 = false;
                player.controlSmart = false;
                player.controlThrow = false;
                player.controlTorch = false;
                player.controlQuickHeal = false;
                player.controlQuickMana = false;
                player.controlMount = false;
                player.controlMap = false;
                player.controlInv = false;
                player.controlUseTile = false;
                player.controlHook = false;
                Main.mapFullscreen = false;
                Main.playerInventory = false;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (ScreenEffectsOn)
            {
                if (Timer > 0)
                {
                    Timer--;
                }
                if (Timer == 0)
                {
                    ScreenEffectsOn = false;
                }
            }
        }
        public void Initialze1()
        {
            ScreenCenter = Vector2.Zero;
            ScreenEffectsOn = false;
            Timer = 0;
            Text = "";
            SubText = "";
        }
        public void Set(Vector2 BossPos, string text, string subtext = "")
        {
            ScreenEffectsOn = true;
            ScreenCenter = BossPos;
            Timer = 300;
            Text = text;
            SubText = subtext;
        }


    }
}