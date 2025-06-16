// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.DeadBurningAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DeadBurningAbilityController : IAbilityController, IChangeParameterListener
  {
    private AbilityItem abilityItem;
    private IEntity itemOwner;
    private Dictionary<AbilityValueNameEnum, IAbilityValue> values = new Dictionary<AbilityValueNameEnum, IAbilityValue>();
    private IParameter<bool> burningParameter;
    private IParameter<float> healthParameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      ParametersComponent component = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      this.burningParameter = component.GetByName<bool>(ParameterNameEnum.IsBurning);
      if (this.burningParameter == null)
        return;
      this.healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
      if (this.healthParameter == null)
        return;
      this.burningParameter.AddListener((IChangeParameterListener) this);
      this.healthParameter.AddListener((IChangeParameterListener) this);
    }

    private void CheckParameters()
    {
      if (this.burningParameter == null || this.healthParameter == null || !this.burningParameter.Value || (double) this.healthParameter.Value > 0.0)
        return;
      this.abilityItem.Active = true;
    }

    public void Shutdown()
    {
      if (this.burningParameter != null)
        this.burningParameter.RemoveListener((IChangeParameterListener) this);
      if (this.healthParameter != null)
        this.healthParameter.RemoveListener((IChangeParameterListener) this);
      this.abilityItem.Active = false;
      this.abilityItem = (AbilityItem) null;
    }

    public void OnParameterChanged(IParameter parameter) => this.CheckParameters();
  }
}
