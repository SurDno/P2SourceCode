using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

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
      get => this.title;
      set
      {
        this.title = value;
        this.Component.Title = EngineAPIManager.CreateEngineTextInstance(this.title);
      }
    }

    [Event("Begin interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> BeginIteractEvent;

    [Event("End interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> EndIteractEvent;

    [Method("Enable component", "Enable", "")]
    public void EnableComponent(bool enable) => this.Component.IsEnabled = enable;

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.BeginInteractEvent -= new Action<IEntity, IInteractableComponent, IInteractItem>(this.FireBeginInteractEvent);
      this.Component.EndInteractEvent -= new Action<IEntity, IInteractableComponent, IInteractItem>(this.FireEndInteractEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.BeginInteractEvent += new Action<IEntity, IInteractableComponent, IInteractItem>(this.FireBeginInteractEvent);
      this.Component.EndInteractEvent += new Action<IEntity, IInteractableComponent, IInteractItem>(this.FireEndInteractEvent);
    }

    private void FireBeginInteractEvent(
      IEntity owner,
      IInteractableComponent target,
      IInteractItem item)
    {
      Action<IEntity, IEntity, InteractType> beginIteractEvent = this.BeginIteractEvent;
      if (beginIteractEvent == null)
        return;
      beginIteractEvent(owner, target.Owner, item.Type);
    }

    private void FireEndInteractEvent(
      IEntity owner,
      IInteractableComponent target,
      IInteractItem item)
    {
      Action<IEntity, IEntity, InteractType> endIteractEvent = this.EndIteractEvent;
      if (endIteractEvent == null)
        return;
      endIteractEvent(owner, target.Owner, item.Type);
    }
  }
}
