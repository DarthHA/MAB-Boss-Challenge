using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MentalAIBoost.Projectiles.DestroyerEXProj;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class LunarFlareBookHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Flare");
            DisplayName.AddTranslation(GameCulture.Chinese, "月曜");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            if (!Main.projectile[(int)projectile.ai[0]].active) projectile.Kill();
            Projectile owner = Main.projectile[(int)projectile.ai[0]];
            if (owner.ai[1] == 1) projectile.Kill();
            projectile.localAI[1] += 0.1f;

            projectile.localAI[0]++;
            float r;
            if (projectile.localAI[0] < 80)
            {
                r = projectile.localAI[0] * 5;
            }
            else
            {
                r = 400;
            }
            projectile.Center = owner.Center - new Vector2(0, r);
            projectile.direction = Main.npc[(int)owner.ai[0]].direction;


        }





        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects SP = projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex1 = mod.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/LFRune");
            Texture2D tex2 = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity * 0.5f, projectile.localAI[1] + projectile.rotation, tex1.Size() / 2, projectile.scale * 2f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex2.Size() / 2, projectile.scale * 1.5f, SP, 0);
            return false;
        }
    }
}