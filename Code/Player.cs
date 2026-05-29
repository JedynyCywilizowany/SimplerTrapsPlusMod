using System.Collections.Generic;
using System.IO;
using ColonyLib;
using ColonyLib.ContentBases;
using Microsoft.Xna.Framework;
using SimplerTrapsPlus.Buffs;
using SimplerTrapsPlus.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SimplerTrapsPlus;

public class SimplerTrapsPlusPlayer : ModPlayer
{
	public class ActiveTransformation
	{
		internal int timeLeft;
		internal int reapplyCooldown;
		internal int trapTileType;

		public bool? forced_IsMale;
		public int? forced_BodyType;
		public int? forced_HairStyle;
		public Color? forced_HairColor;
		public Color? forced_SkinColor;
		public Color? forced_EyeColor;
		public Color? forced_ShirtColor;
		public Color? forced_UnderShirtColor;
		public Color? forced_PantsColor;
		public Color? forced_ShoeColor;

		internal void Apply(Player player)
		{
			var isMale=forced_IsMale??PlayerVariantID.Sets.Male[player.skinVariant];

			if (forced_BodyType.HasValue) player.skinVariant=forced_BodyType.Value;
			
			if (PlayerVariantID.Sets.Male[player.skinVariant]!=isMale)
			{
				player.skinVariant=PlayerVariantID.Sets.AltGenderReference[player.skinVariant];
			}

			if (forced_HairStyle.HasValue) player.hair=forced_HairStyle.Value;
			if (forced_HairColor.HasValue) player.hairColor=forced_HairColor.Value;
			if (forced_SkinColor.HasValue) player.skinColor=forced_SkinColor.Value;
			if (forced_EyeColor.HasValue) player.eyeColor=forced_EyeColor.Value;
			if (forced_ShirtColor.HasValue) player.shirtColor=forced_ShirtColor.Value;
			if (forced_UnderShirtColor.HasValue) player.underShirtColor=forced_UnderShirtColor.Value;
			if (forced_PantsColor.HasValue) player.pantsColor=forced_PantsColor.Value;
			if (forced_ShoeColor.HasValue) player.shoeColor=forced_ShoeColor.Value;
		}
	}
	public List<ActiveTransformation> activeTransformations=null!;

	public override void PreUpdate()
	{
		if (Player.IsLocal()&&activeTransformations is not null&&activeTransformations.Count!=0)
		{
			Player.AddBuff(ModContent.BuffType<TransformedDisplayBuff>(),10);
			for (int i=0;i<activeTransformations.Count;i++)
			{
				var entry=activeTransformations[i];
				if ((--entry.timeLeft)<0)
				{
					activeTransformations.RemoveAt(i);
					i--;
				}
				else
				{
					if (entry.reapplyCooldown>0) entry.reapplyCooldown--;
					entry.Apply(Player);
				}
			}
		}
	}

	public class SyncTransformationHit : ColonyPacketType
	{
		public override void HandleClient(BinaryReader reader,int whoAmI)
		{
			var playerIndex=reader.ReadByte();
			var trapTileType=reader.ReadUInt16();
			var player=Main.player[playerIndex];

			HitByTransformationTrap_Animation(player);

			if (playerIndex==Main.myPlayer)
			{
				var modPlayer=player.GetModPlayer<SimplerTrapsPlusPlayer>();
				modPlayer.HitByTransformationTrap(trapTileType);
				foreach (var effect in modPlayer.activeTransformations)
				{
					effect.Apply(player);
				}
				NetMessage.SendData(MessageID.SyncPlayer,number:player.whoAmI);
			}
		}
	}
	private const int TransformationLockTime=30*60*60;
	private const int TransformationReapplyCooldown=TransformationLockTime/2;
	private static void HitByTransformationTrap_Animation(Player player)
	{
		SoundEngine.PlaySound(SoundID.Item176,player.position);
		for (int i=0;i<100;i++)
		{
			Dust.NewDustDirect(player.position,player.width,player.height,DustID.TintableDust,Main.rand.NextFloat(-2f,2f),Main.rand.NextFloat(-2f,2f),newColor:ColonyUtils.RandomColor(),Scale:Main.rand.NextFloat(1f,2.5f));
		}
	}
	internal void HitByTransformationTrap(int trapTileType)
	{
		if (Main.dedServ)
		{
			var p=ColonyPacket.Get<SyncTransformationHit>();
			p.Write((byte)Player.whoAmI);
			p.Write((ushort)trapTileType);
			p.Send();
		}
		else
		{
			HitByTransformationTrap_Animation(Player);
			
			activeTransformations??=new();
			bool isNewlyApplied=true;
			ActiveTransformation? effects=null;
			foreach (var effect in activeTransformations)
			{
				if (effect.trapTileType==trapTileType)
				{
					if (effect.reapplyCooldown>0)
					{
						effect.timeLeft=TransformationLockTime;
						return;
					}
					else
					{
						effects=effect;
						isNewlyApplied=false;
						break;
					}
				}
			}

			effects??=new();
			var trap=(TransformationTrapBase)TileLoader.GetTile(trapTileType);
			trap.SetTransformations(Player,effects);
			effects.trapTileType=trapTileType;
			effects.timeLeft=TransformationLockTime;
			effects.reapplyCooldown=TransformationReapplyCooldown;
			if (isNewlyApplied) activeTransformations.Add(effects);
		}
	}

