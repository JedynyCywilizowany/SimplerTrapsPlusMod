using ColonyLib;
using ColonyLib.Debug;
using Microsoft.Xna.Framework;
using SimplerTraps.TrapsForTheTrapGod;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.TrapsForTheTrapGod;

public class TransformationTraps_Gen : TrapForTheTrapGod
{
	public override bool? TryPlace(int plateX,int plateY,ref int trapX,ref int trapY,ref int plateStyle)
	{
		if (!SimplerTrapsPlusSystem.GenerateDrunkTraps()) return false;

		var plateTile=Main.tile[plateX,plateY];
		if (plateTile.LiquidAmount!=0&&plateTile.LiquidType==LiquidID.Lava) return false;

		int type=WorldGen.genRand.Next(7) switch
		{
			0=>ModContent.TileType<TransformationTrap_Clothes>(),
			1=>ModContent.TileType<TransformationTrap_Colors>(),
			2=>ModContent.TileType<TransformationTrap_Gender>(),
			3=>ModContent.TileType<TransformationTrap_Hair>(),
			4=>ModContent.TileType<TransformationTrap_Monochrome>(),
			5=>ModContent.TileType<TransformationTrap_Negative>(),
			6=>ModContent.TileType<TransformationTrap_Randomness>(),
			_=>default
		};
		
		int directionNum=WorldGen.genRand.Next(4);
		Point direction=directionNum switch
		{
			0=>new(1,0),
			1=>new(-1,0),
			2=>new(0,1),
			3=>new(0,-1),
			_=>default
		};

		if (direction.Y==0) trapY-=WorldGen.genRand.Next(3);

		while (!WorldGen.SolidOrSlopedTile(trapX,trapY))
		{
			trapX+=direction.X;
			trapY+=direction.Y;
			if (ColonyUtils.ChebyshevDistance(plateX,plateY,trapX,trapY)>128||trapY<Main.worldSurface||!WorldGen.InWorld(trapX,trapY,5)) return false;
		}

		WorldGen.KillTile(trapX,trapY);
		WorldGen.PlaceTile(trapX,trapY,type);
		Main.tile[trapX,trapY].TileFrameX=(short)(directionNum*18);
		
		ColonyDebug.AddWorldGenMarker(trapX,trapY,ModContent.GetModItem(TileLoader.GetItemDropFromTypeAndStyle(type)).Texture);

		return true;
	}
}