using System;
using System.Collections.Generic;
using ColonyLib.ContentBases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimplerTrapsPlus.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.Projectiles;

public class ElectricTrap_Projectile : ColonyProjectile
{
	internal const int Duration=150;
	internal const float Range_Air=3;
	internal const float Range_Solid=10;
	internal const float Range_Water=50;

	internal static SoundStyle activeSound=new(ModContent.GetInstance<ElectricTrap_Projectile>().OwnSoundPath("Activate"))
	{
		Volume=1f,
		PitchVariance=0.25f,
		MaxInstances=2,
	};

	private Dictionary<Point,float> affectedTiles=new();
	public override LocalizedText DisplayName=>ModContent.GetInstance<ElectricTrap_Item>().DisplayName;
	public override void SetDefaults()
	{
		Projectile.trap=true;
		Projectile.friendly=true;
		Projectile.hostile=true;

		Projectile.timeLeft=Duration;
		Projectile.penetrate=-1;
		Projectile.tileCollide=false;
		Projectile.width=8;
		Projectile.height=8;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		var color=Color.Lerp(Color.Cyan,Color.Aquamarine,Main.rand.NextFloat());
		var density=Main.rand.Next(2);
		foreach (var pos in affectedTiles.Keys)
		{
			Vector2 drawPos=new(pos.X*16,pos.Y*16);
			for (int i=Main.rand.Next(2);i<=density;i++) Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,drawPos+new Vector2(Main.rand.NextFloat()*16,Main.rand.NextFloat()*16)-Main.screenPosition,new Rectangle(0,Main.rand.Next(4)*16,8,16),color,Main.rand.Next(4)*(MathF.PI/2),new Vector2(0,8),1,(Main.rand.NextBool() ? SpriteEffects.FlipVertically : SpriteEffects.None));
			if (Main.rand.NextBool(180)) Dust.NewDustDirect(drawPos,16,16,DustID.Electric);
		}
		return false;
	}
	public override bool? Colliding(Rectangle projHitbox,Rectangle targetHitbox)
	{
		var x=targetHitbox.X/16;
		var y=targetHitbox.Y/16;
		var x2=(targetHitbox.X+targetHitbox.Width)/16+1;
		var y2=(targetHitbox.Y+targetHitbox.Height)/16+1;
		Rectangle tileHitbox=new(x,y,x2-x,y2-y);

		foreach (var pos in affectedTiles.Keys)
		{
			if (tileHitbox.Contains(pos)) return true;
		}
		return false;
	}

	public override void AI()
	{
		if (affectedTiles.Count==0)
		{
			SoundEngine.PlaySound(activeSound,Projectile.position);

			Spread((int)(Projectile.position.X/16),(int)(Projectile.position.Y/16),1);
			
			void Spread(int sx,int sy,float rangeLeft)
			{
				if (!WorldGen.InWorld(sx,sy)) return;

				Point pos=new(sx,sy);
				if (!affectedTiles.TryGetValue(pos,out var r)||r<rangeLeft)
				{
					affectedTiles[pos]=rangeLeft;

					float rangeReduction;
					if (WorldGen.SolidOrSlopedTile(sx,sy)) rangeReduction=1/Range_Solid;
					else
					{
						var tile=Main.tile[sx,sy];
						if (tile.LiquidAmount!=0)
						{
							if (tile.LiquidType==LiquidID.Lava) rangeReduction=1/Range_Solid;
							else rangeReduction=1/Range_Water;
						}
						else rangeReduction=1/Range_Air;
					}

					rangeLeft-=rangeReduction;
					if (rangeLeft>0)
					{
						Spread(sx,sy-1,rangeLeft);
						Spread(sx,sy+1,rangeLeft);
						Spread(sx-1,sy,rangeLeft);
						Spread(sx+1,sy,rangeLeft);
					}
				}
			}
		}
	}
}