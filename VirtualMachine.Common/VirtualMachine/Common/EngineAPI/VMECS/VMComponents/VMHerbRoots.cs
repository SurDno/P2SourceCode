using System;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
    public void Reset() => Component.Reset();

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.OnTriggerEnterEvent -= OnTriggerEnterEvent;
      Component.OnTriggerLeaveEvent -= OnTriggerLeaveEvent;
      Component.OnActivateStartEvent -= OnActivateStartEvent;
      Component.OnActivateEndEvent -= OnActivateEndEvent;
      Component.OnHerbSpawnEvent -= HerbSpawnEvent;
      Component.OnLastHerbSpawnEvent -= LastHerbSpawnEvent;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.OnTriggerEnterEvent += OnTriggerEnterEvent;
      Component.OnTriggerLeaveEvent += OnTriggerLeaveEvent;
      Component.OnActivateStartEvent += OnActivateStartEvent;
      Component.OnActivateEndEvent += OnActivateEndEvent;
      Component.OnHerbSpawnEvent += HerbSpawnEvent;
      Component.OnLastHerbSpawnEvent += LastHerbSpawnEvent;
    }

    private void OnTriggerEnterEvent()
    {
      Action triggerEnterEvent = TriggerEnterEvent;
      if (triggerEnterEvent == null)
        return;
      triggerEnterEvent();
    }

    private void OnTriggerLeaveEvent()
    {
      Action triggerLeaveEvent = TriggerLeaveEvent;
      if (triggerLeaveEvent == null)
        return;
      triggerLeaveEvent();
    }

    private void OnActivateStartEvent()
    {
      Action activateStartEvent = ActivateStartEvent;
      if (activateStartEvent == null)
        return;
      activateStartEvent();
    }

    private void OnActivateEndEvent()
    {
      Action activateEndEvent = ActivateEndEvent;
      if (activateEndEvent == null)
        return;
      activateEndEvent();
    }

    private void OnHerbSpawnEvent()
    {
      Action herbSpawnEvent = HerbSpawnEvent;
      if (herbSpawnEvent == null)
        return;
      herbSpawnEvent();
    }

    private void OnLastHerbSpawnEvent()
    {
      Action lastHerbSpawnEvent = LastHerbSpawnEvent;
      if (lastHerbSpawnEvent == null)
        return;
      lastHerbSpawnEvent();
    }

    public delegate void NeedCreateObjectEventType([Template] IEntity entity);
  }
}
