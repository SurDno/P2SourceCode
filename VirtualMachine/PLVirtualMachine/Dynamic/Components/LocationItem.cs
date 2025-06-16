using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMLocationItem))]
  public class LocationItem : VMLocationItem, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (LocationItem);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnChangeHibernation":
          this.OnChangeHibernation += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "OnChangeLocation":
          this.OnChangeLocation += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
