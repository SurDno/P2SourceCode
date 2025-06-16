using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMRegion))]
  public class Region : VMRegion, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Region);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj;
      if (!((IBlueprint) templateObject).TryGetProperty("Region.RegionIndex", out obj))
        return;
      RegionIndex = (int) obj.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "ReputationChanged":
          ReputationChanged += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "DiseaseLevelChanged":
          DiseaseLevelChanged += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
