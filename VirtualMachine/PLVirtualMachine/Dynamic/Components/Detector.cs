using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMDetector))]
  public class Detector : VMDetector, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Detector);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnSee":
          OnSee += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnStopSee":
          OnStopSee += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnHear":
          OnHear += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
