// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMMessangerComponent
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("MessangerComponent", typeof (IMessangerComponent))]
  public class VMMessangerComponent : VMEngineComponent<IMessangerComponent>
  {
    public const string ComponentName = "MessangerComponent";

    [Method("Start teleporting", "", "")]
    public void StartTeleporting() => this.Component.StartTeleporting();

    [Method("Stop teleporting", "", "")]
    public void StopTeleporting() => this.Component.StopTeleporting();

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      base.Clear();
    }

    protected override void Init()
    {
      int num = this.IsTemplate ? 1 : 0;
    }
  }
}
