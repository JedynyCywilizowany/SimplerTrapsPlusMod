using ColonyLib;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Colors : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<7)-1)+1;
				switch (Main.rand.Next(3))
				{
					case 0:
					{
						var color=ColonyUtils.RandomColor();
						if ((randomMask&(1))!=0) effects.forced_HairColor=color;
						if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=color;
						if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=color;
						if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=color;
						if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=color;
						if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=color;
						if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=color;
					}break;
					case 1:
					{
						var color=ColonyUtils.RandomColor();
						if ((randomMask&(1))!=0) effects.forced_HairColor=color.MultiplyRGB(GetNeutral(player.hairColor));
						if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=color.MultiplyRGB(GetNeutral(player.skinColor));
						if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=color.MultiplyRGB(GetNeutral(player.eyeColor));
						if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=color.MultiplyRGB(GetNeutral(player.shirtColor));
						if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=color.MultiplyRGB(GetNeutral(player.underShirtColor));
						if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=color.MultiplyRGB(GetNeutral(player.pantsColor));
						if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=color.MultiplyRGB(GetNeutral(player.shoeColor));
					}break;
					case 2:
					{
						if ((randomMask&(1))!=0) effects.forced_HairColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=ColonyUtils.RandomColor();
					}break;
				}
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Colors_Item : TransformationTrapBase_Item<TransformationTrap_Colors>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.RainbowBrick,5)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
