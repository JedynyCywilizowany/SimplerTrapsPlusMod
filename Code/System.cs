using SimplerTrapsPlus.Config;
using Terraria;
using Terraria.ModLoader;

namespace SimplerTrapsPlus;

public class SimplerTrapsPlusSystem : ModSystem
{
	internal static int evilTraps;
	public override void PreWorldGen()
	{
		evilTraps=0;
	}
	internal static int MaxEvilTraps()
	{
		int maxEvilTraps=WorldGen.GetWorldSize()+1;
		if (WorldGen.noTrapsWorldGen) maxEvilTraps*=5;
		return maxEvilTraps;
	}
	public static bool GenerateDrunkTraps()
	{
		return ModContent.GetInstance<SimplerTrapsPlusConfig>().ShouldGenerateDrunkTraps switch
		{
			SimplerTrapsPlusConfig.DrunkTrapsCondition.Always=>true,	
			SimplerTrapsPlusConfig.DrunkTrapsCondition.Never=>false,
			_=>WorldGen.drunkWorldGen
		};
	}
}