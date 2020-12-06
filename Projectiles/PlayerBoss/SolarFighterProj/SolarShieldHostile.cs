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
    public class SolarShieldHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Shield");
            DisplayName.AddTranslation(GameCulture.Chinese, "日耀盾");
        }
        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 42;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 380;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.netImportant = true;
        }
        public override void AI()
        {
            if (!Main.npc[(int)projectile.ai[0]].active || Main.npc[(int)projectile.ai[0]].type != ModContent.NPCType<SolarFighterBoss>()) 
            {
                projectile.Kill(); return; 
            }
            projectile.Center = Main.npc[(int)projectile.ai[0]].Center;
            projectile.ai[1]++;
            projectile.spriteDirection = Main.npc[(int)projectile.ai[0]].spriteDirection;
            if (projectile.ai[1] % 240 < 120) projectile.rotation = (projectile.ai[1] % 240) / 120 * MathHelper.Pi / 5 - MathHelper.Pi / 10;
            if (projectile.ai[1] % 240 >= 120) projectile.rotation = (240 - (projectile.ai[1] % 240)) / 120 * MathHelper.Pi / 5 - MathHelper.Pi / 10;
            if (projectile.ai[1] == 30 || projectile.ai[1] == 60 || projectile.ai[1] == 1)
            {
                for (int num15 = 0; num15 < 16; num15++)
                {
                    Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f)];
                    dust.noGravity = true;
                    dust.scale = 1.7f;
                    dust.fadeIn = 0.5f;
                    dust.velocity *= 5f;
                    //dust.shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.ArmorSetDye(), Main.LocalPlayer);
                }
            }


        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D Tex = Main.projectileTexture[projectile.type];
            if (projectile.ai[1] < 30) Tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/SolarFighterProj/SolarShield1");
            if (projectile.ai[1] < 60 && projectile.ai[1] >= 30) Tex = MABBossChallenge.Instance.GetTexture("Projectiles/PlayerBoss/SolarFighterProj/SolarShield2");
            SpriteEffects SP = (projectile.spriteDirection > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(Tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Tex.Size() * 0.5f, projectile.scale, SP, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Dazed, 60);
            if (Main.npc[(int)projectile.ai[0]].ai[0] > 1) target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            int protmp = Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileID.SolarCounter, projectile.damage, 0, Main.myPlayer);
            Main.projectile[protmp].hostile = true;
            Main.projectile[protmp].friendly = false;
            Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
            Main.projectile[protmp].Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            target.AddBuff(BuffID.Burning, 300);
            target.AddBuff(BuffID.Dazed, 60);
            if (Main.npc[(int)projectile.ai[0]].ai[0] > 1) target.AddBuff(ModContent.BuffType<SolarFlareBuff>(), (Main.rand.Next(3) + 3) * 60);
            int protmp = Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileID.SolarCounter, projectile.damage, 0, Main.myPlayer);
            Main.projectile[protmp].hostile = true;
            Main.projectile[protmp].friendly = false;
            Main.projectile[protmp].GetGlobalProjectile<PlayerBossProj>().SpecialProj = true;
            Main.projectile[protmp].Kill();
        }
    

        public override void Kill(int timeLeft)
        {
            for (int num15 = 0; num15 < 16; num15++)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f)];
                dust.noGravity = true;
                dust.scale = 1.7f;
                dust.fadeIn = 0.5f;
                dust.velocity *= 5f;
                //dust.shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.ArmorSetDye(), Main.LocalPlayer);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 10;
        }
    }
}