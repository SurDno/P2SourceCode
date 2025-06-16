using System.Collections.Generic;

namespace PLVirtualMachine.Common;

public interface IContextElement : IObject, IEditorBaseTemplate, INamed {
	List<IVariable> LocalContextVariables { get; }

	IVariable GetLocalContextVariable(string variableUniName);
}