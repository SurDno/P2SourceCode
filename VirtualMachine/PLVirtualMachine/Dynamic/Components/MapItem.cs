using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMMapItem))]
  public class MapItem : VMMapItem, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "MapItemComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      if (((IBlueprint) templateObject).TryGetProperty("MapItemComponent.Title", out IParam obj1))
        Title = (ITextRef) obj1.Value;
      if (((IBlueprint) templateObject).TryGetProperty("MapItemComponent.Text", out IParam obj2))
        Text = (ITextRef) obj2.Value;
      if (!((IBlueprint) templateObject).TryGetProperty("MapItemComponent.TooltipText", out IParam obj3))
        return;
      TooltipText = (ITextRef) obj3.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
