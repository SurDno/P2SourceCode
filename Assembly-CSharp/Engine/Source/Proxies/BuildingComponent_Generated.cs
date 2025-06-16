using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BuildingComponent))]
  public class BuildingComponent_Generated : 
    BuildingComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BuildingComponent_Generated instance = Activator.CreateInstance<BuildingComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((BuildingComponent) target2).building = this.building;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<BuildingEnum>(writer, "Building", this.building);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.building = DefaultDataReadUtility.ReadEnum<BuildingEnum>(reader, "Building");
    }
  }
}
