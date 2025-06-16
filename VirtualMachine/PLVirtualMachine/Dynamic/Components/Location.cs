using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMLocation))]
  public class Location : VMLocation, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Location);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnHibernationChange":
          this.OnHibernationChange += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "OnPlayerInside":
          this.OnPlayerInside += (Action<bool>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
