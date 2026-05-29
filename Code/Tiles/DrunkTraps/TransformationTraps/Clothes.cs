using ColonyLib;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Clothes : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<5)-1)+1;
				if ((randomMask&(1))!=0)
				{
					int selectedType=Main.rand.Next(10);
					if (PlayerVariantID.Sets.Male[selectedType]!=PlayerVariantID.Sets.Male[player.skinVariant])
					{
						selectedType=PlayerVariantID.Sets.AltGenderReference[selectedType];
					}
					effects.forced_BodyType=selectedType;
				}

				switch (Main.rand.Next(3))
				{
					case 0:
					{
						var color=ColonyUtils.RandomColor();
						if ((randomMask&(1<<1))!=0) effects.forced_ShirtColor=color;
						if ((randomMask&(1<<2))!=0) effects.forced_UnderShirtColor=color;
						if ((randomMask&(1<<3))!=0) effects.forced_PantsColor=color;
						if ((randomMask&(1<<4))!=0) effects.forced_ShoeColor=color;
					}break;
					case 1:
					{
						var color=ColonyUtils.RandomColor();
						if ((randomMask&(1<<1))!=0) effects.forced_ShirtColor=color.MultiplyRGB(GetNeutral(player.shirtColor));
						if ((randomMask&(1<<2))!=0) effects.forced_UnderShirtColor=color.MultiplyRGB(GetNeutral(player.underShirtColor));
						if ((randomMask&(1<<3))!=0) effects.forced_PantsColor=color.MultiplyRGB(GetNeutral(player.pantsColor));
						if ((randomMask&(1<<4))!=0) effects.forced_ShoeColor=color.MultiplyRGB(GetNeutral(player.shoeColor));
					}break;
					case 2:
					{
						if ((randomMask&(1<<1))!=0) effects.forced_ShirtColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<2))!=0) effects.forced_UnderShirtColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<3))!=0) effects.forced_PantsColor=ColonyUtils.RandomColor();
						if ((randomMask&(1<<4))!=0) effects.forced_ShoeColor=ColonyUtils.RandomColor();
					}break;
				}
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Clothes_Item : TransformationTrapBase_Item<TransformationTrap_Clothes>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.FamiliarShirt)
				.AddIngredient(ItemID.FamiliarPants)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
