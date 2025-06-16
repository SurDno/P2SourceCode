using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
