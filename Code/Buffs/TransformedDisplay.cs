using System;
using System.Collections.Generic;
using System.Text;
using ColonyLib.ContentBases;
using Humanizer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SimplerTrapsPlus.Buffs;

public class TransformedDisplayBuff : ColonyBuff
{
	public override void SetStaticDefaults()
	{
		BuffID.Sets.NurseCannotRemoveDebuff[Type]=true;
		Main.debuff[Type]=true;
		Main.buffNoSave[Type]=true;
		Main.buffNoTimeDisplay[Type]=true;
	}
	private static readonly SortedDictionary<string,int> locked=new();
	private static readonly StringBuilder tooltipBuilder=new();
	private static readonly string transformationEntryColor=CombatText.HealLife.Hex3();
	private static readonly string transformationTimeLeftColor=CombatText.HealMana.Hex3();
	public override void ModifyBuffText(ref string buffName,ref string tip,ref int rare)
	{
		tooltipBuilder.Clear();
		locked.Clear();
		foreach (var entry in Main.LocalPlayer.GetModPlayer<SimplerTrapsPlusPlayer>().activeTransformations??[])
		{
			if (entry.reapplyCooldown>0)
			{
				tooltipBuilder.AppendLine()
				.Append("[c/")
				.Append(transformationEntryColor)
				.Append(':')
				.Append(Lang.GetItemNameValue(TileLoader.GetItemDropFromTypeAndStyle(entry.trapTileType)))
				.Append("]: [c/")
				.Append(transformationTimeLeftColor)
				.Append(':')
				.Append(new TimeSpan((long)Math.Round(entry.reapplyCooldown/60f)*(TimeSpan.TicksPerSecond)).ToString("g"))
				.Append(']');
			}

			void Add(string key)
			{
				if (entry.timeLeft>locked.GetValueOrDefault(key))
				{
					locked[key]=entry.timeLeft;
				}
			}
			if (entry.forced_IsMale.HasValue) Add("IsMale");
			if (entry.forced_BodyType.HasValue) Add("BodyType");
			if (entry.forced_HairStyle.HasValue) Add("HairStyle");
			if (entry.forced_HairColor.HasValue) Add("HairColor");
			if (entry.forced_SkinColor.HasValue) Add("SkinColor");
			if (entry.forced_EyeColor.HasValue) Add("EyeColor");
			if (entry.forced_ShirtColor.HasValue) Add("ShirtColor");
			if (entry.forced_UnderShirtColor.HasValue) Add("UnderShirtColor");
			if (entry.forced_PantsColor.HasValue) Add("PantsColor");
			if (entry.forced_ShoeColor.HasValue) Add("ShoeColor");
		}

		var tip1=tooltipBuilder.ToString();
		tooltipBuilder.Clear();

		foreach (var l in locked)
		{
			tooltipBuilder.AppendLine()
			.Append("[c/")
			.Append(transformationEntryColor)
			.Append(':')
			.Append(this.GetLocalization("TransformationTooltips."+l.Key))
			.Append("]: [c/")
			.Append(transformationTimeLeftColor)
			.Append(':')
			.Append(new TimeSpan((long)Math.Round(l.Value/60f)*(TimeSpan.TicksPerSecond)).ToString("g"))
			.Append(']');
		}
		
		tip=tip.FormatWith(tooltipBuilder,tip1);
	}
}