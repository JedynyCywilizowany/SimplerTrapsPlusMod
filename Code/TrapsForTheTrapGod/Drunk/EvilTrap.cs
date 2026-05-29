using ColonyLib;
using ColonyLib.Debug;
using Microsoft.Xna.Framework;
using SimplerTraps.TrapsForTheTrapGod;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.TrapsForTheTrapGod;

public class EvilTrap_Gen : TrapForTheTrapGod
{
	public override bool? TryPlace(int plateX,int plateY,ref int trapX,ref int trapY,ref int plateStyle)
	{
		if (!SimplerTrapsPlusSystem.GenerateDrunkTraps()||SimplerTrapsPlusSystem.evilTraps>=SimplerTrapsPlusSystem.MaxEvilTraps()) return false;

		var plateTile=Main.tile[plateX,plateY];
		if (plateTile.LiquidAmount!=0&&plateTile.LiquidType==LiquidID.Lava) return false;
		
		Point direction=WorldGen.genRand.Next(4) switch
		{
			0=>new(0,-1),
			1=>new(0,1),
			2=>new(-1,0),
			3=>new(1,0),
			_=>default
		};

		while (!WorldGen.SolidOrSlopedTile(trapX,trapY))
		{
			trapX+=direction.X;
			trapY+=direction.Y;
			if (ColonyUtils.ChebyshevDistance(plateX,plateY,trapX,trapY)>128||trapY<Main.worldSurface||!WorldGen.InWorld(trapX,trapY,5)) return false;
		}

		WorldGen.KillTile(trapX,trapY);
		WorldGen.PlaceTile(trapX,trapY,ModContent.TileType<EvilTrap>());
		bool rightOrientation;
		if (trapX<plateX) rightOrientation=true;
		else if (trapX>plateX) rightOrientation=false;
		else rightOrientation=WorldGen.genRand.NextBool();
		Main.tile[trapX,trapY].TileFrameX=(direction.Y==0 ? (rightOrientation ? (short)18 : (short)0) :
		(direction.Y==1 ?
			(rightOrientation ? (short)54 : (short)36) :
			(rightOrientation ? (short)72 : (short)90)
		));

		SimplerTrapsPlusSystem.evilTraps++;
		ColonyDebug.AddWorldGenMarker(trapX,trapY,ModContent.GetInstance<EvilTrap_Item>().Texture);

		return true;
	}
}