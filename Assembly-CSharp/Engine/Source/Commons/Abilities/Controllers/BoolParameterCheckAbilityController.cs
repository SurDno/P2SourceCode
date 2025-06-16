using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BoolParameterCheckAbilityController : IAbilityController, IChangeParameterListener
  {
    [DataReadProxy(MemberEnum.None, Name = "Parameter")]
    [DataWriteProxy(MemberEnum.None, Name = "Parameter")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum parameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool value;
    private AbilityItem abilityItem;
    private ParametersComponent parameters;
    private IParameter<bool> parameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.parameters = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      this.parameter = this.parameters.GetByName<bool>(this.parameterName);
      if (this.parameter == null)
        return;
      this.parameter.AddListener((IChangeParameterListener) this);
      this.CheckParameter();
    }

    public void Shutdown()
    {
      if (this.parameter != null)
      {
        this.parameter.RemoveListener((IChangeParameterListener) this);
        this.abilityItem.Active = false;
      }
      this.abilityItem = (AbilityItem) null;
    }

    private void CheckParameter()
    {
      if (this.parameter == null)
        return;
      this.abilityItem.Active = this.parameter.Value == this.value;
    }

    public void OnParameterChanged(IParameter parameter) => this.CheckParameter();
  }
}
