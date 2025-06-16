using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Effects.Values;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Vector2AbilityValue))]
  public class Vector2AbilityValue_Generated : 
    Vector2AbilityValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Vector2AbilityValue_Generated instance = Activator.CreateInstance<Vector2AbilityValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((AbilityValue<Vector2>) target2).value = value;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      value = UnityDataReadUtility.Read(reader, "Value", value);
    }
  }
}
