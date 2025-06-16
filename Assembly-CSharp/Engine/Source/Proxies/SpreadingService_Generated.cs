using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Components;
using Engine.Source.Components;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpreadingService))]
  public class SpreadingService_Generated : 
    SpreadingService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveListReferences<SpreadingComponent>(writer, "SpreadingComponents", this.spreadingComponents);
      CustomStateSaveUtility.SaveListReferences<IRegionComponent>(writer, "RegionComponents", this.regionComponents);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.spreadingComponents = CustomStateLoadUtility.LoadListReferences<SpreadingComponent>(reader, "SpreadingComponents", this.spreadingComponents);
      this.regionComponents = CustomStateLoadUtility.LoadListReferences<IRegionComponent>(reader, "RegionComponents", this.regionComponents);
    }
  }
}
