// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ListTypedEntityPriorityItem_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityItem<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityItem_Generated : 
    ListTypedEntityPriorityItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      ListTypedEntityPriorityItem_Generated instance = Activator.CreateInstance<ListTypedEntityPriorityItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ListTypedEntityPriorityItem_Generated priorityItemGenerated = (ListTypedEntityPriorityItem_Generated) target2;
      priorityItemGenerated.Priority = this.Priority;
      CloneableObjectUtility.FillListTo<Typed<IEntity>>(priorityItemGenerated.Value, this.Value);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      UnityDataWriteUtility.WriteList<IEntity>(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = UnityDataReadUtility.ReadList<IEntity>(reader, "Value", this.Value);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      UnityDataWriteUtility.WriteList<IEntity>(writer, "Value", this.Value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = UnityDataReadUtility.ReadList<IEntity>(reader, "Value", this.Value);
    }
  }
}
