using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.MessangerStationary;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MessangerStationaryComponent))]
  public class MessangerStationaryComponent_Generated : 
    MessangerStationaryComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      MessangerStationaryComponent_Generated instance = Activator.CreateInstance<MessangerStationaryComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((MessangerStationaryComponent) target2).spawnpointKindEnum = this.spawnpointKindEnum;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<SpawnpointKindEnum>(writer, "SpawnpointKindEnum", this.spawnpointKindEnum);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.spawnpointKindEnum = DefaultDataReadUtility.ReadEnum<SpawnpointKindEnum>(reader, "SpawnpointKindEnum");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<SpawnpointKindEnum>(writer, "SpawnpointKindEnum", this.spawnpointKindEnum);
      DefaultDataWriteUtility.Write(writer, "Registred", this.registred);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.spawnpointKindEnum = DefaultDataReadUtility.ReadEnum<SpawnpointKindEnum>(reader, "SpawnpointKindEnum");
      this.registred = DefaultDataReadUtility.Read(reader, "Registred", this.registred);
    }
  }
}
