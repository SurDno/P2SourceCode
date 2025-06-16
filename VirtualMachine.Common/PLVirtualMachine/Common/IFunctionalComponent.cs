using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IFunctionalComponent :
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	List<IEvent> EngineEvents { get; }

	List<BaseFunction> EngineFunctions { get; }

	string DependedComponentName { get; }

	bool Main { get; }
}