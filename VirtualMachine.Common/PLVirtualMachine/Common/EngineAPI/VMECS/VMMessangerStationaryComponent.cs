// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMMessangerStationaryComponent
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("MessangerStationaryComponent", typeof (IMessangerStationaryComponent))]
  public class VMMessangerStationaryComponent : VMEngineComponent<IMessangerStationaryComponent>
  {
    public const string ComponentName = "MessangerStationaryComponent";

    [Method("Start teleporting", "", "")]
    public void StartTeleporting() => this.Component.StartTeleporting();

    [Method("Stop teleporting", "", "")]
    public void StopTeleporting() => this.Component.StopTeleporting();

    [Property("SpawnPoint Kind", "")]
    public SpawnpointKindEnum SpawnPointKind
    {
      get
      {
        if (this.Component != null)
          return this.Component.SpawnpointKind;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return SpawnpointKindEnum.None;
      }
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          try
          {
            this.Component.SpawnpointKind = value;
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("SpawnPointKind set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
          }
        }
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      base.Clear();
    }

    protected override void Init()
    {
      int num = this.IsTemplate ? 1 : 0;
    }
  }
}
