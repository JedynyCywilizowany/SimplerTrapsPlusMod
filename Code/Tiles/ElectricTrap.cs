using System;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Projectiles;
using SimplerTrapsPlus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class ElectricTrap : SimplerTrapBase
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();

				DustType=DustID.Electric;

				AddMapEntry(Color.Gray,ModContent.GetInstance<ElectricTrap_Item>().DisplayName);
			}

			public override int Cooldown=>300+ElectricTrap_Projectile.Duration;
			public override int ProjectileType=>ModContent.ProjectileType<ElectricTrap_Projectile>();
			public override int ProjectileDamage=>60;
		}
	}
	namespace Items
	{
		public class ElectricTrap_Item : SimplerTrapBase_Item<ElectricTrap>
		{
			public override string Texture=>ModContent.GetInstance<ElectricTrap>().Texture;
			public override void SetStaticDefaults()
			{
				ItemID.Sets.ShimmerTransformToItem[Type]=ItemID.DartTrap;
			}
			public override void AddRecipes()
			{
				foreach (var jellyfish in (Span<int>)[ItemID.BlueJellyfish,ItemID.GreenJellyfish,ItemID.PinkJellyfish])
				{
					CreateRecipe()
					.AddIngredient(ItemID.DartTrap)
					.AddIngredient(ItemID.Wire,5)
					.AddIngredient(jellyfish)
					.AddTile(TileID.Anvils)
					.Register();
				}
			}
		}
	}
}