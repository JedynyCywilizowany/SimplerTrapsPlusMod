using ColonyLib;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Hair : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<2)-1)+1;
				if ((randomMask&(1))!=0) effects.forced_HairStyle=Main.rand.Next(HairLoader.Count);
				if ((randomMask&(1<<1))!=0) effects.forced_HairColor=ColonyUtils.RandomColor();
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Hair_Item : TransformationTrapBase_Item<TransformationTrap_Hair>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.FamiliarWig)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
