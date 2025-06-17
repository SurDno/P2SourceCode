using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMStorable))]
  public class Storable : VMStorable, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Storable);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      if (((IBlueprint) templateObject).TryGetProperty("Storable.StorableClass", out IParam obj1))
        StorableClass = (string) obj1.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.StoreTag", out IParam obj2))
        StoreTag = (string) obj2.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.DefaultStackCount", out IParam obj3))
        DefaultStackCount = (int) obj3.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Title", out IParam obj4))
        Title = (ITextRef) obj4.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Tooltip", out IParam obj5))
        Tooltip = (ITextRef) obj5.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Description", out IParam obj6))
        Description = (ITextRef) obj6.Value;
      if (!((IBlueprint) templateObject).TryGetProperty("Storable.SpecialDescription", out IParam obj7))
        return;
      SpecialDescription = (ITextRef) obj7.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
