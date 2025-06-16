using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.Maps;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MapPlaceholder))]
public class MapPlaceholder_Generated :
	MapPlaceholder,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var placeholderGenerated = (MapPlaceholder_Generated)target2;
		placeholderGenerated.name = name;
		placeholderGenerated.kind = kind;
		placeholderGenerated.mainSprite = mainSprite;
		placeholderGenerated.hoverSprite = hoverSprite;
		placeholderGenerated.normalSprite = normalSprite;
		placeholderGenerated.shadowSprite = shadowSprite;
		placeholderGenerated.alphaRaycast = alphaRaycast;
		placeholderGenerated.npcSprite = npcSprite;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteEnum(writer, "Kind", kind);
		UnityDataWriteUtility.Write(writer, "MainSprite", mainSprite);
		UnityDataWriteUtility.Write(writer, "HoverSprite", hoverSprite);
		UnityDataWriteUtility.Write(writer, "NormalSprite", normalSprite);
		UnityDataWriteUtility.Write(writer, "ShadowSprite", shadowSprite);
		DefaultDataWriteUtility.Write(writer, "AlphaRaycast", alphaRaycast);
		UnityDataWriteUtility.Write(writer, "NPCSprite", npcSprite);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		kind = DefaultDataReadUtility.ReadEnum<MapPlaceholderKind>(reader, "Kind");
		mainSprite = UnityDataReadUtility.Read(reader, "MainSprite", mainSprite);
		hoverSprite = UnityDataReadUtility.Read(reader, "HoverSprite", hoverSprite);
		normalSprite = UnityDataReadUtility.Read(reader, "NormalSprite", normalSprite);
		shadowSprite = UnityDataReadUtility.Read(reader, "ShadowSprite", shadowSprite);
		alphaRaycast = DefaultDataReadUtility.Read(reader, "AlphaRaycast", alphaRaycast);
		npcSprite = UnityDataReadUtility.Read(reader, "NPCSprite", npcSprite);
	}
}