using Engine.Behaviours.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerJumpAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None, Name = "jump")]
    [DataWriteProxy(MemberEnum.None, Name = "jump")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool jump;
    private AbilityItem abilityItem;
    private PlayerMoveController playerMoveController;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      IEntityView owner = (IEntityView) this.abilityItem.Ability.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null)
        owner.OnGameObjectChangedEvent += new Action(this.OnViewGameObjectChanged);
      else
        this.OnViewGameObjectChanged();
    }

    private void OnViewGameObjectChanged()
    {
      IEntityView owner = (IEntityView) this.abilityItem.Ability.Owner;
      if ((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null)
        return;
      this.playerMoveController = owner.GameObject.GetComponent<PlayerMoveController>();
      if (!(bool) (UnityEngine.Object) this.playerMoveController)
        return;
      owner.OnGameObjectChangedEvent -= new Action(this.OnViewGameObjectChanged);
      this.playerMoveController.JumpEvent += new Action<bool>(this.OnJumpEvent);
    }

    private void OnJumpEvent(bool jump)
    {
      if (this.jump != jump)
        return;
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }

    public void Shutdown()
    {
      ((IEntityView) this.abilityItem.Ability.Owner).OnGameObjectChangedEvent -= new Action(this.OnViewGameObjectChanged);
      if (!(bool) (UnityEngine.Object) this.playerMoveController)
        return;
      this.playerMoveController.JumpEvent -= new Action<bool>(this.OnJumpEvent);
    }
  }
}
