using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Effects.Engine;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextFloatValue))]
  public class EffectContextFloatValue_Generated : 
    EffectContextFloatValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextFloatValue_Generated instance = Activator.CreateInstance<EffectContextFloatValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextFloatValue_Generated floatValueGenerated = (EffectContextFloatValue_Generated) target2;
      floatValueGenerated.effectContext = this.effectContext;
      floatValueGenerated.parameterName = this.parameterName;
      floatValueGenerated.parameterData = this.parameterData;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<EffectContextEnum>(writer, "EffectContext", this.effectContext);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ParameterName", this.parameterName);
      DefaultDataWriteUtility.WriteEnum<ParameterDataEnum>(writer, "ParameterData", this.parameterData);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.effectContext = DefaultDataReadUtility.ReadEnum<EffectContextEnum>(reader, "EffectContext");
      this.parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ParameterName");
      this.parameterData = DefaultDataReadUtility.ReadEnum<ParameterDataEnum>(reader, "ParameterData");
    }
  }
}
