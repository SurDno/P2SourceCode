// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.BoolPriorityItem_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PriorityItem<bool>))]
  public class BoolPriorityItem_Generated : 
    BoolPriorityItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      BoolPriorityItem_Generated instance = Activator.CreateInstance<BoolPriorityItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BoolPriorityItem_Generated priorityItemGenerated = (BoolPriorityItem_Generated) target2;
      priorityItemGenerated.Priority = this.Priority;
      priorityItemGenerated.Value = this.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      DefaultDataWriteUtility.Write(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = DefaultDataReadUtility.Read(reader, "Value", this.Value);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<PriorityParameterEnum>(writer, "Priority", this.Priority);
      DefaultDataWriteUtility.Write(writer, "Value", this.Value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Priority = DefaultDataReadUtility.ReadEnum<PriorityParameterEnum>(reader, "Priority");
      this.Value = DefaultDataReadUtility.Read(reader, "Value", this.Value);
    }
  }
}
