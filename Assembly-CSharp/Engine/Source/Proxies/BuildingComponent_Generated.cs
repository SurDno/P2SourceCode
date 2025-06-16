using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Source.Components;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((BuildingComponent_Generated) target2).building = building;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Building", building);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      building = DefaultDataReadUtility.ReadEnum<BuildingEnum>(reader, "Building");
    }
  }
}
