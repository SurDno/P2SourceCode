using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components;
using Engine.Source.Connections;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DoorComponent))]
  public class DoorComponent_Generated : 
    DoorComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      DoorComponent_Generated instance = Activator.CreateInstance<DoorComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      DoorComponent_Generated componentGenerated = (DoorComponent_Generated) target2;
      componentGenerated.picklocks = CloneableObjectUtility.Clone(picklocks);
      componentGenerated.keys = CloneableObjectUtility.Clone(keys);
      componentGenerated.isOutdoor = isOutdoor;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "Picklocks", picklocks);
      DefaultDataWriteUtility.WriteSerialize(writer, "Keys", keys);
      DefaultDataWriteUtility.Write(writer, "IsOutdoor", isOutdoor);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      picklocks = DefaultDataReadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Picklocks");
      keys = DefaultDataReadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Keys");
      isOutdoor = DefaultDataReadUtility.Read(reader, "IsOutdoor", isOutdoor);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MinReputation", MinReputation);
      DefaultDataWriteUtility.Write(writer, "MaxReputation", MaxReputation);
      DefaultStateSaveUtility.SaveSerialize(writer, "Picklocks", picklocks);
      DefaultStateSaveUtility.SaveSerialize(writer, "Keys", keys);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      MinReputation = DefaultDataReadUtility.Read(reader, "MinReputation", MinReputation);
      MaxReputation = DefaultDataReadUtility.Read(reader, "MaxReputation", MaxReputation);
      picklocks = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Picklocks");
      keys = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Keys");
    }
  }
}
