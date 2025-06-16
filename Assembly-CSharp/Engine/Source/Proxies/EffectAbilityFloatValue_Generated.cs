// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.EffectAbilityFloatValue_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Values;
using System;

#nullable disable
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
