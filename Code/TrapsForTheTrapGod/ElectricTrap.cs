using System;
using System.Collections.Generic;
using System.Linq;
using ColonyLib.Debug;
using Microsoft.Xna.Framework;
using SimplerTraps.TrapsForTheTrapGod;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Projectiles;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.TrapsForTheTrapGod;

public class ElectricTrap_Gen : TrapForTheTrapGod
{
	public override bool? TryPlace(int plateX,int plateY,ref int trapX,ref int trapY,ref int plateStyle)
	{
		var plateTile=Main.tile[plateX,plateY];
		if (plateTile.LiquidAmount==0||(plateTile.LiquidType==LiquidID.Lava||plateTile.LiquidType==LiquidID.Shimmer)) return false;

		Dictionary<Point,int> water=new();
		void ScanWaterBody(int cx,int cy,int rangeLeft)
		{
			if (!WorldGen.InWorld(cx,cy,3)) return;

			var tile=Main.tile[cx,cy];
			
			if (tile.LiquidAmount!=0&&!WorldGen.SolidOrSlopedTile(tile))
			{
				Point pos=new(cx,cy);
				if (water.GetValueOrDefault(pos)<rangeLeft)
				{
					water[pos]=rangeLeft;
					
					rangeLeft--;
					if (rangeLeft>0)
					{
						ScanWaterBody(cx-1,cy,rangeLeft);
						ScanWaterBody(cx,cy-1,rangeLeft);
						ScanWaterBody(cx+1,cy,rangeLeft);
						ScanWaterBody(cx,cy+1,rangeLeft);
					}
				}
			}
		}

		ScanWaterBody(plateX,plateY,(int)ElectricTrap_Projectile.Range_Water-5);
		if (water.Count<30) return false;

		HashSet<Point> borders=new();
		Span<Point> offsets=[new(-1,0),new(1,0),new(0,-1),new(0,1)];
		foreach (var point in water.Keys)
		{
			foreach (var offset in offsets)
			{
				var finalPos=point+offset;
				if (WorldGen.SolidOrSlopedTile(finalPos.X,finalPos.Y)) borders.Add(finalPos);
			}
		}
		WorldGen.genRand.Next(borders.ToArray()).Deconstruct(out trapX,out trapY);
		WorldGen.KillTile(trapX,trapY);
		WorldGen.PlaceTile(trapX,trapY,ModContent.TileType<ElectricTrap>());

		ColonyDebug.AddWorldGenMarker(new(trapX,trapY),ModContent.GetInstance<ElectricTrap_Item>().Texture);

		return true;
	}
}