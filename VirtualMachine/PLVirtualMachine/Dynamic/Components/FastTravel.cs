using Cofe.Proxies;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMFastTravel))]
  public class FastTravel : VMFastTravel, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "FastTravelComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "TravelToPoint"))
        return;
      this.TravelToPoint += (Action<FastTravelPointEnum, GameTime>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
    }
  }
}
