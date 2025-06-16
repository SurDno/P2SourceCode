using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMDetector))]
  public class Detector : VMDetector, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Detector);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnSee":
          this.OnSee += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnStopSee":
          this.OnStopSee += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnHear":
          this.OnHear += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
