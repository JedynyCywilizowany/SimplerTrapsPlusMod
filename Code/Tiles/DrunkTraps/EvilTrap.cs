using ColonyLib.TileEntities;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Projectiles;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class EvilTrap : SimplerTrapBase6Way
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();

				DustType=DustID.Blood;
				MinPick=200;
				MineResist=100f;

				AddMapEntry(Color.Black,Language.GetText("LegacyInterface.38"));
			}
			public override void HitWire(int x,int y)
			{
				if (MechCooldown.Apply(x,y,Cooldown))
				{
					void SpawnFor(int target)
					{
						Projectile.NewProjectileDirect(new EntitySource_Wiring(x,y),new Vector2(x*16,y*16),DirectionFromFrame(Main.tile[x,y].TileFrameX/18).ToVector2()*16f,ModContent.ProjectileType<EvilDart>(),999,4.5f,ai0:target);
					}
					foreach (var player in Main.ActivePlayers)
					{
						if (!player.ghost)
						{
							SpawnFor(player.whoAmI);
						}
					}
					foreach (var npc in Main.ActiveNPCs)
					{
						if (npc.isLikeATownNPC)
						{
							SpawnFor(npc.whoAmI+300);
						}
					}
				}
			}
			public override bool CanExplode(int x,int y)
			{
				return false;
			}
			public override void NumDust(int x,int y,bool fail,ref int num)
			{
				num*=10;
			}

			public override int Cooldown=>3600;
			public override int ProjectileType=>-1;
			public override int ProjectileDamage=>-1;
		}
	}
	namespace Items
	{
		public class EvilTrap_Item : SimplerTrapBase_Item<EvilTrap>
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();
				ItemID.Sets.ShimmerTransformToItem[Type]=Type;
				Item.ResearchUnlockCount=100;
			}
			public override void SetDefaults()
			{
				base.SetDefaults();
				Item.value=-1;
			}
			public override void OnResearched(bool fullyResearched)
			{
				if (Main.LocalPlayerCreativeTracker.ItemSacrifices.TryGetSacrificeNumbers(Type,out var amountWeHave,out _))
				{
					if (amountWeHave>99) Main.LocalPlayerCreativeTracker.ItemSacrifices.SetSacrificeCountDirectly(FullName,99);
				}
			}
		}
	}
}