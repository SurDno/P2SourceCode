using System;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Trigger", typeof (ITriggerComponent))]
  [Depended("Position")]
  public class VMTrigger : VMEngineComponent<ITriggerComponent>
  {
    public const string ComponentName = "Trigger";

    [Event("Object enter event", "Object")]
    public event Action<IEntity> ObjectEnterEvent;

    [Event("Object exit event", "Object")]
    public event Action<IEntity> ObjectExitEvent;

    [Method("Contains object", "object", "")]
    public virtual bool IsContainsObject(IEntity target) => Component.Contains(target);

    public void OnEntityEnter(
      ref EventArgument<IEntity, ITriggerComponent> eventArgs)
    {
      if (ObjectEnterEvent == null)
        return;
      ObjectEnterEvent(eventArgs.Actor);
    }

    public void OnEntityExit(
      ref EventArgument<IEntity, ITriggerComponent> eventArgs)
    {
      if (ObjectExitEvent == null)
        return;
      ObjectExitEvent(eventArgs.Actor);
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.EntityEnterEvent -= OnEntityEnter;
      Component.EntityExitEvent -= OnEntityExit;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.EntityEnterEvent += OnEntityEnter;
      Component.EntityExitEvent += OnEntityExit;
    }
  }
}
