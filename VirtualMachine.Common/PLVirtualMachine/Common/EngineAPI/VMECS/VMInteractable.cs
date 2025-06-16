using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Interactive", typeof (IInteractableComponent))]
  public class VMInteractable : VMEngineComponent<IInteractableComponent>
  {
    public const string ComponentName = "Interactive";
    private ITextRef title;

    [Property("Title", "", true)]
    public ITextRef ObjectName
    {
      get => title;
      set
      {
        title = value;
        Component.Title = EngineAPIManager.CreateEngineTextInstance(title);
      }
    }

    [Event("Begin interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> BeginIteractEvent;

    [Event("End interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> EndIteractEvent;

    [Method("Enable component", "Enable", "")]
    public void EnableComponent(bool enable) => Component.IsEnabled = enable;

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.BeginInteractEvent -= FireBeginInteractEvent;
      Component.EndInteractEvent -= FireEndInteractEvent;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.BeginInteractEvent += FireBeginInteractEvent;
      Component.EndInteractEvent += FireEndInteractEvent;
    }

    private void FireBeginInteractEvent(
      IEntity owner,
      IInteractableComponent target,
      IInteractItem item)
    {
      Action<IEntity, IEntity, InteractType> beginIteractEvent = BeginIteractEvent;
      if (beginIteractEvent == null)
        return;
      beginIteractEvent(owner, target.Owner, item.Type);
    }

    private void FireEndInteractEvent(
      IEntity owner,
      IInteractableComponent target,
      IInteractItem item)
    {
      Action<IEntity, IEntity, InteractType> endIteractEvent = EndIteractEvent;
      if (endIteractEvent == null)
        return;
      endIteractEvent(owner, target.Owner, item.Type);
    }
  }
}
