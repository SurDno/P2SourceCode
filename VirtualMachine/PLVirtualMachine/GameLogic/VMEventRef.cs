using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic;

[VMType("IEventRef")]
[VMFactory(typeof(IEventRef))]
public class VMEventRef : BaseRef, IEventRef, IRef, IVariable, INamed, IVMStringSerializable {
	public void Initialize(IEvent evnt) {
		LoadStaticInstance(evnt);
	}

	public override EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_EVENT;

	public IEvent Event {
		get {
			if (StaticInstance == null && BaseGuid > 0UL)
				LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
			return (IEvent)StaticInstance;
		}
	}

	public override VMType Type => new(typeof(IEventRef));

	public override bool Empty => Event == null && base.Empty;

	protected override Type NeedInstanceType => typeof(IEvent);
}