using Microsoft.Xna.Framework;
using Terraria;

namespace SimplerTrapsPlus.Tiles;

public abstract class SimplerTrapBase6Way : SimplerTrapBase4Way
{
	public override Point DirectionFromFrame(int frame)
	{
		return (frame) switch
		{
			0=>new(-1,0),
			1=>new(1,0),
			2=>new(0,-1),
			3=>new(0,-1),
			4=>new(0,1),
			5=>new(0,1),
			_=>default
		};
	}
	public override bool Slope(int x,int y)
	{
		ref var frameX=ref Main.tile[x,y].TileFrameX;
		frameX=(short)((frameX+18)%(18*6));

		return false;
	}
}