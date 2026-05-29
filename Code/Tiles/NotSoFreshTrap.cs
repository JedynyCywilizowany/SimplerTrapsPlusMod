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
		public class NotSoFreshTrap : SimplerTrapBase6Way
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();

				DustType=DustID.FartInAJar;

				AddMapEntry(Color.Lerp(Color.Gray,Color.DarkGreen,0.5f),ModContent.GetInstance<NotSoFreshTrap_Item>().DisplayName);
			}

			public override int Cooldown=>300+NotSoFreshTrap_Projectile.Duration;
			public override int ProjectileType=>ModContent.ProjectileType<NotSoFreshTrap_Projectile>();
			public override int ProjectileDamage=>20;
		}
	}
	namespace Items
	{
		public class NotSoFreshTrap_Item : SimplerTrapBase_Item<NotSoFreshTrap>
		{
			public override void SetStaticDefaults()
			{
				ItemID.Sets.ShimmerTransformToItem[Type]=ItemID.DartTrap;
			}
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient(ItemID.DartTrap)
				.AddIngredient(ItemID.Wire,5)
				.AddIngredient(ItemID.GasTrap)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}