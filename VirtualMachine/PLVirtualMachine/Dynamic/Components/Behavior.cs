using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBehavior))]
  public class Behavior : VMBehavior, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "BehaviorComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "Success":
          this.Success += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "Fail":
          this.Fail += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "Custom":
          this.Custom += (Action<string>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
