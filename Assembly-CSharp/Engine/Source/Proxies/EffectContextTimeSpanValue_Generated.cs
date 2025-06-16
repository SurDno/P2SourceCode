using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextTimeSpanValue))]
  public class EffectContextTimeSpanValue_Generated : 
    EffectContextTimeSpanValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextTimeSpanValue_Generated instance = Activator.CreateInstance<EffectContextTimeSpanValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextTimeSpanValue_Generated spanValueGenerated = (EffectContextTimeSpanValue_Generated) target2;
      spanValueGenerated.effectContext = effectContext;
      spanValueGenerated.parameterName = parameterName;
      spanValueGenerated.parameterData = parameterData;
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
