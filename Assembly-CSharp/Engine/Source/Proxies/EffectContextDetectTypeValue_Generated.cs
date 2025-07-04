﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextDetectTypeValue))]
  public class EffectContextDetectTypeValue_Generated : 
    EffectContextDetectTypeValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextDetectTypeValue_Generated instance = Activator.CreateInstance<EffectContextDetectTypeValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextDetectTypeValue_Generated typeValueGenerated = (EffectContextDetectTypeValue_Generated) target2;
      typeValueGenerated.effectContext = effectContext;
      typeValueGenerated.parameterName = parameterName;
      typeValueGenerated.parameterData = parameterData;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "EffectContext", effectContext);
      DefaultDataWriteUtility.WriteEnum(writer, "ParameterName", parameterName);
      DefaultDataWriteUtility.WriteEnum(writer, "ParameterData", parameterData);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      effectContext = DefaultDataReadUtility.ReadEnum<EffectContextEnum>(reader, "EffectContext");
      parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ParameterName");
      parameterData = DefaultDataReadUtility.ReadEnum<ParameterDataEnum>(reader, "ParameterData");
    }
  }
}
