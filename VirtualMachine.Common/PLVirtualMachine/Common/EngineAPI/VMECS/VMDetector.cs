using System;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Detector", typeof (IDetectorComponent))]
  public class VMDetector : VMEngineComponent<IDetectorComponent>
  {
    public const string ComponentName = "Detector";

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.OnSee -= FireOnSee;
      Component.OnStopSee -= FireOnStopSee;
      Component.OnHear -= FireHear;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.OnSee += FireOnSee;
      Component.OnStopSee += FireOnStopSee;
      Component.OnHear += FireHear;
    }

    private void FireOnSee(IDetectableComponent target)
    {
      Action<IEntity> onSee = OnSee;
      if (onSee == null)
        return;
      onSee(target.Owner);
    }

    private void FireOnStopSee(IDetectableComponent target)
    {
      Action<IEntity> onStopSee = OnStopSee;
      if (onStopSee == null)
        return;
      onStopSee(target.Owner);
    }

    private void FireHear(IDetectableComponent target)
    {
      Action<IEntity> onHear = OnHear;
      if (onHear == null)
        return;
      onHear(target.Owner);
    }

    [Event("OnSee", "detected object:Detectable")]
    public event Action<IEntity> OnSee;

    [Event("OnStopSee", "detected object:Detectable")]
    public event Action<IEntity> OnStopSee;

    [Event("OnHear", "detected object:Detectable")]
    public event Action<IEntity> OnHear;

    [Property("Enabled", "", false, true, false)]
    public bool DetectorEnabled
    {
      get => Component.IsEnabled;
      set => Component.IsEnabled = value;
    }
  }
}
