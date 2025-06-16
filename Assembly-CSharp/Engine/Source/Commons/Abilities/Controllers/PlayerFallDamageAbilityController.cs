// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.PlayerFallDamageAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerFallDamageAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None, Name = "minFall")]
    [DataWriteProxy(MemberEnum.None, Name = "minFall")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float minFall;
    [DataReadProxy(MemberEnum.None, Name = "maxFall")]
    [DataWriteProxy(MemberEnum.None, Name = "maxFall")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float maxFall;
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
      this.playerMoveController.FallDamageEvent += new Action<float>(this.OnFallDamageEvent);
    }

    public void Shutdown()
    {
      ((IEntityView) this.abilityItem.Ability.Owner).OnGameObjectChangedEvent -= new Action(this.OnViewGameObjectChanged);
      if (!(bool) (UnityEngine.Object) this.playerMoveController)
        return;
      this.playerMoveController.FallDamageEvent -= new Action<float>(this.OnFallDamageEvent);
    }

    private void OnFallDamageEvent(float fallDistance)
    {
      if ((double) fallDistance < (double) this.minFall || (double) fallDistance > (double) this.maxFall)
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("<color=red>Fall damage : ").Append(fallDistance).Append("</color> , min : ").Append(this.minFall).Append(" , max : ").Append(this.maxFall));
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }
  }
}
