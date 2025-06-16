using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBehavior))]
  public class Behavior : VMBehavior, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "BehaviorComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "Success":
          Success += () => target.RaiseFromEngineImpl();
          break;
        case "Fail":
          Fail += () => target.RaiseFromEngineImpl();
          break;
        case "Custom":
          Custom += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
