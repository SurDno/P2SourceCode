using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Values;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectAbilityBoolValue))]
  public class EffectAbilityBoolValue_Generated : 
    EffectAbilityBoolValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectAbilityBoolValue_Generated instance = Activator.CreateInstance<EffectAbilityBoolValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((EffectAbilityValue<bool>) target2).valueName = valueName;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "AbilityValueName", valueName);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      valueName = DefaultDataReadUtility.ReadEnum<AbilityValueNameEnum>(reader, "AbilityValueName");
    }
  }
}
