using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

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
      TravelToPoint += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
    }
  }
}
