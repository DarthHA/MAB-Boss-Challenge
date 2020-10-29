using MABBossChallenge.NPCs.MiniPlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.MiniPlayerBoss.MeteorPlayer
{
    public class PhraseBladeHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Phaseblade");
            DisplayName.AddTranslation(GameCulture.Chinese,"蓝色相位剑");
        }
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<MeteorPlayerBoss>())
            {
                projectile.Kill();
                return;
            }
            Player target = Main.player[owner.target];
            projectile.Center = owner.Center + new Vector2(0, 9);
            Vector2 Facing = Vector2.Normalize(target.Center - owner.Center);
            if (owner.ai[2] > 80 && owner.ai[2] % 40 < 35)
            {
                Facing = Vector2.Normalize(owner.velocity);
            }
            projectile.rotation = (float)Math.Atan2(Facing.Y, Facing.X) + MathHelper.Pi / 4;
            /*
            if (owner.ai[2] > 80 && owner.ai[2] % 40 < 35)
            {
                if (((int)(owner.ai[2] / 40)) % 2 == 1)
                {
                    float t = (int)owner.ai[2] % 40;
                    projectile.rotation += t / 35 * MathHelper.Pi / 4 * 3 - MathHelper.Pi / 8 * 3;
                }
                else
                {
                    float t = (int)owner.ai[2] % 40;
                    projectile.rotation -= t / 35 * MathHelper.Pi / 4 * 3 - MathHelper.Pi / 8 * 3;
                }
            }
            */

            if (owner.ai[1] != 2)
            {
                projectile.Kill();
            }
            if (owner.ai[1] > 5)
            {
                projectile.Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, new Vector2(0, Tex.Height), projectile.scale, SP, 0);

            return false;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage += target.defense / 2;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (target.endurance < 1 && target.endurance > 0)
            {
                damage = (int)(damage / (1 - target.endurance));
            }
            if (target.endurance >= 1)
            {
                target.statLife -= damage;
                if (target.statLife < 0) target.KillMe(PlayerDeathReason.ByNPC(NPC.FindFirstNPC(ModContent.NPCType<MeteorPlayerBoss>())), 114514, 0);
            }
            damage += target.statDefense / 2;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Burning, 60);
            target.AddBuff(BuffID.BrokenArmor, 200);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Burning, 60);
            target.AddBuff(BuffID.BrokenArmor, 200);
        }
    }
}