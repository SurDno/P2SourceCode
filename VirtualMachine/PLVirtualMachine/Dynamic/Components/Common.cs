using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMCommon))]
  public class Common : VMCommon, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Common);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "StartEvent":
          StartEvent += () => target.RaiseFromEngineImpl();
          break;
        case "RemoveEvent":
          RemoveEvent += () => target.RaiseFromEngineImpl();
          break;
      }
    }
  }
}
