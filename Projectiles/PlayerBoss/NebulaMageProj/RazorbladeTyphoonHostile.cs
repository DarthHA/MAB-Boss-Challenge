using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MABBossChallenge.Projectiles.PlayerBoss.NebulaMageProj
{
    public class RazorbladeTyphoonHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Razorblade Typhoon");
            DisplayName.AddTranslation(GameCulture.Chinese, "利刃台风");
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
            projectile.localAI[0]++;
            projectile.localAI[1] += 0.1f;
            float r;
            if (projectile.localAI[0] < 60)
            {
                r = 700 / 60 * projectile.localAI[0];
            }
            else
            {
                r = 700;
            }
            float rot = projectile.ai[1] + projectile.localAI[0] / 120 * MathHelper.TwoPi;
            projectile.Center = owner.Center + rot.ToRotationVector2() * r;
            projectile.rotation = rot;

            if (projectile.localAI[0] > 80 && projectile.localAI[0] < 300) 
            {
                if (projectile.localAI[0] % 25 == 0)
                {
                    for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 3)
                    {
                        Projectile.NewProjectile(projectile.Center, (i + rot).ToRotationVector2() * 15, ModContent.ProjectileType<RazorbladeTyphoonProj>(), projectile.damage, 0, default, 1f, 15);
                    }
                    Main.PlaySound(SoundID.Item84, projectile.Center);
                }
            }
        }





        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex1 = mod.GetTexture("Projectiles/PlayerBoss/NebulaMageProj/RTRune");
            Texture2D tex2 = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity * 0.5f, projectile.localAI[1] + projectile.rotation, tex1.Size() / 2, projectile.scale * 2f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex2.Size() / 2, projectile.scale * 1.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}