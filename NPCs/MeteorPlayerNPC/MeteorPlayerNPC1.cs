using System;
using MABBossChallenge.Items;
using MABBossChallenge.NPCs.MeteorPlayerNPC;
using MABBossChallenge.NPCs.MiniPlayerBoss;
using MABBossChallenge.Projectiles.MeteorPlayerNPC;
using MABBossChallenge.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace MABBossChallenge.NPCs
{
	
	[AutoloadHead]
	public class MeteorPlayerNPC1 : ModNPC
	{
		public override string Texture => "MABBossChallenge/NPCs/MeteorPlayerNPC/MeteorPlayerNPC1";

		//public override string[] AltTextures => new[] { "ExampleMod/NPCs/ExamplePerson_Alt_1" };
		public bool Challenge = false;
		public override bool Autoload(ref string name)
		{
			name = "陨石守卫";
			return mod.Properties.Autoload;
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[npc.type] = 25;
			NPCID.Sets.ExtraFramesCount[npc.type] = 5;
			NPCID.Sets.AttackFrameCount[npc.type] = 4;
			NPCID.Sets.DangerDetectRange[npc.type] = 1000;
			NPCID.Sets.AttackType[npc.type] = NPCID.Sets.AttackType[NPCID.Mechanic];
			NPCID.Sets.AttackTime[npc.type] = 90;
			NPCID.Sets.AttackAverageChance[npc.type] = 100;

			TranslationUtils.AddTranslation("MeteorGuardianChat1", "Now that you've defeat me, my job is over...I believe you will not waste the meteorite.", "既然你打败了我，我的工作也就结束了...我相信你不会让陨石白白浪费。");
			TranslationUtils.AddTranslation("MeteorGuardianChat2", "Aside from some carefully processed meteor products, I can also serve as a temporary mercenary.", "除了精心加工过的陨石制品外，我还可以作为临时的佣兵。");
			TranslationUtils.AddTranslation("MeteorGuardianChat3", "Despite the endless threats, I have to admit how wonderful and spectacular this is.", "尽管蕴藏着无尽的威胁，但我不得不承认这是多么奇妙而壮观的场景。");
			TranslationUtils.AddTranslation("MeteorGuardianChat4", "Those monsters are like just popping out of horror movies.", "那些怪物，就像是从恐怖电影中蹦出来的。");
			TranslationUtils.AddTranslation("MeteorGuardianChat5", "Affected by the strange moon phase, those monsters became restless, ready to fight!", "受到奇异月相的影响，那些怪物变得躁动起来，准备迎战！");
			TranslationUtils.AddTranslation("MeteorGuardianChat6", "What a wonderful sunshine, photons that have traveled hundreds of millions of kilometers came before my eyes.", "多么美妙的阳光啊，旅行了数亿公里的光子来到了我的眼前。");
			TranslationUtils.AddTranslation("MeteorGuardianChat7", "As night falls, I can better observe the meteors in the sky.", "夜幕的降临，能让我更好的观测天空中的流星。");
			TranslationUtils.AddTranslation("MeteorGuardianChat8", "Don't count on me, I won't help you beat the boss!", "别指望我，我不会帮你打败boss的！");
			TranslationUtils.AddTranslation("MeteorGuardianChat9", "This is not the time for small talk!Maybe we can talk about it after this is over", "现在不是闲聊的时候！或许等这结束后我们能好好谈谈");

			TranslationUtils.AddTranslation("MeteorGuardianOption", "Challenge again", "再次挑战");
			TranslationUtils.AddTranslation("MeteorGuardianChallengeAgain", "You want to challenge me again? Then let's begin!", "你想再次挑战我？那就来吧！");
		}
		public override void UpdateLifeRegen(ref int damage)
		{
			npc.lifeRegen += 10;
		}
		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.width = 26;
			npc.height = 38;
			npc.aiStyle = 7;
			npc.damage = 10;
			npc.defense = 25;
			npc.lifeMax = 4000;
			if (Main.hardMode && MABWorld.DownedMeteorPlayerEX)
			{
				npc.lifeMax = 10000;
				npc.defense = 50;
				npc.damage = 20;
				if (NPC.downedMoonlord)
				{
					npc.lifeMax = 100000;
					npc.defense = 100;
					npc.damage = 30;
				}
			}
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath4;
			npc.knockBackResist = 0.5f;
			animationType = NPCID.Merchant;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			int num = npc.life > 0 ? 1 : 5;
			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 6);
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			
			if (Main.LocalPlayer.active) 
			{
				if (MABWorld.DownedMeteorPlayer && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerDefender>()) &&
					!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC1>()) &&
					!NPC.AnyNPCs(ModContent.NPCType<MeteorPlayerNPC2>()))
				{
					return true;
				}
			}

			
			return false;
		}

		public override bool PreAI()
		{
			//Main.NewText(NPCID.Sets.AttackAverageChance[npc.type]);
			if(!MABBossChallenge.mabconfig.NPCAttackBoss && Utils.NPCUtils.AnyBosses())
			{
				NPCID.Sets.AttackAverageChance[npc.type] = 0;
				npc.velocity.X *= 0.8f;
				npc.dontTakeDamageFromHostiles = true;
				npc.dontTakeDamage = true;
				npc.reflectingProjectiles = true;
			}
			else
			{
				npc.dontTakeDamage = false;
				npc.dontTakeDamageFromHostiles = false;
				NPCID.Sets.AttackAverageChance[npc.type] = 100;
				npc.reflectingProjectiles = false;
			}


			if (HomeOnTarget() != -1)
			{
				Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MeteorTransform>(), 0, 0);
			}
			

			return true;
		}

		public override string TownNPCName()
		{
			return TranslationUtils.GetTranslation("MeteorGuardianNPCName");
		}


		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();
			chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat1"));
			chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat2"));
			chat.Add("emmmmmmmmmmmm...");
			if (Main.eclipse)
			{
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat3"));
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat4"));
			}
			if (Main.bloodMoon)
			{
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat5"));
			}
			if (Main.dayTime && !Main.eclipse)
			{
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat6"));
			}
			if(!Main.dayTime && !Main.bloodMoon)
			{
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat7"));
				//chat.Add("看着天上的星星，让我有了“试一下你”的冲动");
			}
			if (Utils.NPCUtils.AnyBosses() && !MABBossChallenge.mabconfig.NPCAttackBoss)
			{
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat8"), 99999);
				chat.Add(TranslationUtils.GetTranslation("MeteorGuardianChat9"), 99999);
			}

			return chat;
		}

		

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = TranslationUtils.GetTranslation("MeteorGuardianOption");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				shop = true;
			}
			else
			{
				Main.npcChatText = TranslationUtils.GetTranslation("MeteorGuardianChallengeAgain");
				npc.Transform(ModContent.NPCType<MeteorPlayerBoss>());
				shop = false;
			}
			
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(ItemID.FallenStar);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.Meteorite);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<MeteorPotion>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.MeteorShot);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.MeteorHamaxe);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.SpaceGun);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.StarCannon);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.BluePhaseblade);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.GreenPhaseblade);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.PurplePhaseblade);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.RedPhaseblade);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.WhitePhaseblade);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ItemID.YellowPhaseblade);
			nextSlot++;

			if (Main.hardMode)
			{
				shop.item[nextSlot].SetDefaults(ItemID.LaserRifle);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.MeteorStaff);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.BluePhasesaber);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.GreenPhasesaber);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.PurplePhasesaber);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.RedPhasesaber);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.WhitePhasesaber);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.YellowPhasesaber);
				nextSlot++;
				if (NPC.downedMartians)
				{
					shop.item[nextSlot].SetDefaults(ItemID.LaserMachinegun);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (!MABBossChallenge.mabconfig.NPCAttackBoss && Utils.NPCUtils.AnyBosses())
			{
				Texture2D tex = MABBossChallenge.Instance.GetTexture("Images/BubbleShield");
				spriteBatch.Draw(tex, npc.Center - Main.screenPosition, null, Color.White, 0, tex.Size() * 0.5f, npc.scale * 1.2f, SpriteEffects.None, 0);
			}
			return true;
		}

		public override void NPCLoot()
		{
			
		}

		// Make this Town NPC teleport to the King and/or Queen statue when triggered.
		public override bool CanGoToStatue(bool toKingStatue)
		{
			return true;
		}

		// Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = mod.GetPacket();
				packet.Write((byte)npc.whoAmI);
				packet.Send();
			}
			else
			{
				StatueTeleport();
			}
		}

		public void StatueTeleport()
		{
			for (int i = 0; i < 30; i++)
			{
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y))
				{
					position.X = Math.Sign(position.X) * 20;
				}
				else
				{
					position.Y = Math.Sign(position.Y) * 20;
				}
				Dust.NewDustPerfect(npc.Center + position, 6, Vector2.Zero).noGravity = true;
			}
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 1;
			knockback = 1;
		}
		
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 5;
			
		}


		public int HomeOnTarget()
		{
			if (Utils.NPCUtils.AnyBosses() && !MABBossChallenge.mabconfig.NPCAttackBoss)
			{
				return -1;
			}
			float homingMaximumRangeInPixels = NPCID.Sets.DangerDetectRange[npc.type];
			int selectedTarget = -1;
			foreach (NPC target in Main.npc)
			{

				if (MABBossChallenge.mabconfig.NPCAttackBoss || target.damage > 0)
				{
					if (target.active && !target.friendly && !target.immortal && !target.dontTakeDamage && (target.type != NPCID.SkeletonMerchant || !NPCID.Sets.Skeletons.Contains(target.netID)))
					{
						float distance = Vector2.Distance(npc.Center, target.Center);
						if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || distance < Vector2.Distance(Main.npc[selectedTarget].Center, npc.Center)))
						{
							selectedTarget = target.whoAmI;
						}

					}
				}

			}
			return selectedTarget;
		}


		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<MeteorTransform>();
			attackDelay = 1;
		}
		
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 1f;
			randomOffset = 1f;
		}
	}
}
