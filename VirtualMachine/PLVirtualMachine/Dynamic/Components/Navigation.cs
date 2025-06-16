using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMNavigation))]
  public class Navigation : VMNavigation, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "Position";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "LeaveRegionEvent":
          LeaveRegionEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "ArrivedRegionEvent":
          ArrivedRegionEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "LeaveAreaEvent":
          LeaveAreaEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "ArrivedAreaEvent":
          ArrivedAreaEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "LeaveBuildingEvent":
          LeaveBuildingEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "ArrivedBuildingEvent":
          ArrivedBuildingEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
