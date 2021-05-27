using MABBossChallenge.Buffs;
using MABBossChallenge.Buffs.EGO;
using MABBossChallenge.Items.EGO;
using MABBossChallenge.Projectiles.EGO;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MABBossChallenge
{
    public class EGOPlayer : ModPlayer
    {
        public Item[] SaveItem = new Item[2];
        public Item[] SavedArmors;
        public bool ItemSaved = false;
        public int CurrentEGO = 0;
        public int SolarRamCD = 0;
        
        public override void PostUpdateMiscEffects()
        {
            //Main.NewText(player.head + " " + player.body + " " + player.legs + " " + player.wings);
            if (CurrentEGO > 0) 
            {
                if (CurrentEGO == EGOUtils.SolarEGO)
                {
                    SolarShield();
                    player.endurance = 0.45f;
                    player.statDefense = 150;
                    player.meleeSpeed = 0.6f;
                    player.meleeDamage += 1.5f;
                    player.meleeCrit += 50;
                    player.rangedDamage = 0;
                    player.magicDamage = 0;
                    player.minionDamage = 0;
                    player.thrownDamage = 0;
                    player.magmaStone = true;
                    player.resistCold = true;
                    player.lavaImmune = true;
                    player.longInvince = true;
                    player.fireWalk = true;
                    player.waterWalk2 = true;
                    player.buffImmune[ModContent.BuffType<SolarFlareBuff>()] = true;
                    player.buffImmune[BuffID.OnFire] = true;
                    player.buffImmune[BuffID.CursedInferno] = true;
                    player.buffImmune[BuffID.Frostburn] = true;
                    player.buffImmune[BuffID.ShadowFlame] = true;
                    player.buffImmune[BuffID.Burning] = true;
                    if (player.dashDelay < 0)
                    {
                        if (SolarRamCD > 1)
                        {
                            SolarRamCD--;
                        }
                        if (SolarRamCD == 0)
                        {
                            SolarRamCD = -1;
                            for (float i = -MathHelper.Pi / 2; i <= MathHelper.Pi / 2; i += MathHelper.Pi / 8)
                            {
                                int protmp = Projectile.NewProjectile(player.Center, (player.velocity.ToRotation() + i).ToRotationVector2() * 20, ProjectileID.DD2FlameBurstTowerT3Shot, (int)(500 * player.meleeDamage), 0.5f, player.whoAmI);
                                SunFuryEGO.FireballSetDefaults(protmp);
                            }
                        }
                        if (SolarRamCD == -1)
                        {
                            if (++player.miscTimer % 4 == 2)
                            {
                                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<RamFireballEGO>(), (int)(350 * player.meleeDamage), 0.5f, player.whoAmI);
                            }
                        }
                    }
                    else
                    {
                        if (SolarRamCD == -1)
                        {
                            SolarRamCD = 300;
                        }
                        if (SolarRamCD > 0)
                        {
                            SolarRamCD--;
                        }
                    }
                }

                if (CurrentEGO == EGOUtils.VortexEGO)
                {
                    player.endurance = 0.15f;
                    player.statDefense = 120;
                    player.rangedDamage += 1.5f;
                    player.rangedCrit = 50;
                    player.ammoBox = true;
                    player.ammoCost75 = true;
                    player.ammoCost80 = true;
                    player.ammoPotion = true;
                    player.archery = true;
                    player.rocketDamage += 0.3f;
                    player.arrowDamage += 0.3f;
                    player.bulletDamage += 0.3f;
                    player.meleeDamage = 0;
                    player.magicDamage = 0;
                    player.minionDamage = 0;
                    player.thrownDamage = 0;
                    player.gravControl = true;
                    player.waterWalk = true;
                    player.waterWalk2 = true;
                    player.fireWalk = true;
                    player.buffImmune[BuffID.Dazed] = true;
                    player.buffImmune[BuffID.Electrified] = true;
                    player.buffImmune[164] = true;              //免疫失重
                    
                }

                if (!ItemSaved)                            //保存
                {
                    ItemSaved = true;
                    SaveItem[0] = player.inventory[0].DeepClone();
                    SaveItem[1] = player.inventory[1].DeepClone();
                    switch (CurrentEGO)
                    {
                        case EGOUtils.SolarEGO:
                            player.inventory[0].SetDefaults(ModContent.ItemType<DayBreakEGO>());
                            player.inventory[1].SetDefaults(ModContent.ItemType<SolarEruptionEGO>());
                            break;
                        case EGOUtils.VortexEGO:
                            player.inventory[0].SetDefaults(ItemID.VortexBeater);
                            player.inventory[1].SetDefaults(ItemID.Phantasm);
                            break;
                        default:
                            player.inventory[0].SetDefaults(ModContent.ItemType<Nope>());
                            player.inventory[1].SetDefaults(ModContent.ItemType<Nope>());
                            break;
                    }
                    SavedArmors = new Item[player.extraAccessorySlots + 8];
                    for(int i = 0; i < player.extraAccessorySlots + 8; i++)
                    {
                        SavedArmors[i] = player.armor[i].DeepClone();
                        player.armor[i].SetDefaults(ModContent.ItemType<Nope>());
                    }
                }
                else
                {
                    for (int i = 0; i < player.extraAccessorySlots + 8; i++)
                    {
                        player.armor[i].SetDefaults(ModContent.ItemType<Nope>());
                    }
                }
            }
            else
            {
                if (ItemSaved)
                {
                    ItemSaved = false;
                    player.inventory[0] = SaveItem[0].DeepClone();
                    player.inventory[1] = SaveItem[1].DeepClone();
                    SaveItem[0] = new Item();
                    SaveItem[1] = new Item();
                    for (int i = 0; i < player.extraAccessorySlots + 8; i++)
                    {
                        player.armor[i] = SavedArmors[i].DeepClone();
                        SavedArmors[i] = new Item();
                    }
                    SavedArmors = null;
                    player.AddBuff(ModContent.BuffType<EGOCD>(), 60 * 60 * 5);
                }
            }
        }
        public override void PreSavePlayer()
        {
            if (CurrentEGO > 0) 
            {
                player.CleaningEGOBuff();
                ItemSaved = false;
                player.inventory[0] = SaveItem[0].DeepClone();
                player.inventory[1] = SaveItem[1].DeepClone();
                SaveItem[0] = new Item();
                SaveItem[1] = new Item();
                for (int i = 0; i < player.extraAccessorySlots + 8; i++)
                {
                    player.armor[i] = SavedArmors[i].DeepClone();
                    SavedArmors[i] = new Item();
                }
                SavedArmors = null;
            }
        }

        public override void PostUpdateEquips()
        {
            if (CurrentEGO == EGOUtils.SolarEGO)
            {
                List<int> SolarAcc = new List<int> { ItemID.WingsSolar, ItemID.FrogLeg, ItemID.FrostsparkBoots, ItemID.AnkhShield };
                FakeEquipAcc(SolarAcc);
                player.wingTime = player.wingTimeMax;
            }
            if (CurrentEGO == EGOUtils.VortexEGO)
            {
                List<int> VortexAcc = new List<int> { ItemID.WingsVortex, ItemID.FrogLeg, ItemID.FrostsparkBoots, ItemID.MasterNinjaGear };
                FakeEquipAcc(VortexAcc);
                player.wingTime = player.wingTimeMax;
            }
        }

        public void FakeEquipAcc(int type)
        {
            Item item = new Item();
            item.SetDefaults(type);
            bool flag = false;
            player.VanillaUpdateAccessory(player.whoAmI, item, true, ref flag, ref flag, ref flag);
        }

        public void FakeEquipAcc(List<int> types)
        {
            if (types.Count > 0)
            {
                foreach(int i in types)
                {
                    FakeEquipAcc(i);
                }
            }
        }

        public override void FrameEffects()
        {
            //solar
            if (CurrentEGO == EGOUtils.SolarEGO)
            {
                player.head = 171;
                player.body = 177;
                player.legs = 112;
                player.wings = 29;
            }
            if (CurrentEGO == EGOUtils.VortexEGO)
            {
                player.head = 169;
                player.body = 175;
                player.legs = 110;
                player.wings = 30;
            }
        }
        public override void ResetEffects()
        {
            CurrentEGO = 0;
        }
        public override void UpdateDead()
        {
            CurrentEGO = 0;
        }
       
        public void SolarShield()
        {
            player.AddBuff(BuffID.SolarShield3, 5, false);
            player.setSolar = true;
            player.solarCounter++;
            int solarCD = 240;
            if (player.solarCounter >= solarCD)
            {
                if (player.solarShields > 0 && player.solarShields < 3)
                {
                    for (int i = 0; i < 22; i++)
                    {
                        if (player.buffType[i] >= BuffID.SolarShield1 && player.buffType[i] <= BuffID.SolarShield2)
                        {
                            player.DelBuff(i);
                        }
                    }
                }
                if (player.solarShields < 3)
                {
                    player.AddBuff(BuffID.SolarShield1 + player.solarShields, 5, false);
                    for (int i = 0; i < 16; i++)
                    {
                        Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, 6, 0f, 0f, 100)];
                        dust.noGravity = true;
                        dust.scale = 1.7f;
                        dust.fadeIn = 0.5f;
                        dust.velocity *= 5f;
                    }
                    player.solarCounter = 0;
                }
                else
                {
                    player.solarCounter = solarCD;
                }
            }
            for (int i = player.solarShields; i < 3; i++)
            {
                player.solarShieldPos[i] = Vector2.Zero;
            }
            for (int i = 0; i < player.solarShields; i++)
            {
                player.solarShieldPos[i] += player.solarShieldVel[i];
                Vector2 value = (player.miscCounter / 100f * MathHelper.TwoPi + i * (MathHelper.TwoPi / player.solarShields)).ToRotationVector2() * 6f;
                value.X = player.direction * 20;
                player.solarShieldVel[i] = (value - player.solarShieldPos[i]) * 0.2f;
            }
            if (player.dashDelay >= 0)
            {
                player.solarDashing = false;
                player.solarDashConsumedFlare = false;
            }
            bool flag = player.solarDashing && player.dashDelay < 0;
            if (player.solarShields > 0 || flag)
            {
                player.dash = 3;
            }
        }












    }

    public class EGOItem : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            return base.IsArmorSet(head, body, legs);
        }
        public override bool CanEquipAccessory(Item item, Player player, int slot)
        {
            if (player.GetModPlayer<EGOPlayer>().CurrentEGO > 0)
            {
                return false;
            }
            return true;
        }

    }
}