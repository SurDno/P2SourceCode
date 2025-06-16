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
  [FactoryProxy(typeof (TimeSpanParameter))]
  public class TimeSpanParameter_Generated : 
    TimeSpanParameter,
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
      TimeSpanParameter_Generated parameterGenerated = (TimeSpanParameter_Generated) target2;
      if (parameterGenerated.name != this.name || parameterGenerated.value != this.value || parameterGenerated.minValue != this.minValue || parameterGenerated.maxValue != this.maxValue)
        return;
      this.NeedSave = false;
    }

    public object Clone()
    {
      TimeSpanParameter_Generated instance = Activator.CreateInstance<TimeSpanParameter_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      TimeSpanParameter_Generated parameterGenerated = (TimeSpanParameter_Generated) target2;
      parameterGenerated.name = this.name;
      parameterGenerated.value = this.value;
      parameterGenerated.minValue = this.minValue;
      parameterGenerated.maxValue = this.maxValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
      DefaultDataWriteUtility.Write(writer, "MinValue", this.minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", this.maxValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
      this.minValue = DefaultDataReadUtility.Read(reader, "MinValue", this.minValue);
      this.maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", this.maxValue);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
      DefaultDataWriteUtility.Write(writer, "MinValue", this.minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", this.maxValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
      this.minValue = DefaultDataReadUtility.Read(reader, "MinValue", this.minValue);
      this.maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", this.maxValue);
    }
  }
}
