using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBlueprintComponent))]
  public class BlueprintComponent : 
    VMBlueprintComponent,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (BlueprintComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "Complete":
          this.Complete += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "Attach":
          this.Attach += (Action) (() => target.RaiseFromEngineImpl());
          break;
      }
    }
  }
}
