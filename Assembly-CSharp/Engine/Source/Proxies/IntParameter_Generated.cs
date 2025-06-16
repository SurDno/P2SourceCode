using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (IntParameter))]
  public class IntParameter_Generated : 
    IntParameter,
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
      NeedSave = true;
      IntParameter_Generated parameterGenerated = (IntParameter_Generated) target2;
      if (parameterGenerated.name != name || parameterGenerated.value != value || parameterGenerated.minValue != minValue || parameterGenerated.maxValue != maxValue)
        return;
      NeedSave = false;
    }

    public object Clone()
    {
      IntParameter_Generated instance = Activator.CreateInstance<IntParameter_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      IntParameter_Generated parameterGenerated = (IntParameter_Generated) target2;
      parameterGenerated.name = name;
      parameterGenerated.value = value;
      parameterGenerated.minValue = minValue;
      parameterGenerated.maxValue = maxValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.Write(writer, "Value", value);
      DefaultDataWriteUtility.Write(writer, "MinValue", minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", maxValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      value = DefaultDataReadUtility.Read(reader, "Value", value);
      minValue = DefaultDataReadUtility.Read(reader, "MinValue", minValue);
      maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", maxValue);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.Write(writer, "Value", value);
      DefaultDataWriteUtility.Write(writer, "MinValue", minValue);
      DefaultDataWriteUtility.Write(writer, "MaxValue", maxValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.Read(reader, "Value", value);
      minValue = DefaultDataReadUtility.Read(reader, "MinValue", minValue);
      maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", maxValue);
    }
  }
}
