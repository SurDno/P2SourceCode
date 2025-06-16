using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

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
        FundEnabled = (bool) obj1.Value;
      IParam obj2;
      if (((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.FundFinished", out obj2))
        FundFinished = (bool) obj2.Value;
      IParam obj3;
      if (((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.FundPoints", out obj3))
        FundPoints = (float) obj3.Value;
      IParam obj4;
      if (!((IBlueprint) templateObject).TryGetProperty("PlayerControllerComponent.CanReceiveMail", out obj4))
        return;
      CanReceiveMail = (bool) obj4.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnChangeHealth":
          OnChangeHealth += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnChangeInfection":
          OnChangeInfection += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnChangePreInfection":
          OnChangePreInfection += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnChangeSleep":
          OnChangeSleep += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "CombatActionEvent":
          CombatActionEvent += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
          break;
      }
    }
  }
}
