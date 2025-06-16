// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.PlayerController
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMPlayerController))]
  public class PlayerController : 
    VMPlayerController,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => "PlayerControllerComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj1;
      if (((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.FundEnabled", out obj1))
        this.FundEnabled = (bool) obj1.Value;
      IParam obj2;
      if (((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.FundFinished", out obj2))
        this.FundFinished = (bool) obj2.Value;
      IParam obj3;
      if (((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.FundPoints", out obj3))
        this.FundPoints = (float) obj3.Value;
      IParam obj4;
      if (!((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.CanReceiveMail", out obj4))
        return;
      this.CanReceiveMail = (bool) obj4.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnChangeHealth":
          this.OnChangeHealth += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnChangeInfection":
          this.OnChangeInfection += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnChangePreInfection":
          this.OnChangePreInfection += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnChangeSleep":
          this.OnChangeSleep += (Action<bool>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "CombatActionEvent":
          this.CombatActionEvent += (Action<CombatActionEnum, IEntity>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
      }
    }
  }
}
