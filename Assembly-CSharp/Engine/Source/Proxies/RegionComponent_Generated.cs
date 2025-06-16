using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RegionComponent))]
  public class RegionComponent_Generated : 
    RegionComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RegionComponent_Generated instance = Activator.CreateInstance<RegionComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RegionComponent_Generated componentGenerated = (RegionComponent_Generated) target2;
      componentGenerated.region = this.region;
      componentGenerated.regionBehaviour = this.regionBehaviour;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<RegionEnum>(writer, "Region", this.region);
      DefaultDataWriteUtility.WriteEnum<RegionBehaviourEnum>(writer, "RegionBehavior", this.regionBehaviour);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.region = DefaultDataReadUtility.ReadEnum<RegionEnum>(reader, "Region");
      this.regionBehaviour = DefaultDataReadUtility.ReadEnum<RegionBehaviourEnum>(reader, "RegionBehavior");
    }
  }
}
