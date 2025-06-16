using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMCommon))]
  public class Common : VMCommon, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Common);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "StartEvent":
          this.StartEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "RemoveEvent":
          this.RemoveEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
      }
    }
  }
}
