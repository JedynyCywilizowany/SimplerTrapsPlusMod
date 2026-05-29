using SimplerTrapsPlus.Tiles;
using Terraria;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class TransformationTrap_Empty : TransformationTrapBase
		{
			public override void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects)
			{
			}
		}
	}
	namespace Items
	{
		public class TransformationTrap_Empty_Item : TransformationTrapBase_Item<TransformationTrap_Empty>
		{
			
		}
	}
}
