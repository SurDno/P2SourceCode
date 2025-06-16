using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMCrowdItem))]
  public class CrowdItem : VMCrowdItem, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "CrowdItemComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
