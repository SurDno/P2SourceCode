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
      IParam obj1;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.StorableClass", out obj1))
        StorableClass = (string) obj1.Value;
      IParam obj2;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.StoreTag", out obj2))
        StoreTag = (string) obj2.Value;
      IParam obj3;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.DefaultStackCount", out obj3))
        DefaultStackCount = (int) obj3.Value;
      IParam obj4;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Title", out obj4))
        Title = (ITextRef) obj4.Value;
      IParam obj5;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Tooltip", out obj5))
        Tooltip = (ITextRef) obj5.Value;
      IParam obj6;
      if (((IBlueprint) templateObject).TryGetProperty("Storable.Description", out obj6))
        Description = (ITextRef) obj6.Value;
      IParam obj7;
      if (!((IBlueprint) templateObject).TryGetProperty("Storable.SpecialDescription", out obj7))
        return;
      SpecialDescription = (ITextRef) obj7.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
