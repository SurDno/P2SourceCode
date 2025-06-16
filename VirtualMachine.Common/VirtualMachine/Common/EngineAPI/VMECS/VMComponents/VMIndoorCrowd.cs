using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace VirtualMachine.Common.EngineAPI.VMECS.VMComponents
{
  [Info("IndoorCrowdComponent", typeof (IIndoorCrowdComponent))]
  public class VMIndoorCrowd : VMEngineComponent<IIndoorCrowdComponent>
  {
    public const string ComponentName = "IndoorCrowdComponent";

    [Event("Need create object event", "template object", false)]
    public event VMIndoorCrowd.NeedCreateObjectEventType NeedCreateObjectEvent;

    [Event("Need delete object event", "object", false)]
    public event Action<IEntity> NeedDeleteObjectEvent;

    [Method("Add entity", "Target", "")]
    public void AddEntity(IEntity entity) => this.Component.AddEntity(entity);

    [Method("Reset", "", "")]
    public void Reset() => this.Component.Reset();

    public void OnCreateEntity(IEntity entity)
    {
      EngineAPIManager.ObjectCreationExtraDebugInfoMode = true;
      VMIndoorCrowd.NeedCreateObjectEventType createObjectEvent = this.NeedCreateObjectEvent;
      if (createObjectEvent != null)
        createObjectEvent(entity);
      EngineAPIManager.ObjectCreationExtraDebugInfoMode = false;
    }

    public void OnDeleteEntity(IEntity entity)
    {
      EngineAPIManager.ObjectCreationExtraDebugInfoMode = true;
      Action<IEntity> deleteObjectEvent = this.NeedDeleteObjectEvent;
      if (deleteObjectEvent != null)
        deleteObjectEvent(entity);
      EngineAPIManager.ObjectCreationExtraDebugInfoMode = false;
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnCreateEntity -= new Action<IEntity>(this.OnCreateEntity);
      this.Component.OnDeleteEntity -= new Action<IEntity>(this.OnDeleteEntity);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnCreateEntity += new Action<IEntity>(this.OnCreateEntity);
      this.Component.OnDeleteEntity += new Action<IEntity>(this.OnDeleteEntity);
    }

    public delegate void NeedCreateObjectEventType([Template] IEntity entity);
  }
}
