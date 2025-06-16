// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AbilityValueInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Effects.Values;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AbilityValueInfo))]
  public class AbilityValueInfo_Generated : 
    AbilityValueInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AbilityValueInfo_Generated instance = Activator.CreateInstance<AbilityValueInfo_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AbilityValueInfo_Generated valueInfoGenerated = (AbilityValueInfo_Generated) target2;
      valueInfoGenerated.Name = this.Name;
      valueInfoGenerated.Value = CloneableObjectUtility.Clone<IAbilityValue>(this.Value);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<AbilityValueNameEnum>(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteSerialize<IAbilityValue>(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.ReadEnum<AbilityValueNameEnum>(reader, "Name");
      this.Value = DefaultDataReadUtility.ReadSerialize<IAbilityValue>(reader, "Value");
    }
  }
}
