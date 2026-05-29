using ColonyLib;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Negative : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<7)-1)+1;
				if ((randomMask&(1))!=0) effects.forced_HairColor=ColonyUtils.InvertedColor(player.hairColor);
				if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=ColonyUtils.InvertedColor(player.skinColor);
				if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=ColonyUtils.InvertedColor(player.eyeColor);
				if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=ColonyUtils.InvertedColor(player.shirtColor);
				if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=ColonyUtils.InvertedColor(player.underShirtColor);
				if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=ColonyUtils.InvertedColor(player.pantsColor);
				if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=ColonyUtils.InvertedColor(player.shoeColor);
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Negative_Item : TransformationTrapBase_Item<TransformationTrap_Negative>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.NegativePaint,50)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
