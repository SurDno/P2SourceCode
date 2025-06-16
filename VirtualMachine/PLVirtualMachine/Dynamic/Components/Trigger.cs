using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMTrigger))]
  public class Trigger : VMTrigger, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Trigger);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "ObjectEnterEvent":
          this.ObjectEnterEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ObjectExitEvent":
          this.ObjectExitEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
