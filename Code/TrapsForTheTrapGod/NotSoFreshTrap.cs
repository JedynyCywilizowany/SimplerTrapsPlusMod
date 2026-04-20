using System;
using System.Collections.Generic;
using System.Linq;
using ColonyLib;
using ColonyLib.Debug;
using Microsoft.Xna.Framework;
using SimplerTraps.TrapsForTheTrapGod;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Projectiles;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.TrapsForTheTrapGod;

public class NotSoFreshTrap_Gen : TrapForTheTrapGod
{
	public override bool? TryPlace(int plateX,int plateY,ref int trapX,ref int trapY,ref int plateStyle)
	{
		var plateTile=Main.tile[plateX,plateY];
		if (plateTile.LiquidAmount!=0) return false;

		Dictionary<Point,int> air=new();//air now
		void ScanRoom(int cx,int cy,int rangeLeft)
		{
			if (!WorldGen.InWorld(cx,cy,3)) return;

			var tile=Main.tile[cx,cy];
			
			if (tile.LiquidAmount==0&&!WorldGen.SolidOrSlopedTile(tile))
			{
				Point pos=new(cx,cy);
				if (air.GetValueOrDefault(pos)<rangeLeft)
				{
					air[pos]=rangeLeft;
					
					rangeLeft--;
					if (rangeLeft>0)
					{
						ScanRoom(cx-1,cy,rangeLeft);
						ScanRoom(cx,cy-1,rangeLeft);
						ScanRoom(cx+1,cy,rangeLeft);
						ScanRoom(cx,cy+1,rangeLeft);
					}
				}
			}
		}

		int range=(int)MathF.Sqrt(NotSoFreshTrap_Projectile.SprayDuration*NotSoFreshTrap_Projectile.SprayPower);
		ScanRoom(plateX,plateY,range*2);
		if (air.Count<30) return false;

		HashSet<(Point pos,Point offset)> borders=new();
		Span<Point> offsets=[new(-1,0),new(1,0),new(0,-1),new(0,1)];
		foreach (var point in air.Keys)
		{
			if (point.ManhattanDistance(new(plateX,plateY))<range)
			{
				foreach (var offset in offsets)
				{
					var finalPos=point+offset;
					if (WorldGen.SolidOrSlopedTile(finalPos.X,finalPos.Y)) borders.Add((finalPos,offset));
				}
			}
		}
		var borderList=borders.ToList();
		var count=air.Count/(NotSoFreshTrap_Projectile.SprayDuration*NotSoFreshTrap_Projectile.SprayPower);
		if (count==0) return false;
		for (int i=(int)Math.Ceiling(WorldGen.genRand.NextFloat(count*0.75f,count*2f));i>0;i--)
		{
			if (borderList.Count==0) break;
			var posIndex=WorldGen.genRand.Next(borderList.Count);
			var (pos,offset)=borderList[posIndex];
			borderList.RemoveAt(posIndex);
			pos.Deconstruct(out trapX,out trapY);

			WorldGen.KillTile(trapX,trapY);
			WorldGen.PlaceTile(trapX,trapY,ModContent.TileType<NotSoFreshTrap>());
			ref var tileFrameX=ref Main.tile[trapX,trapY].TileFrameX;

			var direction=Vector2.Zero.DirectionTo(-((offset).ToVector2()));
			if (Math.Abs(direction.Y)>Math.Abs(direction.X))
			{
				tileFrameX=(direction.Y>0 ? (short)(4*18) : (short)(2*18));
			}
			else tileFrameX=0;
			if (direction.X>=0) tileFrameX+=18;

			for (int wireY=Math.Min(plateY,trapY);wireY<=Math.Max(plateY,trapY);wireY++)
			{
				var wiredTile=Main.tile[trapX,wireY];
				wiredTile.RedWire=true;
			}
			for (int wireX=Math.Min(plateX,trapX);wireX<=Math.Max(plateX,trapX);wireX++)
			{
				var wiredTile=Main.tile[wireX,plateY];
				wiredTile.RedWire=true;
			}
		}

		trapX=plateX;
		trapY=plateY;

		ColonyDebug.AddWorldGenMarker(new(plateX,plateY),ModContent.GetInstance<NotSoFreshTrap_Item>().Texture);

		return true;
	}
}