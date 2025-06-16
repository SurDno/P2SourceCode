using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public class AbstractParameter : IVariable, INamed, IParam {
	protected string name;
	protected string componentName;
	protected VMType paramType;
	protected object defaultValue;

	public AbstractParameter(
		string name,
		string componentName,
		VMType type,
		object defValue,
		bool not_used = false) {
		this.name = name;
		this.componentName = componentName;
		paramType = type;
	}

	public EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;

	public string Name => componentName != "" ? componentName + "." + name : name;

	public object Value {
		get => defaultValue;
		set { }
	}

	public VMType Type => paramType;

	public bool Implicit => false;

	public IGameObjectContext OwnerContext => null;

	public virtual bool IsEqual(IVariable other) {
		if (!typeof(AbstractParameter).IsAssignableFrom(other.GetType()))
			return false;
		var abstractParameter = (AbstractParameter)other;
		return (!("" != componentName) || !(componentName != abstractParameter.componentName)) &&
		       Name == abstractParameter.Name;
	}

	public void Clear() {
		defaultValue = null;
	}
}