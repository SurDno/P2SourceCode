using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo(((AbilitiesComponent) target2).resourceAbilities, resourceAbilities);
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.WriteList(writer, "Abilities", resourceAbilities);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      resourceAbilities = UnityDataReadUtility.ReadList(reader, "Abilities", resourceAbilities);
    }

    public void StateSave(IDataWriter writer)
    {
    }

    public void StateLoad(IDataReader reader, Type type)
    {
    }
  }
}
