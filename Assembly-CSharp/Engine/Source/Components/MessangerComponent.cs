using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IMessangerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MessangerComponent : EngineComponent, IMessangerComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected bool registred;
    private int areaMask = -1;

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
      ServiceLocator.GetService<PostmanTeleportService>().RegisterPostman(this.Owner, this.areaMask);
      this.registred = true;
    }

    public void StopTeleporting()
    {
      ServiceLocator.GetService<PostmanTeleportService>().UnregisterPostman(this.Owner);
      this.registred = false;
    }

    [Cofe.Serializations.Data.OnLoaded]
    protected void OnLoaded()
    {
      if (!this.registred)
        return;
      ServiceLocator.GetService<PostmanTeleportService>().RegisterPostman(this.Owner, this.areaMask);
    }
  }
}
