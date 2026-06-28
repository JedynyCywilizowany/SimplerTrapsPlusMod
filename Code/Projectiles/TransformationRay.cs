using System;
using ColonyLib;
using ColonyLib.ContentBases;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.Projectiles;

public class TransformationRay : ColonyProjectile
{
	internal const int Duration=120;

	Vector2 startPosition;
	Vector2 endPosition;
	public int trapTileType;
	public override string Texture=>ColonyContentUtils.EmptyTexturePath;
	public override void SetDefaults()
	{
		Projectile.trap=true;
		Projectile.friendly=true;
		Projectile.hostile=true;

		Projectile.timeLeft=Duration;
		Projectile.penetrate=-1;
		Projectile.tileCollide=false;
		Projectile.width=1;
		Projectile.height=1;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Utils.DrawLaser(Main.spriteBatch,TextureAssets.BlackTile.Value,startPosition-Main.screenPosition,endPosition-Main.screenPosition,new Vector2(MathF.Sin(Projectile.timeLeft/(Duration/MathF.PI)),1),
		(int stage,Vector2 currentPosition,float distanceLeft,Rectangle lastFrame,out float distanceCovered,out Rectangle frame,out Vector2 origin,out Color color)=>
		{
			distanceCovered=8;
			frame=new(0,0,8,8);
			origin=new Vector2(4);
			color=Color.Lerp(Color.Black,Color.White,(MathF.Sin((float)((Main.timeForVisualEffects/3f)+(distanceLeft/60f)))+1)/2);
		});
		return false;
	}
	public override bool? Colliding(Rectangle projHitbox,Rectangle targetHitbox)
	{
		return false;
	}
	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override void AI()
	{
		if (endPosition==default)
		{
			SoundEngine.PlaySound(SoundID.Item67,Projectile.position);

			startPosition=Projectile.position;
			endPosition=Projectile.position;
			do
			{
				endPosition+=Projectile.velocity*16;
				foreach (var player in Main.ActivePlayers)
				{
					if (player.Hitbox.Intersects(new((int)endPosition.X-8,(int)endPosition.Y-8,16,16)))
					{
						if (Main.netMode!=NetmodeID.MultiplayerClient)
						{
							player.GetModPlayer<SimplerTrapsPlusPlayer>().HitByTransformationTrap(trapTileType);
							if (CombinedHooks.CanBeHitByProjectile(player,Projectile))
							{
								player.Hurt(PlayerDeathReason.ByProjectile(-1,Projectile.whoAmI),Projectile.damage,0);
							}
						}
						goto stop;
					}
				}
			}
			while (!Collision.SolidCollision(endPosition,1,1)&&endPosition.ChebyshevDistance(startPosition)<16*256);
			stop:

			Projectile.position.X=Math.Min(startPosition.X,endPosition.X);
			Projectile.position.Y=Math.Min(startPosition.Y,endPosition.Y);
			Projectile.width=(int)Math.Abs(startPosition.X-endPosition.X);
			Projectile.height=(int)Math.Abs(startPosition.Y-endPosition.Y);
		}
	}
}