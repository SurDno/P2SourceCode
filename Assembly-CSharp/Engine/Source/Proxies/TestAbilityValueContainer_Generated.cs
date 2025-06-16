using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons.Abilities;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TestAbilityValueContainer))]
  public class TestAbilityValueContainer_Generated : 
    TestAbilityValueContainer,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<TestAbilityValueContainer_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      TestAbilityValueContainer_Generated containerGenerated = (TestAbilityValueContainer_Generated) target2;
      containerGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<AbilityValueInfo>(containerGenerated.values, this.values);
      containerGenerated.blueprint = this.blueprint;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<AbilityValueInfo>(writer, "Values", this.values);
      UnityDataWriteUtility.Write<GameObject>(writer, "Blueprint", this.blueprint);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.values = DefaultDataReadUtility.ReadListSerialize<AbilityValueInfo>(reader, "Values", this.values);
      this.blueprint = UnityDataReadUtility.Read<GameObject>(reader, "Blueprint", this.blueprint);
    }
  }
}
