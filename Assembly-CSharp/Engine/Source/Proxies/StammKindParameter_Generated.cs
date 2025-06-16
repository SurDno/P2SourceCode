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
  [FactoryProxy(typeof (StammKindParameter))]
  public class StammKindParameter_Generated : 
    StammKindParameter,
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
      StammKindParameter_Generated parameterGenerated = (StammKindParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      StammKindParameter_Generated instance = Activator.CreateInstance<StammKindParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StammKindParameter_Generated parameterGenerated = (StammKindParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<StammKind>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.ReadEnum<StammKind>(reader, "Value");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<StammKind>(writer, "Value", this.value);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<StammKind>(reader, "Value");
    }
  }
}
