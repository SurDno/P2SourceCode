using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMNpcController))]
  public class NpcController : VMNpcController, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "NpcControllerComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "ActionEvent":
          ActionEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "CombatActionEvent":
          CombatActionEvent += (p1, p2) => target.RaiseFromEngineImpl(p1, p2);
          break;
        case "OnChangeHealth":
          OnChangeHealth += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "OnChangePain":
          OnChangePain += p1 => target.RaiseFromEngineImpl(p1);
          break;
        case "ChangeAwayEvent":
          ChangeAwayEvent += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
