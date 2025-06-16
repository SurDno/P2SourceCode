// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Common.EngineAPI.VMECS.VMComponents.VMHerbRoots
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace VirtualMachine.Common.EngineAPI.VMECS.VMComponents
{
  [Info("HerbRootsComponent", typeof (IHerbRootsComponent))]
  public class VMHerbRoots : VMEngineComponent<IHerbRootsComponent>
  {
    public const string ComponentName = "HerbRootsComponent";

    [Event("Player enters trigger event", "")]
    public event Action TriggerEnterEvent;

    [Event("Player leaves trigger event", "")]
    public event Action TriggerLeaveEvent;

    [Event("Activation start event", "")]
    public event Action ActivateStartEvent;

    [Event("Activation end event", "")]
    public event Action ActivateEndEvent;

    [Event("Herb spawn event", "")]
    public event Action HerbSpawnEvent;

    [Event("Last herb spawn event", "")]
    public event Action LastHerbSpawnEvent;

    [Method("Reset", "", "")]
    public void Reset() => this.Component.Reset();

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnTriggerEnterEvent -= new Action(this.OnTriggerEnterEvent);
      this.Component.OnTriggerLeaveEvent -= new Action(this.OnTriggerLeaveEvent);
      this.Component.OnActivateStartEvent -= new Action(this.OnActivateStartEvent);
      this.Component.OnActivateEndEvent -= new Action(this.OnActivateEndEvent);
      this.Component.OnHerbSpawnEvent -= this.HerbSpawnEvent;
      this.Component.OnLastHerbSpawnEvent -= this.LastHerbSpawnEvent;
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnTriggerEnterEvent += new Action(this.OnTriggerEnterEvent);
      this.Component.OnTriggerLeaveEvent += new Action(this.OnTriggerLeaveEvent);
      this.Component.OnActivateStartEvent += new Action(this.OnActivateStartEvent);
      this.Component.OnActivateEndEvent += new Action(this.OnActivateEndEvent);
      this.Component.OnHerbSpawnEvent += this.HerbSpawnEvent;
      this.Component.OnLastHerbSpawnEvent += this.LastHerbSpawnEvent;
    }

    private void OnTriggerEnterEvent()
    {
      Action triggerEnterEvent = this.TriggerEnterEvent;
      if (triggerEnterEvent == null)
        return;
      triggerEnterEvent();
    }

    private void OnTriggerLeaveEvent()
    {
      Action triggerLeaveEvent = this.TriggerLeaveEvent;
      if (triggerLeaveEvent == null)
        return;
      triggerLeaveEvent();
    }

    private void OnActivateStartEvent()
    {
      Action activateStartEvent = this.ActivateStartEvent;
      if (activateStartEvent == null)
        return;
      activateStartEvent();
    }

    private void OnActivateEndEvent()
    {
      Action activateEndEvent = this.ActivateEndEvent;
      if (activateEndEvent == null)
        return;
      activateEndEvent();
    }

    private void OnHerbSpawnEvent()
    {
      Action herbSpawnEvent = this.HerbSpawnEvent;
      if (herbSpawnEvent == null)
        return;
      herbSpawnEvent();
    }

    private void OnLastHerbSpawnEvent()
    {
      Action lastHerbSpawnEvent = this.LastHerbSpawnEvent;
      if (lastHerbSpawnEvent == null)
        return;
      lastHerbSpawnEvent();
    }

    public delegate void NeedCreateObjectEventType([Template] IEntity entity);
  }
}