	public override void SaveData(TagCompound tag)
	{
		List<TagCompound> all=new();
		foreach (var entry in activeTransformations??[])
		{
			TagCompound e=new();
			e[nameof(ActiveTransformation.trapTileType)]=TileID.Search.GetName(entry.trapTileType);
			e[nameof(ActiveTransformation.timeLeft)]=entry.timeLeft;
			e.AddIfNotDefault(nameof(ActiveTransformation.reapplyCooldown),entry.reapplyCooldown);

			e.AddIfNotNull(nameof(ActiveTransformation.forced_IsMale),entry.forced_IsMale);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_BodyType),entry.forced_BodyType);
			if (entry.forced_HairStyle.HasValue)
			{
				if (entry.forced_HairStyle.Value>=HairID.Count) e.Add(nameof(ActiveTransformation.forced_HairStyle)+"_Modded",HairID.Search.GetName(entry.forced_HairStyle.Value));
				else e.Add(nameof(ActiveTransformation.forced_HairStyle),entry.forced_HairStyle.Value);
			}
			e.AddIfNotNull(nameof(ActiveTransformation.forced_HairColor),entry.forced_HairColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_SkinColor),entry.forced_SkinColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_EyeColor),entry.forced_EyeColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_ShirtColor),entry.forced_ShirtColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_UnderShirtColor),entry.forced_UnderShirtColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_PantsColor),entry.forced_PantsColor);
			e.AddIfNotNull(nameof(ActiveTransformation.forced_ShoeColor),entry.forced_ShoeColor);

			all.Add(e);
		}
		tag.AddIfNotEmpty(nameof(activeTransformations),all);
	}
	public override void LoadData(TagCompound tag)
	{
		activeTransformations=new();
		foreach (var entry in tag.Get<List<TagCompound>>(nameof(activeTransformations)))
		{
			ActiveTransformation loadedEntry=new()
			{
				trapTileType=TileID.Search.GetId(entry.Get<string>(nameof(ActiveTransformation.trapTileType))),
				timeLeft=entry.Get<int>(nameof(ActiveTransformation.timeLeft)),
				reapplyCooldown=entry.Get<int>(nameof(ActiveTransformation.reapplyCooldown))
			};
			if (entry.TryGet<bool>(nameof(ActiveTransformation.forced_IsMale),out var IsMale)) loadedEntry.forced_IsMale=IsMale;
			if (entry.TryGet<int>(nameof(ActiveTransformation.forced_BodyType),out var BodyType)) loadedEntry.forced_BodyType=BodyType;
			if (entry.TryGet<int>(nameof(ActiveTransformation.forced_HairStyle),out var HairStyle)) loadedEntry.forced_HairStyle=HairStyle;
			else if (entry.TryGet<string>(nameof(ActiveTransformation.forced_HairStyle)+"_Modded",out var HairStyleModded)&&HairID.Search.TryGetId(HairStyleModded,out HairStyle)) loadedEntry.forced_HairStyle=HairStyle;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_HairColor),out var Color)) loadedEntry.forced_HairColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_SkinColor),out Color)) loadedEntry.forced_SkinColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_EyeColor),out Color)) loadedEntry.forced_EyeColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_ShirtColor),out Color)) loadedEntry.forced_ShirtColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_UnderShirtColor),out Color)) loadedEntry.forced_UnderShirtColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_PantsColor),out Color)) loadedEntry.forced_PantsColor=Color;
			if (entry.TryGet<Color>(nameof(ActiveTransformation.forced_ShoeColor),out Color)) loadedEntry.forced_ShoeColor=Color;

			activeTransformations.Add(loadedEntry);
		}
	}
}