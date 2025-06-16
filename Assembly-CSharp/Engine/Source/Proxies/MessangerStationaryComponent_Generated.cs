using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.MessangerStationary;
using Engine.Source.Components;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((MessangerStationaryComponent_Generated) target2).spawnpointKindEnum = spawnpointKindEnum;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "SpawnpointKindEnum", spawnpointKindEnum);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      spawnpointKindEnum = DefaultDataReadUtility.ReadEnum<SpawnpointKindEnum>(reader, "SpawnpointKindEnum");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "SpawnpointKindEnum", spawnpointKindEnum);
      DefaultDataWriteUtility.Write(writer, "Registred", registred);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      spawnpointKindEnum = DefaultDataReadUtility.ReadEnum<SpawnpointKindEnum>(reader, "SpawnpointKindEnum");
      registred = DefaultDataReadUtility.Read(reader, "Registred", registred);
    }
  }
}
