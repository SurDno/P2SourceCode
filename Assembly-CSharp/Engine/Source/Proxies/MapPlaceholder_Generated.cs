// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.MapPlaceholder_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.Maps;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MapPlaceholder))]
  public class MapPlaceholder_Generated : 
    MapPlaceholder,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<MapPlaceholder_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      MapPlaceholder_Generated placeholderGenerated = (MapPlaceholder_Generated) target2;
      placeholderGenerated.name = this.name;
      placeholderGenerated.kind = this.kind;
      placeholderGenerated.mainSprite = this.mainSprite;
      placeholderGenerated.hoverSprite = this.hoverSprite;
      placeholderGenerated.normalSprite = this.normalSprite;
      placeholderGenerated.shadowSprite = this.shadowSprite;
      placeholderGenerated.alphaRaycast = this.alphaRaycast;
      placeholderGenerated.npcSprite = this.npcSprite;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteEnum<MapPlaceholderKind>(writer, "Kind", this.kind);
      UnityDataWriteUtility.Write<Sprite>(writer, "MainSprite", this.mainSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "HoverSprite", this.hoverSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "NormalSprite", this.normalSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "ShadowSprite", this.shadowSprite);
      DefaultDataWriteUtility.Write(writer, "AlphaRaycast", this.alphaRaycast);
      UnityDataWriteUtility.Write<Sprite>(writer, "NPCSprite", this.npcSprite);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.kind = DefaultDataReadUtility.ReadEnum<MapPlaceholderKind>(reader, "Kind");
      this.mainSprite = UnityDataReadUtility.Read<Sprite>(reader, "MainSprite", this.mainSprite);
      this.hoverSprite = UnityDataReadUtility.Read<Sprite>(reader, "HoverSprite", this.hoverSprite);
      this.normalSprite = UnityDataReadUtility.Read<Sprite>(reader, "NormalSprite", this.normalSprite);
      this.shadowSprite = UnityDataReadUtility.Read<Sprite>(reader, "ShadowSprite", this.shadowSprite);
      this.alphaRaycast = DefaultDataReadUtility.Read(reader, "AlphaRaycast", this.alphaRaycast);
      this.npcSprite = UnityDataReadUtility.Read<Sprite>(reader, "NPCSprite", this.npcSprite);
    }
  }
}
