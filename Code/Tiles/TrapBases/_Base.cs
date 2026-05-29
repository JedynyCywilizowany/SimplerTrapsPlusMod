using ColonyLib.ContentBases;
using ColonyLib.TileEntities;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public abstract class SimplerTrapBase : ColonyTile
		{
			public abstract int Cooldown{get;}
			public abstract int ProjectileType{get;}
			public abstract int ProjectileDamage{get;}
			public virtual Point DirectionFromFrame(int frame)
			{
				return default;
			}
			public virtual void ModifyProjectile(int x,int y,Projectile projectile)
			{
			}

			public override void SetStaticDefaults()
			{
				Main.tileSolid[Type]=true;
				Main.tileBlockLight[Type]=true;
				Main.tileFrameImportant[Type]=true;
				TileID.Sets.IsAMechanism[Type]=true;
				TileID.Sets.DoesntGetReplacedWithTileReplacement[Type]=true;
				TileID.Sets.DontDrawTileSliced[Type]=true;
			}
			public override bool Slope(int x,int y)
			{
				return false;
			}
			public override void HitWire(int x, int y)
			{
				if (MechCooldown.Apply(x,y,Cooldown))
				{
					ModifyProjectile(x,y,Projectile.NewProjectileDirect(new EntitySource_Wiring(x,y),new Point(x,y).ToWorldCoordinates(),DirectionFromFrame(Main.tile[x,y].TileFrameX/18).ToVector2(),ProjectileType,ProjectileDamage,1));
				}
			}
		}
	}
	namespace Items
	{
		public abstract class SimplerTrapBase_Item<TTile> : ColonyItem where TTile : SimplerTrapBase
		{
			public override void SetStaticDefaults()
			{
				Item.ResearchUnlockCount=5;
			}
			public override void SetDefaults()
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<TTile>());
				Item.value=5000;
				Item.mech=true;
			}
		}
	}
}