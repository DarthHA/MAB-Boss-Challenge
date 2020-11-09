using MABBossChallenge.Buffs;
using MABBossChallenge.NPCs.PlayerBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MABBossChallenge.Projectiles.PlayerBoss.SolarFighterProj
{
    public class StarWrathHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Wrath");
            DisplayName.AddTranslation(GameCulture.Chinese, "狂星之怒");
        }
        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 54;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.NPCType<SolarFighterBoss>()) projectile.Kill();
            Player target = Main.player[owner.target];
            projectile.spriteDirection = owner.spriteDirection;
            projectile.direction = owner.direction;
            projectile.Center = owner.Center + new Vector2(projectile.spriteDirection * 17, 0);
            projectile.rotation += 0.1f;             //2f
            //projectile.rotation = 1f;

            if (projectile.timeLeft % 20 == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    float r = Main.rand.NextFloat() * MathHelper.Pi;
                    //int protmp = Projectile.NewProjectile(new Vector2(owner.Center.X, Main.screenPosition.Y - 50), new Vector2(Main.rand.Next(5) - 2, 10), ProjectileID.StarWrath, projectile.damage, 0,target.whoAmI);
                    int protmp = Projectile.NewProjectile(target.Center + target.velocity / 3 + r.ToRotationVector2() * -1000, r.ToRotationVector2() * 13, ProjectileID.StarWrath, projectile.damage, 0, owner.target);
                    Main.projectile[protmp].hostile = true;
                    Main.projectile[protmp].friendly = false;
                    Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
                }
            }



        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (SP == SpriteEffects.None)
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition + new Vector2(-10, 7), null, Color.White, projectile.rotation - MathHelper.Pi / 2, new Vector2(0, 54), 1, SP, 0);

            }
            if (SP == SpriteEffects.FlipHorizontally)
            {
                spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition + new Vector2(10, 7), null, Color.White, MathHelper.Pi / 2 - projectile.rotation, new Vector2(46, 54), 1, SP, 0);

            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}