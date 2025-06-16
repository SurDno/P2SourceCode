using Inspectors;

namespace Scripts.Expressions.Commons;

public class ExpressionViewWrapper {
	[Inspected(Mode = ExecuteMode.EditAndRuntime)]
	public string Value;
}