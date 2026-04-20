using System;
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
		public class ElectricTrap : ColonyTile
		{
			public override void SetStaticDefaults()
			{
				Main.tileSolid[Type]=true;
				Main.tileBlockLight[Type]=true;
				Main.tileFrameImportant[Type]=true;
				TileID.Sets.IsAMechanism[Type]=true;
				TileID.Sets.DoesntGetReplacedWithTileReplacement[Type]=true;
				TileID.Sets.DontDrawTileSliced[Type]=true;

				DustType=DustID.Electric;

				AddMapEntry(Color.Gray,ModContent.GetInstance<Items.ElectricTrap_Item>().DisplayName);
			}
			public override bool Slope(int x,int y)
			{
				return false;
			}
			public override void HitWire(int x, int y)
			{
				if (MechCooldown.Apply(x,y,300+ElectricTrap_Projectile.Duration))
				{
					Projectile.NewProjectile(new EntitySource_Wiring(x,y),new Point(x,y).ToWorldCoordinates(),Vector2.Zero,ModContent.ProjectileType<ElectricTrap_Projectile>(),60,1);
				}
			}
		}
	}
	namespace Items
	{
		public class ElectricTrap_Item : ColonyItem
		{
			public override string Texture=>ModContent.GetInstance<Tiles.ElectricTrap>().Texture;
			public override void SetStaticDefaults()
			{
				ItemID.Sets.ShimmerTransformToItem[Type]=ItemID.DartTrap;
			}
			public override void SetDefaults()
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ElectricTrap>());
				Item.value=5000;
				Item.mech=true;
			}
			public override void AddRecipes()
			{
				foreach (var jellyfish in (Span<int>)[ItemID.BlueJellyfish,ItemID.GreenJellyfish,ItemID.PinkJellyfish])
				{
					CreateRecipe()
					.AddIngredient(ItemID.DartTrap)
					.AddIngredient(ItemID.Wire,5)
					.AddIngredient(jellyfish)
					.AddTile(TileID.Anvils)
					.Register();
				}
			}
		}
	}
}