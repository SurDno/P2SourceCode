using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Values;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectAbilityFloatValue))]
  public class EffectAbilityFloatValue_Generated : 
    EffectAbilityFloatValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectAbilityFloatValue_Generated instance = Activator.CreateInstance<EffectAbilityFloatValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((EffectAbilityValue<float>) target2).valueName = this.valueName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<AbilityValueNameEnum>(writer, "AbilityValueName", this.valueName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.valueName = DefaultDataReadUtility.ReadEnum<AbilityValueNameEnum>(reader, "AbilityValueName");
    }
  }
}
