using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

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
          this.LeaveRegionEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedRegionEvent":
          this.ArrivedRegionEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "LeaveAreaEvent":
          this.LeaveAreaEvent += (Action<AreaEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedAreaEvent":
          this.ArrivedAreaEvent += (Action<AreaEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "LeaveBuildingEvent":
          this.LeaveBuildingEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedBuildingEvent":
          this.ArrivedBuildingEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
