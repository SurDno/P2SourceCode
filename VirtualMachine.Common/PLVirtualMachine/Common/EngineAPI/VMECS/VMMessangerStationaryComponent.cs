using System;
using Cofe.Loggers;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("MessangerStationaryComponent", typeof (IMessangerStationaryComponent))]
  public class VMMessangerStationaryComponent : VMEngineComponent<IMessangerStationaryComponent>
  {
    public const string ComponentName = "MessangerStationaryComponent";

    [Method("Start teleporting", "", "")]
    public void StartTeleporting() => Component.StartTeleporting();

    [Method("Stop teleporting", "", "")]
    public void StopTeleporting() => Component.StopTeleporting();

    [Property("SpawnPoint Kind", "")]
    public SpawnpointKindEnum SpawnPointKind
    {
      get
      {
        if (Component != null)
          return Component.SpawnpointKind;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return SpawnpointKindEnum.None;
      }
      set
      {
        if (Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        }
        else
        {
          try
          {
            Component.SpawnpointKind = value;
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("SpawnPointKind set error: {0} at {1}", ex, Parent.Name));
          }
        }
      }
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      base.Clear();
    }

    protected override void Init()
    {
      int num = IsTemplate ? 1 : 0;
    }
  }
}
