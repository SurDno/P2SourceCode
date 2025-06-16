using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System;

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
