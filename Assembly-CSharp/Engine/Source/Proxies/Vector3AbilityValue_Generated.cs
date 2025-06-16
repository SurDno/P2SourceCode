using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Effects.Values;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Vector3AbilityValue))]
  public class Vector3AbilityValue_Generated : 
    Vector3AbilityValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Vector3AbilityValue_Generated instance = Activator.CreateInstance<Vector3AbilityValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((AbilityValue<Vector3>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.value = UnityDataReadUtility.Read(reader, "Value", this.value);
    }
  }
}
