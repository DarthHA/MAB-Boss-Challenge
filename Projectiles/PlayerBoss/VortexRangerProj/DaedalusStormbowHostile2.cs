using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MABBossChallenge.Projectiles.PlayerBoss.VortexRangerProj
{
    public class DaedalusStormbowHostile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Stormbow");
            DisplayName.AddTranslation(GameCulture.Chinese,"代达罗斯风暴弓");
        }
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 460;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (projectile.ai[0] > 200 || projectile.ai[0] < 0)
            {
                projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<VortexRangerBoss>())
            {
                projectile.Kill();
                return;
            }
            Player target = Main.player[owner.target];
            projectile.alpha = owner.alpha;
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center + new Vector2(0, -4);
            float Facing = Math.Abs(target.Center.X - owner.Center.X);
            if (Facing == 0) Facing = 0.01f;
            if (Facing > 1000) Facing = 1000;
            projectile.rotation = -MathHelper.Pi / 2 + MathHelper.Pi / 16 * projectile.spriteDirection * Facing / 300;
            projectile.ai[1]++;

            if (projectile.ai[1] > 100)
            {
                if (projectile.ai[1] % 40 == 20)
                {
                    Main.PlaySound(SoundID.Item5, projectile.position);
                }
                if (projectile.ai[1] % 4 == 1)
                {

                    Vector2 Pos = target.Center + new Vector2(Main.rand.NextFloat() * 2000 - 1000, -1000);
                    Projectile.NewProjectile(Pos, new Vector2(0, 3), ModContent.ProjectileType<HolyArrowHostile>(), projectile.damage, 0, target.whoAmI);

                }
            }


        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color27 = Color.White * projectile.Opacity;
            Texture2D Tex = Main.itemTexture[ItemID.DaedalusStormbow];
            SpriteEffects SP = SpriteEffects.None;
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, color27, projectile.rotation, new Vector2(0, Tex.Height / 2), projectile.scale, SP, 0);


            return false;
        }
    }
}