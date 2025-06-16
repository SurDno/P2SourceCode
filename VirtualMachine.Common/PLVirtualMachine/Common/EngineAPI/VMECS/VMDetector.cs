// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMDetector
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Detector", typeof (IDetectorComponent))]
  public class VMDetector : VMEngineComponent<IDetectorComponent>
  {
    public const string ComponentName = "Detector";

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnSee -= new Action<IDetectableComponent>(this.FireOnSee);
      this.Component.OnStopSee -= new Action<IDetectableComponent>(this.FireOnStopSee);
      this.Component.OnHear -= new Action<IDetectableComponent>(this.FireHear);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnSee += new Action<IDetectableComponent>(this.FireOnSee);
      this.Component.OnStopSee += new Action<IDetectableComponent>(this.FireOnStopSee);
      this.Component.OnHear += new Action<IDetectableComponent>(this.FireHear);
    }

    private void FireOnSee(IDetectableComponent target)
    {
      Action<IEntity> onSee = this.OnSee;
      if (onSee == null)
        return;
      onSee(target.Owner);
    }

    private void FireOnStopSee(IDetectableComponent target)
    {
      Action<IEntity> onStopSee = this.OnStopSee;
      if (onStopSee == null)
        return;
      onStopSee(target.Owner);
    }

    private void FireHear(IDetectableComponent target)
    {
      Action<IEntity> onHear = this.OnHear;
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
      get => this.Component.IsEnabled;
      set => this.Component.IsEnabled = value;
    }
  }
}
