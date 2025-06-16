using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AbilitiesComponent))]
  public class AbilitiesComponent_Generated : 
    AbilitiesComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      AbilitiesComponent_Generated instance = Activator.CreateInstance<AbilitiesComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo<Typed<IAbility>>(((AbilitiesComponent) target2).resourceAbilities, this.resourceAbilities);
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.WriteList<IAbility>(writer, "Abilities", this.resourceAbilities);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.resourceAbilities = UnityDataReadUtility.ReadList<IAbility>(reader, "Abilities", this.resourceAbilities);
    }

    public void StateSave(IDataWriter writer)
    {
    }

    public void StateLoad(IDataReader reader, Type type)
    {
    }
  }
}
