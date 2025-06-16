using Cofe.Proxies;
using Engine.Common.Components.AttackerPlayer;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMAttackerPlayer))]
  public class AttackerPlayer : 
    VMAttackerPlayer,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (AttackerPlayer);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "HandsHolsteredEvent":
          this.HandsHolsteredEvent += (Action<WeaponKind>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "HandsUnholsteredEvent":
          this.HandsUnholsteredEvent += (Action<WeaponKind>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
