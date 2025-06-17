using Cofe.Proxies;
using Engine.Common.Services.Mails;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMMail))]
  public class Mail : VMMail, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Mail);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      if (((IBlueprint) templateObject).TryGetProperty("Mail.Header", out IParam obj1))
        Header = (ITextRef) obj1.Value;
      if (((IBlueprint) templateObject).TryGetProperty("Mail.Text", out IParam obj2))
        Text = (ITextRef) obj2.Value;
      if (!((IBlueprint) templateObject).TryGetProperty("Mail.State", out IParam obj3))
        return;
      State = (MailStateEnum) obj3.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
