using ColonyLib;
using Terraria.ModLoader;

namespace SimplerTrapsPlus;

public partial class SimplerTrapsPlus : Mod
{
	public override string Name=>nameof(SimplerTrapsPlus);
	public static SimplerTrapsPlus Instance=>ModContent.GetInstance<SimplerTrapsPlus>();

	public override void Load()
	{
		
	}
	public override void Unload()
	{
		this.AutoUnload();
	}
}