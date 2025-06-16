using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IBlueprint :
	IGameObjectContext,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	IContext,
	ILogicObject {
	List<IBlueprint> BaseBlueprints { get; }

	List<BaseFunction> Functions { get; }

	IFiniteStateMachine StateGraph { get; }

	Dictionary<string, IFunctionalComponent> FunctionalComponents { get; }

	bool TryGetProperty(string name, out IParam param);

	IParam GetProperty(string componentName, string propertyName);

	bool IsDerivedFrom(ulong blueprintGuid, bool withSelf = false);

	new IVariable GetSelf();
}