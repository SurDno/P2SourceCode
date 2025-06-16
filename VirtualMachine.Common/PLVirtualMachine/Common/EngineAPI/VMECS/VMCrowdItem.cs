// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMCrowdItem
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("CrowdItemComponent", typeof (ICrowdItemComponent))]
  public class VMCrowdItem : VMEngineComponent<ICrowdItemComponent>
  {
    public const string ComponentName = "CrowdItemComponent";

    [Property("Area", "")]
    public AreaEnum Area => this.Component.Area;
  }
}
