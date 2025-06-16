using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IMessangerStationaryComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MessangerStationaryComponent : 
    EngineComponent,
    IMessangerStationaryComponent,
    IComponent,
    INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected SpawnpointKindEnum spawnpointKindEnum;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected bool registred;

    public SpawnpointKindEnum SpawnpointKind
    {
      get => this.spawnpointKindEnum;
      set
      {
        this.spawnpointKindEnum = value;
        if (!this.registred)
          return;
        this.StopTeleporting();
        this.StartTeleporting();
      }
    }

    public bool NeedSave => true;

    public override void OnAdded()
    {
    }

    public override void OnRemoved()
    {
      if (!this.registred)
        return;
      this.StopTeleporting();
    }

    public void StartTeleporting()
    {
      ServiceLocator.GetService<PostmanStaticTeleportService>().RegisterPostman(this.Owner, this.spawnpointKindEnum);
      this.registred = true;
    }

    public void StopTeleporting()
    {
      ServiceLocator.GetService<PostmanStaticTeleportService>().UnregisterPostman(this.Owner);
      this.registred = false;
    }

    [Cofe.Serializations.Data.OnLoaded]
    protected void OnLoaded()
    {
      if (!this.registred)
        return;
      ServiceLocator.GetService<PostmanStaticTeleportService>().RegisterPostman(this.Owner, this.spawnpointKindEnum);
    }
  }
}
