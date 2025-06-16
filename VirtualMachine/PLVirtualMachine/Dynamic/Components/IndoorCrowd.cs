using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Objects;
using System;
using VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMIndoorCrowd))]
  public class IndoorCrowd : VMIndoorCrowd, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "IndoorCrowdComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "NeedCreateObjectEvent":
          this.NeedCreateObjectEvent += (VMIndoorCrowd.NeedCreateObjectEventType) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "NeedDeleteObjectEvent":
          this.NeedDeleteObjectEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
