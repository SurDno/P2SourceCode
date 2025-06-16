namespace PLVirtualMachine.Common;

public class LocalVariable : ContextVariable {
	public override EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOCAL_VAR;
}