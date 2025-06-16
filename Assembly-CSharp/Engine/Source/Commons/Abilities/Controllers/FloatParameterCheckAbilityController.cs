using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FloatParameterCheckAbilityController : IAbilityController, IChangeParameterListener {
	[DataReadProxy(Name = "Parameter")]
	[DataWriteProxy(Name = "Parameter")]
	[CopyableProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum parameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float value;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected CompareEnum compare;

	private AbilityItem abilityItem;
	private ParametersComponent parameters;
	private IParameter<float> parameter;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		parameters = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
		parameter = parameters.GetByName<float>(parameterName);
		if (parameter == null)
			return;
		parameter.AddListener(this);
		CheckParameter();
	}

	public void Shutdown() {
		if (parameter != null) {
			parameter.RemoveListener(this);
			abilityItem.Active = false;
		}

		abilityItem = null;
	}

	private void CheckParameter() {
		if (parameter == null)
			return;
		var flag = false;
		if (compare == CompareEnum.Equal)
			flag = parameter.Value == (double)value;
		else if (compare == CompareEnum.Less)
			flag = parameter.Value < (double)value;
		else if (compare == CompareEnum.More)
			flag = parameter.Value > (double)value;
		else if (compare == CompareEnum.LessEqual)
			flag = parameter.Value <= (double)value;
		else if (compare == CompareEnum.MoreEqual)
			flag = parameter.Value >= (double)value;
		abilityItem.Active = flag;
	}

	public void OnParameterChanged(IParameter parameter) {
		CheckParameter();
	}
}