// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.NpcPunchToDodgeEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Controllers;
using Engine.Source.Commons.Effects;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcPunchToDodgeEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy(MemberEnum.None, Name = "punchType")]
    [DataWriteProxy(MemberEnum.None, Name = "punchType")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected PunchTypeEnum punchEnum;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

    public string Name => this.name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public void Cleanup()
    {
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      EnemyBase component1 = ((IEntityView) this.AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
      EnemyBase component2 = ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>();
      if (!(this.AbilityItem.AbilityController is CloseCombatAbilityController abilityController))
      {
        Debug.LogError((object) (typeof (NpcPunchEffect).Name + " requires " + typeof (CloseCombatAbilityController).Name));
        return false;
      }
      component2?.PunchToDodge(this.punchEnum, abilityController.ReactionType, abilityController.WeaponKind, component1);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime) => false;
  }
}
