// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ListTypedEntityPriorityContainer_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityContainer<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityContainer_Generated : 
    ListTypedEntityPriorityContainer,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      ListTypedEntityPriorityContainer_Generated instance = Activator.CreateInstance<ListTypedEntityPriorityContainer_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<PriorityItem<List<Typed<IEntity>>>>(((PriorityContainer<List<Typed<IEntity>>>) target2).items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<PriorityItem<List<Typed<IEntity>>>>(writer, "Items", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.items = DefaultDataReadUtility.ReadListSerialize<PriorityItem<List<Typed<IEntity>>>>(reader, "Items", this.items);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize<PriorityItem<List<Typed<IEntity>>>>(writer, "Items", this.items);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.items = DefaultStateLoadUtility.ReadListSerialize<PriorityItem<List<Typed<IEntity>>>>(reader, "Items", this.items);
    }
  }
}
