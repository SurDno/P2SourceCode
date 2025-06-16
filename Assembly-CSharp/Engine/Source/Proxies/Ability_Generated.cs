// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Ability_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons.Abilities;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Ability))]
  public class Ability_Generated : 
    Ability,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone() => (object) ServiceCache.Factory.Instantiate<Ability_Generated>(this);

    public void CopyTo(object target2)
    {
      Ability_Generated abilityGenerated = (Ability_Generated) target2;
      abilityGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<AbilityItem>(abilityGenerated.items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<AbilityItem>(writer, "AbilityItems", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.items = DefaultDataReadUtility.ReadListSerialize<AbilityItem>(reader, "AbilityItems", this.items);
    }
  }
}
