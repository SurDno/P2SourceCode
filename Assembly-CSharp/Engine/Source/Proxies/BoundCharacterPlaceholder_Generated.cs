using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Components.BoundCharacters;
using Scripts.Tools.Serializations.Converters;

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
      return ServiceCache.Factory.Instantiate(this);
    }

    public void CopyTo(object target2)
    {
      BoundCharacterPlaceholder_Generated placeholderGenerated = (BoundCharacterPlaceholder_Generated) target2;
      placeholderGenerated.name = name;
      placeholderGenerated.Gender = Gender;
      placeholderGenerated.normalSprite = normalSprite;
      placeholderGenerated.dangerSprite = dangerSprite;
      placeholderGenerated.deadSprite = deadSprite;
      placeholderGenerated.diseasedSprite = diseasedSprite;
      placeholderGenerated.undiscoveredNormalSprite = undiscoveredNormalSprite;
      placeholderGenerated.largeNormalSprite = largeNormalSprite;
      placeholderGenerated.largeDangerSprite = largeDangerSprite;
      placeholderGenerated.largeDeadSprite = largeDeadSprite;
      placeholderGenerated.largeDiseasedSprite = largeDiseasedSprite;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.WriteEnum(writer, "Gender", Gender);
      UnityDataWriteUtility.Write(writer, "NormalSprite", normalSprite);
      UnityDataWriteUtility.Write(writer, "DangerSprite", dangerSprite);
      UnityDataWriteUtility.Write(writer, "DeadSprite", deadSprite);
      UnityDataWriteUtility.Write(writer, "DiseasedSprite", diseasedSprite);
      UnityDataWriteUtility.Write(writer, "UndiscoveredNormalSprite", undiscoveredNormalSprite);
      UnityDataWriteUtility.Write(writer, "LargeNormalSprite", largeNormalSprite);
      UnityDataWriteUtility.Write(writer, "LargeDangerSprite", largeDangerSprite);
      UnityDataWriteUtility.Write(writer, "LargeDeadSprite", largeDeadSprite);
      UnityDataWriteUtility.Write(writer, "LargeDiseasedSprite", largeDiseasedSprite);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      Gender = DefaultDataReadUtility.ReadEnum<Gender>(reader, "Gender");
      normalSprite = UnityDataReadUtility.Read(reader, "NormalSprite", normalSprite);
      dangerSprite = UnityDataReadUtility.Read(reader, "DangerSprite", dangerSprite);
      deadSprite = UnityDataReadUtility.Read(reader, "DeadSprite", deadSprite);
      diseasedSprite = UnityDataReadUtility.Read(reader, "DiseasedSprite", diseasedSprite);
      undiscoveredNormalSprite = UnityDataReadUtility.Read(reader, "UndiscoveredNormalSprite", undiscoveredNormalSprite);
      largeNormalSprite = UnityDataReadUtility.Read(reader, "LargeNormalSprite", largeNormalSprite);
      largeDangerSprite = UnityDataReadUtility.Read(reader, "LargeDangerSprite", largeDangerSprite);
      largeDeadSprite = UnityDataReadUtility.Read(reader, "LargeDeadSprite", largeDeadSprite);
      largeDiseasedSprite = UnityDataReadUtility.Read(reader, "LargeDiseasedSprite", largeDiseasedSprite);
    }
  }
}
