using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextBoolValue))]
  public class EffectContextBoolValue_Generated : 
    EffectContextBoolValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextBoolValue_Generated instance = Activator.CreateInstance<EffectContextBoolValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextBoolValue_Generated boolValueGenerated = (EffectContextBoolValue_Generated) target2;
      boolValueGenerated.effectContext = effectContext;
      boolValueGenerated.parameterName = parameterName;
      boolValueGenerated.parameterData = parameterData;
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
