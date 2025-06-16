using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

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
          ObjectEnterEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "ObjectExitEvent":
          ObjectExitEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
