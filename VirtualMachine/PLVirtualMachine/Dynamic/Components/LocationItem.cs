using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMLocationItem))]
  public class LocationItem : VMLocationItem, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (LocationItem);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnChangeHibernation":
          OnChangeHibernation += () => target.RaiseFromEngineImpl();
          break;
        case "OnChangeLocation":
          OnChangeLocation += p1 => target.RaiseFromEngineImpl(p1);
          break;
      }
    }
  }
}
