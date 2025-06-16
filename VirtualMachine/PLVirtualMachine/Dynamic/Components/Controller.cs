using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMController))]
  public class Controller : VMController, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Controller);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "BeginControllIteractEvent":
          this.BeginControllIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
        case "EndControllIteractEvent":
          this.EndControllIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
      }
    }
  }
}
