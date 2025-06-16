// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.DoorComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DoorComponent_Generated componentGenerated = (DoorComponent_Generated) target2;
      componentGenerated.picklocks = CloneableObjectUtility.Clone<PriorityContainer<List<Typed<IEntity>>>>(this.picklocks);
      componentGenerated.keys = CloneableObjectUtility.Clone<PriorityContainer<List<Typed<IEntity>>>>(this.keys);
      componentGenerated.isOutdoor = this.isOutdoor;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<PriorityContainer<List<Typed<IEntity>>>>(writer, "Picklocks", this.picklocks);
      DefaultDataWriteUtility.WriteSerialize<PriorityContainer<List<Typed<IEntity>>>>(writer, "Keys", this.keys);
      DefaultDataWriteUtility.Write(writer, "IsOutdoor", this.isOutdoor);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.picklocks = DefaultDataReadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Picklocks");
      this.keys = DefaultDataReadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Keys");
      this.isOutdoor = DefaultDataReadUtility.Read(reader, "IsOutdoor", this.isOutdoor);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MinReputation", this.MinReputation);
      DefaultDataWriteUtility.Write(writer, "MaxReputation", this.MaxReputation);
      DefaultStateSaveUtility.SaveSerialize<PriorityContainer<List<Typed<IEntity>>>>(writer, "Picklocks", this.picklocks);
      DefaultStateSaveUtility.SaveSerialize<PriorityContainer<List<Typed<IEntity>>>>(writer, "Keys", this.keys);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.MinReputation = DefaultDataReadUtility.Read(reader, "MinReputation", this.MinReputation);
      this.MaxReputation = DefaultDataReadUtility.Read(reader, "MaxReputation", this.MaxReputation);
      this.picklocks = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Picklocks");
      this.keys = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<List<Typed<IEntity>>>>(reader, "Keys");
    }
  }
}
