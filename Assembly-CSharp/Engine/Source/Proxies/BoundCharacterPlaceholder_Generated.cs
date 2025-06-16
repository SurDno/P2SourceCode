using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.BoundCharacters;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BoundCharacterPlaceholder))]
  public class BoundCharacterPlaceholder_Generated : 
    BoundCharacterPlaceholder,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<BoundCharacterPlaceholder_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      BoundCharacterPlaceholder_Generated placeholderGenerated = (BoundCharacterPlaceholder_Generated) target2;
      placeholderGenerated.name = this.name;
      placeholderGenerated.Gender = this.Gender;
      placeholderGenerated.normalSprite = this.normalSprite;
      placeholderGenerated.dangerSprite = this.dangerSprite;
      placeholderGenerated.deadSprite = this.deadSprite;
      placeholderGenerated.diseasedSprite = this.diseasedSprite;
      placeholderGenerated.undiscoveredNormalSprite = this.undiscoveredNormalSprite;
      placeholderGenerated.largeNormalSprite = this.largeNormalSprite;
      placeholderGenerated.largeDangerSprite = this.largeDangerSprite;
      placeholderGenerated.largeDeadSprite = this.largeDeadSprite;
      placeholderGenerated.largeDiseasedSprite = this.largeDiseasedSprite;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteEnum<Gender>(writer, "Gender", this.Gender);
      UnityDataWriteUtility.Write<Sprite>(writer, "NormalSprite", this.normalSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "DangerSprite", this.dangerSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "DeadSprite", this.deadSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "DiseasedSprite", this.diseasedSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "UndiscoveredNormalSprite", this.undiscoveredNormalSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "LargeNormalSprite", this.largeNormalSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "LargeDangerSprite", this.largeDangerSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "LargeDeadSprite", this.largeDeadSprite);
      UnityDataWriteUtility.Write<Sprite>(writer, "LargeDiseasedSprite", this.largeDiseasedSprite);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.Gender = DefaultDataReadUtility.ReadEnum<Gender>(reader, "Gender");
      this.normalSprite = UnityDataReadUtility.Read<Sprite>(reader, "NormalSprite", this.normalSprite);
      this.dangerSprite = UnityDataReadUtility.Read<Sprite>(reader, "DangerSprite", this.dangerSprite);
      this.deadSprite = UnityDataReadUtility.Read<Sprite>(reader, "DeadSprite", this.deadSprite);
      this.diseasedSprite = UnityDataReadUtility.Read<Sprite>(reader, "DiseasedSprite", this.diseasedSprite);
      this.undiscoveredNormalSprite = UnityDataReadUtility.Read<Sprite>(reader, "UndiscoveredNormalSprite", this.undiscoveredNormalSprite);
      this.largeNormalSprite = UnityDataReadUtility.Read<Sprite>(reader, "LargeNormalSprite", this.largeNormalSprite);
      this.largeDangerSprite = UnityDataReadUtility.Read<Sprite>(reader, "LargeDangerSprite", this.largeDangerSprite);
      this.largeDeadSprite = UnityDataReadUtility.Read<Sprite>(reader, "LargeDeadSprite", this.largeDeadSprite);
      this.largeDiseasedSprite = UnityDataReadUtility.Read<Sprite>(reader, "LargeDiseasedSprite", this.largeDiseasedSprite);
    }
  }
}
