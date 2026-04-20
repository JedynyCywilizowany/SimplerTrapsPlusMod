using ColonyLib.ContentBases;
using ColonyLib.TileEntities;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus
{
	namespace Tiles
	{
		public class NotSoFreshTrap : ColonyTile
		{
			public override void SetStaticDefaults()
			{
				Main.tileSolid[Type]=true;
				Main.tileBlockLight[Type]=true;
				Main.tileFrameImportant[Type]=true;
				TileID.Sets.IsAMechanism[Type]=true;
				TileID.Sets.DoesntGetReplacedWithTileReplacement[Type]=true;
				TileID.Sets.DontDrawTileSliced[Type]=true;

				DustType=DustID.FartInAJar;

				AddMapEntry(Color.Lerp(Color.Gray,Color.DarkGreen,0.5f),ModContent.GetInstance<Items.NotSoFreshTrap_Item>().DisplayName);
			}
			public override bool Slope(int x,int y)
			{
				ref var frameX=ref Main.tile[x,y].TileFrameX;
				frameX=(short)((frameX+18)%(18*6));

				return false;
			}
			public override void HitWire(int x, int y)
			{
				if (MechCooldown.Apply(x,y,300+NotSoFreshTrap_Projectile.Duration))
				{
					Vector2 projDir=(Main.tile[x,y].TileFrameX/18) switch
					{
						0=>new(-1,0),
						1=>new(1,0),
						2=>new(0,-1),
						3=>new(0,-1),
						4=>new(0,1),
						5=>new(0,1),
						_=>default
					};
					Projectile.NewProjectile(new EntitySource_Wiring(x,y),new Point(x,y).ToWorldCoordinates(),projDir,ModContent.ProjectileType<NotSoFreshTrap_Projectile>(),20,0);
				}
			}
			public override void PlaceInWorld(int x,int y,Item item)
			{
				Main.tile[x,y].TileFrameX=(Main.LocalPlayer.direction<0 ? (short)0 : (short)18);
			}
		}
	}
	namespace Items
	{
		public class NotSoFreshTrap_Item : ColonyItem
		{
			public override void SetStaticDefaults()
			{
				ItemID.Sets.ShimmerTransformToItem[Type]=ItemID.DartTrap;
			}
			public override void SetDefaults()
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.NotSoFreshTrap>());
				Item.value=5000;
				Item.mech=true;
			}
			public override void AddRecipes()
			{
				CreateRecipe()
				.AddIngredient(ItemID.DartTrap)
				.AddIngredient(ItemID.Wire,5)
				.AddIngredient(ItemID.GasTrap)
				.AddTile(TileID.Anvils)
				.Register();
			}
		}
	}
}