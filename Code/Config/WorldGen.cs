using Terraria.ModLoader.Config;

namespace SimplerTrapsPlus.Config;

public class SimplerTrapsPlusConfig : ModConfig
{
	public override ConfigScope Mode=>ConfigScope.ClientSide;

	public enum DrunkTrapsCondition
	{
		DrunkOnly,
		Always,
		Never
	}
	public DrunkTrapsCondition ShouldGenerateDrunkTraps{get;set;}
}