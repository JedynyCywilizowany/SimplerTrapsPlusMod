using ColonyLib.ContentBases;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.Projectiles;

public class EvilDart_Item : ColonyItem
{
	public override LocalizedText DisplayName=>ModContent.GetInstance<EvilDart>().DisplayName;
	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type]=Type;
		Item.ResearchUnlockCount=100;
	}
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.PoisonDart);
		Item.shoot=ModContent.ProjectileType<EvilDart>();
		Item.damage=999;
		Item.crit=999;
		Item.knockBack=0;
		Item.value=0;
	}
	public override void OnResearched(bool fullyResearched)
	{
		if (Main.LocalPlayerCreativeTracker.ItemSacrifices.TryGetSacrificeNumbers(Type,out var amountWeHave,out _))
		{
			if (amountWeHave>99) Main.LocalPlayerCreativeTracker.ItemSacrifices.SetSacrificeCountDirectly(FullName,99);
		}
	}
}