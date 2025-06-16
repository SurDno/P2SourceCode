// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.FloatParameterCheckAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FloatParameterCheckAbilityController : IAbilityController, IChangeParameterListener
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
    protected float value;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected CompareEnum compare;
    private AbilityItem abilityItem;
    private ParametersComponent parameters;
    private IParameter<float> parameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.parameters = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      this.parameter = this.parameters.GetByName<float>(this.parameterName);
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
      bool flag = false;
      if (this.compare == CompareEnum.Equal)
        flag = (double) this.parameter.Value == (double) this.value;
      else if (this.compare == CompareEnum.Less)
        flag = (double) this.parameter.Value < (double) this.value;
      else if (this.compare == CompareEnum.More)
        flag = (double) this.parameter.Value > (double) this.value;
      else if (this.compare == CompareEnum.LessEqual)
        flag = (double) this.parameter.Value <= (double) this.value;
      else if (this.compare == CompareEnum.MoreEqual)
        flag = (double) this.parameter.Value >= (double) this.value;
      this.abilityItem.Active = flag;
    }

    public void OnParameterChanged(IParameter parameter) => this.CheckParameter();
  }
}
