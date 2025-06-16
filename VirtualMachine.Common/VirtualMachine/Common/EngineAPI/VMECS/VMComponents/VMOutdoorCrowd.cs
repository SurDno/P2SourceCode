// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Common.EngineAPI.VMECS.VMComponents.VMOutdoorCrowd
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Crowds;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace VirtualMachine.Common.EngineAPI.VMECS.VMComponents
{
  [Info("OutdoorCrowdComponent", typeof (IOutdoorCrowdComponent))]
  public class VMOutdoorCrowd : VMEngineComponent<IOutdoorCrowdComponent>
  {
    public const string ComponentName = "OutdoorCrowdComponent";

    [Property("Layout", "")]
    public OutdoorCrowdLayoutEnum Layout
    {
      get => this.Component.Layout;
      set => this.Component.Layout = value;
    }

    [Event("Need create object event", "template object", false)]
    public event VMOutdoorCrowd.NeedCreateObjectEventType NeedCreateObjectEvent;

    [Event("Need delete object event", "object", false)]
    public event Action<IEntity> NeedDeleteObjectEvent;

    [Method("Add entity", "Target", "")]
    public void AddEntity(IEntity entity)
    {
      try
      {
        this.Component.AddEntity(entity);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Outdoor crowd entity adding error at {0}: {1} !", (object) this.Parent.Name, (object) ex.ToString()));
      }
    }

    [Method("Reset", "", "")]
    public void Reset() => this.Component.Reset();

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

    private void OnDeleteEntity(IEntity entity)
    {
      Action<IEntity> deleteObjectEvent = this.NeedDeleteObjectEvent;
      if (deleteObjectEvent == null)
        return;
      deleteObjectEvent(entity);
    }

    private void OnCreateEntity(IEntity entity)
    {
      VMOutdoorCrowd.NeedCreateObjectEventType createObjectEvent = this.NeedCreateObjectEvent;
      if (createObjectEvent == null)
        return;
      createObjectEvent(entity);
    }

    public delegate void NeedCreateObjectEventType([Template] IEntity entity);
  }
}
