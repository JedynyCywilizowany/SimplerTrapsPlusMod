using System;
using System.Collections.Generic;
using ColonyLib;
using ColonyLib.ContentBases;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SimplerTrapsPlus.Projectiles;

public class EvilDart : ColonyProjectile
{
	private int TargetIndex=>(PlayerTarget ? (int)Projectile.ai[0] : (int)Projectile.ai[0]-300);
	private bool PlayerTarget=>Projectile.ai[0]<300;
	private int graceTime;
	private Entity Target
	{
		get=>(PlayerTarget ? Main.player[TargetIndex] : Main.npc[TargetIndex]);
		set
		{
			Projectile.ai[0]=((value is NPC) ? value.whoAmI+300 : value.whoAmI);
		}
	}

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.NeedsUUID[Type]=true;
	}
	public override void SetDefaults()
	{
		Projectile.trap=true;
		Projectile.hostile=true;

		Projectile.penetrate=-1;
		Projectile.tileCollide=false;
		Projectile.width=28;
		Projectile.height=28;
		Projectile.timeLeft*=5;
		Projectile.netImportant=true;
	}
	public override void OnSpawn(IEntitySource source)
	{
		if (source is EntitySource_Parent source_Parent&&source_Parent.Entity is Player)
		{
			graceTime=60;
			Target=source_Parent.Entity;
		}
	}

	public override void AI()
	{
		if (graceTime>0) graceTime--;
		if (Projectile.ai[0]>=0)
		{
			var target=Target;
			if (!target.active||(!PlayerTarget&&!((NPC)target).isLikeATownNPC)) Projectile.ai[0]=-1;
			else
			{
				if (!(PlayerTarget&&((Player)target).DeadOrGhost))
				{
					Projectile.velocity+=Projectile.DirectionTo(target.Center)*0.1f;
					Projectile.rotation=Projectile.velocity.ToRotation();

					Projectile.velocity*=1-(0.025f*Math.Abs(MathF.Sin(Projectile.rotation)-MathF.Sin(Projectile.AngleTo(target.Center))));

					if (Projectile.timeLeft>3600&&Projectile.Center.DistanceSQ(target.Center)<(64*16)*(64*16))
					{
						Projectile.timeLeft=3600;
					}

					if (graceTime==0&&Projectile.Hitbox.Intersects(target.Hitbox))
					{
						if (PlayerTarget)
						{
							var player=(Player)target;
							if (player.IsLocal())
							{
								player.Hurt(PlayerDeathReason.ByProjectile(Projectile.owner,Projectile.whoAmI),999,Comparer<float>.Default.Compare(player.Center.X,Projectile.Center.X));
							
								if (player.DeadOrGhost) Projectile.Kill();
							}
						}
						else
						{
							if (Main.netMode!=NetmodeID.MultiplayerClient)
							{
								var npc=(NPC)target;
								npc.StrikeInstantKill();
								Projectile.Kill();
							}
						}
					}
				}
			}
		}
		Projectile.velocity*=0.999f;
		if ((Projectile.position.X+Projectile.velocity.X)/16<10) Projectile.velocity.X=Math.Abs(Projectile.velocity.X);
		if ((Projectile.position.X+Projectile.velocity.X)/16>Main.maxTilesX-10) Projectile.velocity.X=-Math.Abs(Projectile.velocity.X);
		if ((Projectile.position.Y+Projectile.velocity.Y)/16<10) Projectile.velocity.Y=Math.Abs(Projectile.velocity.Y);
		if ((Projectile.position.Y+Projectile.velocity.Y)/16>Main.maxTilesY-10) Projectile.velocity.Y=-Math.Abs(Projectile.velocity.Y);
	}
	public override bool? Colliding(Rectangle projHitbox,Rectangle targetHitbox)
	{
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if (!Main.dedServ)
		{
			var dir=Projectile.rotation.ToRotationVector2();
			for (int i=0;i<100;i++)
			{
				Dust.NewDustPerfect(Projectile.Center+(dir*Main.rand.NextFloat(-12,12)),DustID.Blood,Projectile.velocity*Main.rand.NextFloat(),Scale:Main.rand.NextFloat(1f,2f));
			}
		}
	}
}