using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BlueprintComponent))]
  public class BlueprintComponent_Generated : 
    BlueprintComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BlueprintComponent_Generated instance = Activator.CreateInstance<BlueprintComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BlueprintComponent_Generated componentGenerated = (BlueprintComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      componentGenerated.blueprintResource = this.blueprintResource;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<GameObject>(writer, "Blueprint", this.blueprintResource);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.blueprintResource = UnityDataReadUtility.Read<GameObject>(reader, "Blueprint", this.blueprintResource);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<GameObject>(writer, "BlueprintResource", this.blueprintResource);
    }

    public void StateLoad(IDataReader reader, System.Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.blueprintResource = UnityDataReadUtility.Read<GameObject>(reader, "BlueprintResource", this.blueprintResource);
    }
  }
}
