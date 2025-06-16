// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.StealthAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StealthAbilityController : IAbilityController
  {
    private IControllerComponent controller;
    private AbilityItem abilityItem;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.controller = this.abilityItem.Ability.Owner.GetComponent<IControllerComponent>();
      if (this.controller == null)
        return;
      this.controller.IsStelth.ChangeValueEvent += new Action<bool>(this.OnStelthEnableChanged);
      this.OnStelthEnableChanged(this.controller.IsStelth.Value);
    }

    public void Shutdown()
    {
      if (this.controller == null)
        return;
      this.controller.IsStelth.ChangeValueEvent -= new Action<bool>(this.OnStelthEnableChanged);
      this.controller = (IControllerComponent) null;
    }

    private void OnStelthEnableChanged(bool enabled) => this.abilityItem.Active = enabled;
  }
}
