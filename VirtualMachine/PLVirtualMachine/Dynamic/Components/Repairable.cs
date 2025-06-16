using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMRepairable))]
  public class Repairable : VMRepairable, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "RepairableComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "OnChangeDurability"))
        return;
      this.OnChangeDurability += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
    }
  }
}
