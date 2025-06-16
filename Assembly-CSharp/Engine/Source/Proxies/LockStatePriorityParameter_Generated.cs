// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.LockStatePriorityParameter_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LockStatePriorityParameter))]
  public class LockStatePriorityParameter_Generated : 
    LockStatePriorityParameter,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      LockStatePriorityParameter_Generated instance = Activator.CreateInstance<LockStatePriorityParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LockStatePriorityParameter_Generated parameterGenerated = (LockStatePriorityParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.container = CloneableObjectUtility.Clone<PriorityContainer<LockState>>(this.container);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteSerialize<PriorityContainer<LockState>>(writer, "Container", this.container);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.container = DefaultDataReadUtility.ReadSerialize<PriorityContainer<LockState>>(reader, "Container");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultStateSaveUtility.SaveSerialize<PriorityContainer<LockState>>(writer, "Container", this.container);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.container = DefaultStateLoadUtility.ReadSerialize<PriorityContainer<LockState>>(reader, "Container");
    }
  }
}
