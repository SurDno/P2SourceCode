﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ResetableDetectTypeParameter))]
  public class ResetableDetectTypeParameter_Generated : 
    ResetableDetectTypeParameter,
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
      ResetableDetectTypeParameter_Generated parameterGenerated = (ResetableDetectTypeParameter_Generated) target2;
      if (parameterGenerated.name != name || parameterGenerated.value != value || parameterGenerated.baseValue != baseValue)
        return;
      NeedSave = false;
    }

    public object Clone()
    {
      ResetableDetectTypeParameter_Generated instance = Activator.CreateInstance<ResetableDetectTypeParameter_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ResetableDetectTypeParameter_Generated parameterGenerated = (ResetableDetectTypeParameter_Generated) target2;
      parameterGenerated.name = name;
      parameterGenerated.value = value;
      parameterGenerated.baseValue = baseValue;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
      DefaultDataWriteUtility.WriteEnum(writer, "BaseValue", baseValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
      value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
      baseValue = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "BaseValue");
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
      DefaultDataWriteUtility.WriteEnum(writer, "BaseValue", baseValue);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
      baseValue = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "BaseValue");
    }
  }
}
