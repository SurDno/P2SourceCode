using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BoundHealthStateParameter))]
  public class BoundHealthStateParameter_Generated : 
    BoundHealthStateParameter,
    IComputeNeedSave,
    INeedSave,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public bool NeedSave { get; private set; } = true;

    public void ComputeNeedSave(object target2)
    {
      this.NeedSave = true;
      BoundHealthStateParameter_Generated parameterGenerated = (BoundHealthStateParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      BoundHealthStateParameter_Generated instance = Activator.CreateInstance<BoundHealthStateParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BoundHealthStateParameter_Generated parameterGenerated = (BoundHealthStateParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<BoundHealthStateEnum>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.ReadEnum<BoundHealthStateEnum>(reader, "Value");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<BoundHealthStateEnum>(writer, "Value", this.value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<BoundHealthStateEnum>(reader, "Value");
    }
  }
}
