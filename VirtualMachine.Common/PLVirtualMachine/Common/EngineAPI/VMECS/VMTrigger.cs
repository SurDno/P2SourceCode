// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMTrigger
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
    public virtual bool IsContainsObject(IEntity target) => this.Component.Contains(target);

    public void OnEntityEnter(
      ref EventArgument<IEntity, ITriggerComponent> eventArgs)
    {
      if (this.ObjectEnterEvent == null)
        return;
      this.ObjectEnterEvent(eventArgs.Actor);
    }

    public void OnEntityExit(
      ref EventArgument<IEntity, ITriggerComponent> eventArgs)
    {
      if (this.ObjectExitEvent == null)
        return;
      this.ObjectExitEvent(eventArgs.Actor);
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.EntityEnterEvent -= new TriggerHandler(this.OnEntityEnter);
      this.Component.EntityExitEvent -= new TriggerHandler(this.OnEntityExit);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.EntityEnterEvent += new TriggerHandler(this.OnEntityEnter);
      this.Component.EntityExitEvent += new TriggerHandler(this.OnEntityExit);
    }
  }
}
