// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.BehaviorTreeAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BehaviorTreeAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    private AbilityItem abilityItem;
    private BehaviorComponent behavior;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.behavior = abilityItem.Ability.Owner.GetComponent<BehaviorComponent>();
      if (this.behavior == null)
        return;
      this.behavior.OnAbility += new Action<string, bool>(this.OnAbility);
    }

    public void Shutdown()
    {
      if (this.behavior == null)
        return;
      this.behavior.OnAbility -= new Action<string, bool>(this.OnAbility);
      this.behavior = (BehaviorComponent) null;
    }

    private void OnAbility(string name, bool enable)
    {
      if (!(this.name == name))
        return;
      this.abilityItem.Active = enable;
    }
  }
}
