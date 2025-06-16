using Cofe.Proxies;
using Engine.Common.Services.Mails;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMMail))]
public class Mail : VMMail, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(Mail);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) {
		IParam obj1;
		if (((IBlueprint)templateObject).TryGetProperty("Mail.Header", out obj1))
			Header = (ITextRef)obj1.Value;
		IParam obj2;
		if (((IBlueprint)templateObject).TryGetProperty("Mail.Text", out obj2))
			Text = (ITextRef)obj2.Value;
		IParam obj3;
		if (!((IBlueprint)templateObject).TryGetProperty("Mail.State", out obj3))
			return;
		State = (MailStateEnum)obj3.Value;
	}

	public void InitialiseEvent(DynamicEvent target) {
		var name = target.Name;
	}
}