using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMLipSync))]
  public class LipSync : VMLipSync, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (LipSync);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "PlayCompleteEvent"))
        return;
      this.PlayCompleteEvent += (Action) (() => target.RaiseFromEngineImpl());
    }
  }
}
