using ColonyLib;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Randomness : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
				int randomMask=Main.rand.Next((1<<10)-1)+1;
				if ((randomMask&(1))!=0) effects.forced_IsMale=Main.rand.NextBool();
				if ((randomMask&(1<<1))!=0) effects.forced_BodyType=Main.rand.Next(10);
				if ((randomMask&(1<<2))!=0) effects.forced_HairStyle=Main.rand.Next(HairLoader.Count);
				if ((randomMask&(1<<3))!=0) effects.forced_HairColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<4))!=0) effects.forced_SkinColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<5))!=0) effects.forced_EyeColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<6))!=0) effects.forced_ShirtColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<7))!=0) effects.forced_UnderShirtColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<8))!=0) effects.forced_PantsColor=ColonyUtils.RandomColor();
				if ((randomMask&(1<<9))!=0) effects.forced_ShoeColor=ColonyUtils.RandomColor();
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Randomness_Item : TransformationTrapBase_Item<TransformationTrap_Randomness>
		{
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient<TransformationTrap_Empty_Item>()
				.AddIngredient(ItemID.LogicGateLamp_Faulty)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}
