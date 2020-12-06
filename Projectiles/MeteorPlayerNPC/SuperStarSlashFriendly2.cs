using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Utilities;
using System;

namespace MABBossChallenge.Projectiles.MeteorPlayerNPC
{
    public class SuperStarSlashFriendly2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Star Slash");
            DisplayName.AddTranslation(GameCulture.Chinese, "³¬¼¶ÐÇÐÇ");
        }
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1f + Main.rand.Next(30) * 0.01f;
            projectile.extraUpdates = 2;
            //projectile.timeLeft = 10 * projectile.MaxUpdates;
            projectile.timeLeft = 400;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }


        public override void AI()
        {
            projectile.alpha -= 10;
            if (projectile.alpha < 100)
            {
                projectile.alpha = 100;
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                Main.PlaySound(SoundID.Item9, projectile.position);
            }

            Vector2 Center = new Vector2(projectile.localAI[0], projectile.localAI[1]);
            projectile.Center = Utils.NPCUtils.RotPos(projectile.Center, Center, MathHelper.Pi / 360 * Math.Sign(projectile.ai[1]));
            projectile.velocity = Vector2.Normalize(projectile.Center - Center) * projectile.velocity.Length();
            if (projectile.velocity.Length() < 4)
            {
                projectile.velocity *= 1.03f;
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Dazed, 30);
            target.AddBuff(BuffID.BrokenArmor, 180);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
        public override void Kill(int timeLeft)
        {
			Main.PlaySound(SoundID.Item10, projectile.position);
			int num6;
			for (int num546 = 0; num546 < 10; num546 = num6 + 1)
			{
				Dust dust37 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 279, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.2f);
				dust37.noGravity = true;
				ref float ptr = ref dust37.velocity.X;
				ptr *= 2f;
				num6 = num546;
			}
		}
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 200) * projectile.Opacity, projectile.rotation + MathHelper.Pi / 2, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);

			return false;
        }
    }
}