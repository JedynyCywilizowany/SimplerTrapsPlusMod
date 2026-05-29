using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Gender : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				effects.forced_IsMale=!PlayerVariantID.Sets.Male[player.skinVariant];
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Gender_Item : TransformationTrapBase_Item<TransformationTrap_Gender>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.GenderChangePotion)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
