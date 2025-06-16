// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMInventory
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components;
using Engine.Common.Components.Storable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Inventory", typeof (IInventoryComponent))]
  public class VMInventory : VMEngineComponent<IInventoryComponent>
  {
    public const string ComponentName = "Inventory";

    [Property("Available", "", false, true, false)]
    public bool Available
    {
      get => this.Component.Available.Value;
      set => this.Component.Available.Value = value;
    }

    [Property("Enabled", "", false, true, false)]
    public bool Enabled
    {
      get => this.Component.Enabled.Value;
      set => this.Component.Enabled.Value = value;
    }

    [Property("Open state", "", false, ContainerOpenStateEnum.Open, false)]
    public ContainerOpenStateEnum OpenState
    {
      get => this.Component.OpenState.Value;
      set => this.Component.OpenState.Value = value;
    }

    [Property("Disease", "", false, 0.0f, false)]
    public float Disease
    {
      get => this.Component.Disease.Value;
      set => this.Component.Disease.Value = value;
    }
  }
}
