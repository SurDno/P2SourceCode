using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMInteractable))]
  public class Interactable : VMInteractable, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "Interactive";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      if (!((IBlueprint) templateObject).TryGetProperty("Interactive.ObjectName", out IParam obj))
        return;
      ObjectName = (ITextRef) obj.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "BeginIteractEvent":
          BeginIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
          break;
        case "EndIteractEvent":
          EndIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
          break;
      }
    }
  }
}
