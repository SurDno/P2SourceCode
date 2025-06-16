// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.ContainerOpenAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ContainerOpenAbilityController : IAbilityController, IChangeParameterListener
  {
    private AbilityItem abilityItem;
    private IParameter<ContainerOpenStateEnum> parameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.parameter = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>()?.GetByName<ContainerOpenStateEnum>(ParameterNameEnum.OpenState);
      if (this.parameter == null)
        return;
      this.parameter.AddListener((IChangeParameterListener) this);
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (this.parameter.Value != ContainerOpenStateEnum.Open)
        return;
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }

    public void Shutdown()
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.abilityItem = (AbilityItem) null;
      this.parameter = (IParameter<ContainerOpenStateEnum>) null;
    }
  }
}
