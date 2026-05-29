using ColonyLib;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Monochrome : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<7)-1)+1;
				switch (Main.rand.Next(3))
				{
					case 0:
					{
						int v=Main.rand.Next(256);
						Color color=new(v,v,v);
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
						if ((randomMask&(1))!=0) effects.forced_HairColor=GetNeutral(player.hairColor);
						if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=GetNeutral(player.skinColor);
						if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=GetNeutral(player.eyeColor);
						if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=GetNeutral(player.shirtColor);
						if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=GetNeutral(player.underShirtColor);
						if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=GetNeutral(player.pantsColor);
						if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=GetNeutral(player.shoeColor);
					}break;
					case 2:
					{
						static Color GetGray(Color color)
						{
							int v=(color.R+color.G+color.B)/3;
							return new(v,v,v);
						}
						if ((randomMask&(1))!=0) effects.forced_HairColor=GetGray(player.hairColor);
						if ((randomMask&(1<<1))!=0) effects.forced_SkinColor=GetGray(player.skinColor);
						if ((randomMask&(1<<2))!=0) effects.forced_EyeColor=GetGray(player.eyeColor);
						if ((randomMask&(1<<3))!=0) effects.forced_ShirtColor=GetGray(player.shirtColor);
						if ((randomMask&(1<<4))!=0) effects.forced_UnderShirtColor=GetGray(player.underShirtColor);
						if ((randomMask&(1<<5))!=0) effects.forced_PantsColor=GetGray(player.pantsColor);
						if ((randomMask&(1<<6))!=0) effects.forced_ShoeColor=GetGray(player.shoeColor);
					}break;
				}
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Monochrome_Item : TransformationTrapBase_Item<TransformationTrap_Monochrome>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.BlackAndWhiteDye)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
