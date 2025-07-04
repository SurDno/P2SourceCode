﻿using System;
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
  [FactoryProxy(typeof (ResetableBoolParameter))]
  public class ResetableBoolParameter_Generated : 
    ResetableBoolParameter,
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
      ResetableBoolParameter_Generated parameterGenerated = (ResetableBoolParameter_Generated) target2;
      if (parameterGenerated.name != name || parameterGenerated.value != value || parameterGenerated.baseValue != baseValue)
        return;
      NeedSave = false;
    }

    public object Clone()
    {
      ResetableBoolParameter_Generated instance = Activator.CreateInstance<ResetableBoolParameter_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ResetableBoolParameter_Generated parameterGenerated = (ResetableBoolParameter_Generated) target2;
      parameterGenerated.name = name;
      parameterGenerated.value = value;
      parameterGenerated.baseValue = baseValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.Write(writer, "Value", value);
      DefaultDataWriteUtility.Write(writer, "BaseValue", baseValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      value = DefaultDataReadUtility.Read(reader, "Value", value);
      baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", baseValue);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.Write(writer, "Value", value);
      DefaultDataWriteUtility.Write(writer, "BaseValue", baseValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.Read(reader, "Value", value);
      baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", baseValue);
    }
  }
}
