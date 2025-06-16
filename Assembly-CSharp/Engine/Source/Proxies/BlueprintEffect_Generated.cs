using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BlueprintEffect))]
  public class BlueprintEffect_Generated : 
    BlueprintEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BlueprintEffect_Generated instance = Activator.CreateInstance<BlueprintEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BlueprintEffect_Generated blueprintEffectGenerated = (BlueprintEffect_Generated) target2;
      blueprintEffectGenerated.name = this.name;
      blueprintEffectGenerated.queue = this.queue;
      blueprintEffectGenerated.blueprint = this.blueprint;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<GameObject>(writer, "Blueprint", this.blueprint);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.blueprint = UnityDataReadUtility.Read<GameObject>(reader, "Blueprint", this.blueprint);
    }
  }
}
