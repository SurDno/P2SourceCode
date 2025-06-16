using Cofe.Proxies;
using Engine.Common.Commons;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBoundCharacterComponent))]
  public class BoundCharacterComponent : 
    VMBoundCharacterComponent,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (BoundCharacterComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "OnChangeBoundHealthState"))
        return;
      this.OnChangeBoundHealthState += (Action<BoundHealthStateEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
    }
  }
}
