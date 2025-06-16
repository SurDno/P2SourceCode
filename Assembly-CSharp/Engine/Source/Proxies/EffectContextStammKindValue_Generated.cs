using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextStammKindValue))]
  public class EffectContextStammKindValue_Generated : 
    EffectContextStammKindValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextStammKindValue_Generated instance = Activator.CreateInstance<EffectContextStammKindValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextStammKindValue_Generated kindValueGenerated = (EffectContextStammKindValue_Generated) target2;
      kindValueGenerated.effectContext = effectContext;
      kindValueGenerated.parameterName = parameterName;
      kindValueGenerated.parameterData = parameterData;
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
