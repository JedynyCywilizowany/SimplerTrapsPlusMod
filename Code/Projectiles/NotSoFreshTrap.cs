using System;
using System.Collections.Generic;
using System.Linq;
using ColonyLib;
using ColonyLib.ContentBases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.Projectiles;

public class NotSoFreshTrap_Projectile : ColonyProjectile
{
	internal const int Duration=60*30;
	private const int FadeTime=Duration/2;
	internal const int SprayDuration=(Duration*2/4);
	internal const float SprayPower=0.75f;
	private const int SprayRange=16;

	private bool IsFading=>(Projectile.timeLeft<=FadeTime);
	private float Fade=>(!IsFading ? 1f : (float)Projectile.timeLeft/FadeTime);
	internal static SoundStyle? activateSound;

	private Dictionary<Point,float> affectedTiles=new();
	private Dictionary<Point,(Point direction,int timeLeft)> sprays=new();
	public override LocalizedText DisplayName=>ModContent.GetInstance<NotSoFreshTrap_Item>().DisplayName;
	public override void SetDefaults()
	{
		Projectile.trap=true;
		Projectile.friendly=true;
		Projectile.hostile=true;

		Projectile.timeLeft=Duration+FadeTime;
		Projectile.penetrate=-1;
		Projectile.tileCollide=false;
		Projectile.width=8;
		Projectile.height=8;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Main.instance.LoadProjectile(Type);
		var texture=TextureAssets.Projectile[Type].Value;
		var fade=Fade;
		foreach (var pos in affectedTiles.Keys)
		{
			Vector2 drawPos=new(pos.X*16+8,pos.Y*16+8);
			for (int y2=-1;y2<=1;y2++) for (int x2=-1;x2<=1;x2++)
			{
				var movedPos=pos+new Point(x2,y2);
				if ((x2==0&&y2==0)||!affectedTiles.ContainsKey(movedPos))
				{
					var effects=(SpriteEffects)(Main.timeForVisualEffects/6%3);
					Rectangle frame=new((1+x2)*16,(1+y2)*16,16,16);
					switch (effects)
					{
						case SpriteEffects.FlipHorizontally:
						{
							frame.X=32-frame.X;
						}break;
						case SpriteEffects.FlipVertically:
						{
							frame.Y=32-frame.Y;
						}break;
					}
					var color=Lighting.GetColor(movedPos);
					color.A=(byte.MaxValue*7/8);
					color*=fade;

					Main.EntitySpriteDraw(texture,drawPos+new Vector2(x2*16,y2*16)-Main.screenPosition,frame,color,0/*Main.rand.Next(4)*(MathF.PI/2)*/,new Vector2(8,8),1,effects);
				}
			}
		}
		return false;
	}
	public override bool? Colliding(Rectangle projHitbox,Rectangle targetHitbox)
	{
		if (IsFading) return false;
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
	public override void ModifyHitPlayer(Player target,ref Player.HurtModifiers modifiers)
	{
		target.AddBuff(BuffID.Stinky,60*60);
		target.AddBuff(BuffID.Poisoned,10*60);
		modifiers.HitDirectionOverride=0;
	}
	public override void ModifyHitNPC(NPC target,ref NPC.HitModifiers modifiers)
	{
		target.AddBuff(BuffID.Stinky,60*60);
		target.AddBuff(BuffID.Poisoned,10*60);
		modifiers.HitDirectionOverride=0;
	}

	private static List<Point> affectedTilesSnapshot=new();
	private static List<(Point pos,float difference)> spreadQueue=new();
	public override void AI()
	{
		if (affectedTiles.Count==0&&sprays.Count==0)
		{
			if (!Main.dedServ)
			{
				SoundEngine.PlaySound(activateSound??=new(this.OwnSoundPath("Activate"))
				{
					Volume=1f,
					Pitch=0.025f,
					PitchVariance=0.01f,
					MaxInstances=3
				},Projectile.position);
			}
			
			sprays[(Projectile.position/16).ToPoint()]=(Projectile.velocity.ToPoint(),SprayDuration);
			Projectile.velocity=Vector2.Zero;
		}

		bool ValidPass(Point point,Point direction,bool firstTime=false)
		{
			if (!WorldGen.InWorld(point.X,point.Y)) return false;

			var tile=Main.tile[point];
			if (tile.LiquidAmount==byte.MaxValue) return false;

			bool NotBlocked(Point point,Point direction)
			{
				var tile=Main.tile[point];
				if (!WorldGen.SolidOrSlopedTile(tile)) return true;
				//I know this looks weird...
				return (((direction.X+1)<<2)|(direction.Y+1)) switch
				{
					((0<<2)|1)=>tile.RightSlope||tile.IsHalfBlock,
					((2<<2)|1)=>tile.LeftSlope||tile.IsHalfBlock,
					((1<<2)|0)=>tile.BottomSlope,
					((1<<2)|2)=>tile.TopSlope||tile.IsHalfBlock,
					_=>true
				};
			}
			return NotBlocked(point,direction)&&(firstTime||NotBlocked(point-direction,new Point(-direction.X,-direction.Y)));
		}

		int s=0;
		Span<Point> spraysCache=stackalloc Point[sprays.Count];
		foreach (var spr in sprays.Keys) spraysCache[s++]=spr;

		foreach (var sprayOrigin in spraysCache)
		{
			var (direction,timeLeft)=sprays[sprayOrigin];

			timeLeft--;
			if (timeLeft<0) sprays.Remove(sprayOrigin);
			else
			{
				var tile=Main.tile[sprayOrigin];
				if (!tile.HasTile||tile.TileType!=ModContent.TileType<NotSoFreshTrap>()) timeLeft=0;
				else
				{
					Point p=sprayOrigin;
					do
					{
						p+=direction;
					}
					while (ValidPass(p,direction,sprayOrigin.ManhattanDistance(p)<2)&&affectedTiles.ContainsKey(p)&&p.ManhattanDistance(sprayOrigin)<SprayRange);
					if (!ValidPass(p,direction,sprayOrigin.ManhattanDistance(p)<2)) p-=direction;

					var val=affectedTiles.GetValueOrDefault(p,-1);
					affectedTiles[p]=val+SprayPower;

					timeLeft--;
				}

				sprays[sprayOrigin]=(direction,timeLeft);
			}
		}
		
		var point=affectedTiles.Keys.FirstOrDefault();
		int leftX=point.X;
		int rightX=point.X;
		int topY=point.Y;
		int bottomY=point.Y;

		affectedTilesSnapshot.Clear();
		foreach (var t in affectedTiles.Keys)
		{
			if (t.X<leftX) leftX=t.X;
			if (t.X>rightX) rightX=t.X;
			if (t.Y<topY) topY=t.Y;
			if (t.Y>bottomY) bottomY=t.Y;

			affectedTilesSnapshot.Add(t);
		}
		Rectangle ownRect=new((leftX-1)*16,(topY-1)*16,(rightX-leftX+2)*16,(bottomY-topY+2)*16);
		Projectile.Hitbox=ownRect;

		foreach (var t in affectedTilesSnapshot)
		{
			var density=affectedTiles[t];
			if (density>0)
			{
				int x=t.X;
				int y=t.Y;
				spreadQueue.Clear();

				Spread(x,y-1);
				Spread(x,y+1);
				Spread(x-1,y);
				Spread(x+1,y);

				float sum=0;
				foreach (var (pos,difference) in spreadQueue) sum+=difference;

				foreach (var (pos,difference) in spreadQueue)
				{
					var distribution=difference/sum/2f;
					var transfer=difference*distribution;
					var val=affectedTiles.GetValueOrDefault(pos,-1);
					affectedTiles[pos]=val+transfer;
					density-=transfer;
				}
				affectedTiles[t]=density;
			}

			void Spread(int sx,int sy)
			{
				Point pos=new(sx,sy);
				if (ValidPass(pos,pos-t))
				{
					var targetDens=affectedTiles.GetValueOrDefault(pos,-1);
					if (targetDens<density) spreadQueue.Add((pos,density-targetDens));
				}
			}
		}

		if (!IsFading) foreach (var targetProj in Main.ActiveProjectiles)
		{
			if (Projectile.whoAmI!=targetProj.whoAmI&&targetProj.type==ModContent.ProjectileType<NotSoFreshTrap_Projectile>()&&targetProj.ModProjectile is NotSoFreshTrap_Projectile targetModProj&&!targetModProj.IsFading)
			{
				if (ownRect.Intersects(targetProj.Hitbox)&&affectedTiles.Keys.Any(targetModProj.affectedTiles.Keys.Contains))
				{
					foreach (var entry in targetModProj.affectedTiles)
					{
						var val=affectedTiles.GetValueOrDefault(entry.Key,0);
						affectedTiles[entry.Key]=val+entry.Value;
					}
					foreach (var entry in targetModProj.sprays)
					{
						var val=sprays.GetValueOrDefault(entry.Key).timeLeft;
						if (entry.Value.timeLeft>val) sprays[entry.Key]=entry.Value;
					}
					Projectile.timeLeft=Math.Max(Projectile.timeLeft,targetProj.timeLeft);

					targetProj.active=false;
				}
			}
		}
		
	}
	public override bool? CanCutTiles()
	{
		return false;
	}
	public override bool ShouldUpdatePosition()
	{
		return false;
	}
}