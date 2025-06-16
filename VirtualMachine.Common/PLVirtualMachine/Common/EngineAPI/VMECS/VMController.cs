// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMController
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Controller", typeof (IControllerComponent))]
  public class VMController : VMEngineComponent<IControllerComponent>
  {
    public const string ComponentName = "Controller";

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
      Action<IEntity, IEntity, InteractType> controllIteractEvent = this.BeginControllIteractEvent;
      if (controllIteractEvent == null)
        return;
      controllIteractEvent(owner, target.Owner, item.Type);
    }

    private void FireEndInteractEvent(
      IEntity owner,
      IInteractableComponent target,
      IInteractItem item)
    {
      Action<IEntity, IEntity, InteractType> controllIteractEvent = this.EndControllIteractEvent;
      if (controllIteractEvent == null)
        return;
      controllIteractEvent(owner, target.Owner, item.Type);
    }

    [Event("Begin interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> BeginControllIteractEvent;

    [Event("End interact event", "агент:Controller,цель:Interactive,тип")]
    public event Action<IEntity, IEntity, InteractType> EndControllIteractEvent;
  }
}
