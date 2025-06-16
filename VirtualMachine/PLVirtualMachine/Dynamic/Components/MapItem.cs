// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.MapItem
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMMapItem))]
  public class MapItem : VMMapItem, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "MapItemComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj1;
      if (((IBlueprint) templateObject).TryGetProperty("MapItemComponent.Title", out obj1))
        this.Title = (ITextRef) obj1.Value;
      IParam obj2;
      if (((IBlueprint) templateObject).TryGetProperty("MapItemComponent.Text", out obj2))
        this.Text = (ITextRef) obj2.Value;
      IParam obj3;
      if (!((IBlueprint) templateObject).TryGetProperty("MapItemComponent.TooltipText", out obj3))
        return;
      this.TooltipText = (ITextRef) obj3.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
