using System;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Items;
using SimplerTrapsPlus.Projectiles;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public abstract class TransformationTrapBase : SimplerTrapBase4Way
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();

				DustType=DustID.Stone;

				AddMapEntry(Color.Gray,ModContent.GetInstance<TransformationTrap_Empty_Item>().DisplayName);
			}

			public abstract void SetTransformations(Player player,SimplerTrapsPlusPlayer.ActiveTransformation effects);

			public override int Cooldown=>600;
			public override int ProjectileType=>ModContent.ProjectileType<TransformationRay>();
			public override int ProjectileDamage=>100;
			public override void ModifyProjectile(int x,int y,Projectile projectile)
			{
				((TransformationRay)projectile.ModProjectile).trapTileType=Type;
			}
			
			protected static Color GetNeutral(Color color)
			{
				int v=Math.Max(color.R,Math.Max(color.G,color.B));
				return new(v,v,v);
			}
		}
	}
	namespace Items
	{
		public class TransformationTrapBase_Item<TTile> : SimplerTrapBase_Item<TTile> where TTile : TransformationTrapBase
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();
				ItemID.Sets.ShimmerTransformToItem[Type]=ModContent.ItemType<TransformationTrap_Empty_Item>();
			}
		}
	}
}