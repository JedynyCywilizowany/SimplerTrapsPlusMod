using Microsoft.Xna.Framework;
using Terraria;

namespace SimplerTrapsPlus.Tiles;

public abstract class SimplerTrapBase4Way : SimplerTrapBase
{
	public override Point DirectionFromFrame(int frame)
	{
		return (frame) switch
		{
			0=>new(-1,0),
			1=>new(1,0),
			2=>new(0,-1),
			3=>new(0,1),
			_=>default
		};
	}
	public override void PlaceInWorld(int x,int y,Item item)
	{
		Main.tile[x,y].TileFrameX=(Main.LocalPlayer.direction<0 ? (short)0 : (short)18);
	}
	public override bool Slope(int x,int y)
	{
		ref var frameX=ref Main.tile[x,y].TileFrameX;
		frameX=(short)((frameX+18)%(18*4));

		return false;
	}
}