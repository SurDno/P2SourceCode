// Decompiled with JetBrains decompiler
// Type: EnemyBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class EnemyBase : MonoBehaviour, IEntityAttachable
{
  [Header("You can use ComputeNpc to fill this!")]
  [SerializeField]
  protected Pivot pivot;
  [SerializeField]
  protected NavMeshAgent agent;
  private IParameter<bool> deadParameter;
  private IParameter<float> healthParameter;
  private IParameter<bool> cantBlock;
  private float blockNormalizedTime = 0.0f;
  private IParameter<float> blockStanceParameter;
  private bool blockStance;
  private IParameter<BlockTypeEnum> blocktype;
  private IParameter<float> stamina;
  private IParameter<bool> isPushed;
  private bool isFaint;
  private IParameter<bool> isFighting;
  private IParameter<bool> isCombatIgnored;
  private IParameter<bool> isImmortal;
  protected float currentWalkSpeed = 0.0f;
  protected float desiredWalkSpeed = 0.0f;
  private float? retreatAngle;
  private bool rotateByPath;
  private Transform rotationTarget;
  private EnemyBase enemy;
  [Inspected]
  private HashSet<EnemyBase> attackers = new HashSet<EnemyBase>();

  public event Action<EnemyBase> WasPrepunchEvent;

  public void FireWasWasPrepunchEvent()
  {
    Action<EnemyBase> wasPrepunchEvent = this.WasPrepunchEvent;
    if (wasPrepunchEvent == null)
      return;
    wasPrepunchEvent(this);
  }

  public event Action<EnemyBase> WasPunchedEvent;

  public void FireWasPunchedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasPunchedEvent = this.WasPunchedEvent;
    if (wasPunchedEvent == null)
      return;
    wasPunchedEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToSurrenderEvent;

  public void FireWasPunchedToSurrenderEvent(EnemyBase enemy)
  {
    Action<EnemyBase> toSurrenderEvent = this.WasPunchedToSurrenderEvent;
    if (toSurrenderEvent == null)
      return;
    toSurrenderEvent(enemy);
  }

  public event Action<EnemyBase> WasLowStaminaPunchedEvent;

  public void FireWasLowStaminaPunchedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> staminaPunchedEvent = this.WasLowStaminaPunchedEvent;
    if (staminaPunchedEvent == null)
      return;
    staminaPunchedEvent(enemy);
  }

  public event Action<EnemyBase> WasStaggeredEvent;

  public void FireWasStaggeredEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasStaggeredEvent = this.WasStaggeredEvent;
    if (wasStaggeredEvent == null)
      return;
    wasStaggeredEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToBlockEvent;

  public void FireWasPunchedToBlockEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToBlockEvent = this.WasPunchedToBlockEvent;
    if (punchedToBlockEvent == null)
      return;
    punchedToBlockEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToQuickBlock;

  public void FireWasPunchedToQuickBlockEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToQuickBlock = this.WasPunchedToQuickBlock;
    if (punchedToQuickBlock == null)
      return;
    punchedToQuickBlock(enemy);
  }

  public event Action<EnemyBase> WasPunchedToDodgeEvent;

  public void FireWasPunchedToDodgeEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToDodgeEvent = this.WasPunchedToDodgeEvent;
    if (punchedToDodgeEvent == null)
      return;
    punchedToDodgeEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToStaggerEvent;

  public void FireWasPunchedToStaggerEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToStaggerEvent = this.WasPunchedToStaggerEvent;
    if (punchedToStaggerEvent == null)
      return;
    punchedToStaggerEvent(enemy);
  }

  public event Action<EnemyBase> WasPushedEvent;

  public void FireWasPushedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasPushedEvent = this.WasPushedEvent;
    if (wasPushedEvent == null)
      return;
    wasPushedEvent(enemy);
  }

  public event Action<EnemyBase> WasKnockDownedEvent;

  public void FireWasKnockDownedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> knockDownedEvent = this.WasKnockDownedEvent;
    if (knockDownedEvent == null)
      return;
    knockDownedEvent(enemy);
  }

  public event Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum> PunchEvent;

  public void FirePunchEvent(
    WeaponEnum weapon,
    IEntity weaponEntity,
    ShotType punch,
    ReactionType reaction,
    ShotSubtypeEnum subtype = ShotSubtypeEnum.None)
  {
    Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum> punchEvent = this.PunchEvent;
    if (punchEvent == null)
      return;
    punchEvent(weaponEntity, punch, reaction, weapon, subtype);
  }

  public event Action<WeaponEnum> PunchHitEvent;

  public void FirePunchHitEvent(WeaponEnum weapon)
  {
    Action<WeaponEnum> punchHitEvent = this.PunchHitEvent;
    if (punchHitEvent == null)
      return;
    punchHitEvent(weapon);
  }

  [Inspected]
  public virtual bool IsStagger => false;

  [Inspected]
  public float StaggerTime { get; set; }

  public Vector3 CalculateRepulseVelocity(EnemyBase attacker)
  {
    Vector3 zero = Vector3.zero;
    if (this.attackers.Count < 2)
      return Vector3.zero;
    float num1 = 0.5f;
    float num2 = 2f;
    foreach (EnemyBase attacker1 in this.attackers)
    {
      if (!((UnityEngine.Object) attacker1 == (UnityEngine.Object) attacker))
      {
        Vector3 vector3 = attacker.transform.position - attacker1.transform.position;
        float num3 = num1 * num1 / vector3.sqrMagnitude;
        if ((double) num3 >= 0.15999999642372131)
        {
          vector3.Normalize();
          zero += num3 * vector3 * num2;
        }
      }
    }
    return zero;
  }

  protected void CaclulateRection(
    EnemyBase enemy,
    ReactionType reactionType,
    out float fightReactionX,
    out float fightReactionY)
  {
    Vector3 position = enemy.transform.position;
    switch (reactionType)
    {
      case ReactionType.Left:
        position -= enemy.transform.right * 0.3f;
        break;
      case ReactionType.Right:
        position += enemy.transform.right * 0.3f;
        break;
    }
    Vector3 normalized = (position - this.transform.position).normalized;
    float y = Vector3.Cross(normalized, this.transform.forward).y;
    if ((double) Vector3.Dot(normalized, this.transform.forward) > 0.0)
    {
      if ((double) Mathf.Abs(y) < (double) Mathf.Sin(0.2617994f))
      {
        switch (reactionType)
        {
          case ReactionType.Left:
            fightReactionX = 0.0f;
            fightReactionY = 1f;
            break;
          case ReactionType.Right:
            fightReactionX = 0.0f;
            fightReactionY = 1f;
            break;
          case ReactionType.Uppercut:
            fightReactionX = 0.0f;
            fightReactionY = 2f;
            break;
          default:
            fightReactionX = 0.0f;
            fightReactionY = 1f;
            break;
        }
      }
      else
      {
        bool flag = (double) y < 0.0;
        fightReactionX = flag ? 1f : -1f;
        fightReactionY = 0.0f;
      }
    }
    else
    {
      fightReactionX = 0.0f;
      fightReactionY = -1f;
    }
  }

  public void PlayLipSync(CombatCryEnum cryEnum)
  {
    if (!((UnityEngine.Object) this.pivot != (UnityEngine.Object) null) || !((UnityEngine.Object) this.pivot.SoundBank != (UnityEngine.Object) null))
      return;
    IEntity entity = EntityUtility.GetEntity(this.gameObject);
    if (entity != null)
    {
      LipSyncComponent component = entity.GetComponent<LipSyncComponent>();
      NPCSoundBankCrySettings soundBankCrySettings = this.pivot.SoundBank.CombatCries.Find((Predicate<NPCSoundBankCrySettings>) (x => x.Name == cryEnum));
      if (component != null && soundBankCrySettings != null && (double) soundBankCrySettings.Chance > (double) UnityEngine.Random.value)
      {
        LipSyncObjectSerializable description = soundBankCrySettings.Description;
        component.Play3D((ILipSyncObject) description.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcCombatReplics, true);
      }
    }
  }

  private void OnEnable() => ServiceLocator.GetService<CombatService>()?.RegisterCharacter(this);

  protected virtual void OnDisable()
  {
    ServiceLocator.GetService<CombatService>()?.UnregisterCharacter(this);
  }

  private void OnDestroy()
  {
    this.Enemy = (EnemyBase) null;
    ServiceLocator.GetService<CombatService>()?.UnregisterCharacter(this);
  }

  public void RegisterAttacker(EnemyBase enemy)
  {
    if (this.attackers.Add(enemy))
      return;
    Debug.LogWarning((object) ("RegisterAttacker " + this.gameObject.name + " already cotains " + enemy.gameObject.name));
  }

  public void UnregisterAttacker(EnemyBase enemy)
  {
    if (this.attackers.Remove(enemy))
      return;
    Debug.LogWarning((object) ("UnregisterAttacker " + this.gameObject.name + " doesn't cotain " + enemy.gameObject.name));
  }

  [Inspected]
  public EnemyBase Enemy
  {
    get => this.enemy;
    set
    {
      this.enemy?.UnregisterAttacker(this);
      this.enemy = value;
      this.enemy?.RegisterAttacker(this);
    }
  }

  public bool IsDead => this.deadParameter != null && this.deadParameter.Value;

  private IParameter<float> Health => this.healthParameter;

  public IEntity Owner { get; private set; }

  public bool CantBlock
  {
    get => this.cantBlock != null && this.cantBlock.Value;
    set
    {
      if (this.cantBlock == null)
        return;
      this.cantBlock.Value = value;
    }
  }

  public float BlockNormalizedTime
  {
    get => this.blockNormalizedTime;
    set => this.blockNormalizedTime = value;
  }

  public virtual bool BlockStance
  {
    get => this.blockStance;
    set
    {
      this.blockStance = value;
      if (this.blockStanceParameter == null)
        return;
      this.blockStanceParameter.Value = this.blockStance ? 1f : 0.0f;
    }
  }

  public BlockTypeEnum BlockType
  {
    get => this.blocktype == null ? BlockTypeEnum.None : this.blocktype.Value;
    set
    {
      if (this.blocktype == null)
        return;
      this.blocktype.Value = value;
    }
  }

  public float Stamina
  {
    get => this.stamina == null ? 0.0f : this.stamina.Value;
    set
    {
      if (this.stamina == null)
        return;
      this.stamina.Value = value;
    }
  }

  public bool IsPushed
  {
    get => this.isPushed != null && this.isPushed.Value;
    set
    {
      if (this.isPushed == null)
        return;
      this.isPushed.Value = value;
    }
  }

  public bool IsFighting
  {
    get => this.isFighting != null && this.isFighting.Value;
    set
    {
      if (this.isFighting == null)
        return;
      this.isFighting.Value = value;
    }
  }

  public bool IsCombatIgnored
  {
    get => this.isCombatIgnored != null && this.isCombatIgnored.Value;
    set
    {
      if (this.isCombatIgnored == null)
        return;
      this.isCombatIgnored.Value = value;
    }
  }

  public bool IsImmortal
  {
    get => this.isImmortal != null && this.isImmortal.Value;
    set
    {
      if (this.isImmortal == null)
        return;
      this.isImmortal.Value = value;
    }
  }

  public bool IsFaint
  {
    get => this.isFaint;
    set => this.isFaint = value;
  }

  public Transform RotationTarget
  {
    get => this.rotationTarget;
    set => this.rotationTarget = value;
  }

  public bool RotateByPath
  {
    get => this.rotateByPath;
    set => this.rotateByPath = value;
  }

  public float? RetreatAngle
  {
    get => this.retreatAngle;
    set => this.retreatAngle = value;
  }

  [Inspected]
  public float DesiredWalkSpeed
  {
    get => this.desiredWalkSpeed;
    set => this.desiredWalkSpeed = value;
  }

  [Inspected]
  public float CurrentWalkSpeed
  {
    get => this.currentWalkSpeed;
    set => this.currentWalkSpeed = value;
  }

  public virtual void Prepunch(ReactionType reactionType, WeaponEnum weapon, EnemyBase enemy)
  {
  }

  public virtual void PrepunchUppercut(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchToSurrender(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchToBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchToQuickBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchToDodge(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchToStagger(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void PunchLowStamina(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
  }

  public virtual void Stagger(EnemyBase enemy)
  {
  }

  public virtual void KnockDown(EnemyBase enemy)
  {
  }

  public virtual void Push(Vector3 velocity, EnemyBase enemy)
  {
  }

  public virtual void PushMove(Vector3 direction)
  {
  }

  public virtual void KnockDownMove(float angularVelocity)
  {
  }

  [Inspected]
  public virtual Vector3 Velocity { get; }

  public virtual void OnExternalAnimatorMove()
  {
  }

  protected void FixedUpdate()
  {
    if (this.BlockType == BlockTypeEnum.Block)
      this.BlockNormalizedTime += Time.fixedDeltaTime;
    else
      this.BlockNormalizedTime = Mathf.Max(this.BlockNormalizedTime - 5f * Time.fixedDeltaTime, 0.0f);
  }

  public float LastThrowAngle { get; private set; }

  public float LastThrowV { get; private set; }

  public void EnqueueProjectileThrow(float angle, float v)
  {
    this.LastThrowAngle = angle;
    this.LastThrowV = v;
  }

  public void Attach(IEntity owner)
  {
    this.Owner = owner;
    this.ClearParameters();
    ParametersComponent component = this.Owner.GetComponent<ParametersComponent>();
    if (component == null)
      return;
    this.deadParameter = component.GetByName<bool>(ParameterNameEnum.Dead);
    this.healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
    this.cantBlock = component.GetByName<bool>(ParameterNameEnum.BlockDisabled);
    this.blockStanceParameter = component.GetByName<float>(ParameterNameEnum.Block);
    this.blocktype = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
    this.stamina = component.GetByName<float>(ParameterNameEnum.Stamina);
    this.isPushed = component.GetByName<bool>(ParameterNameEnum.MovementControlBlock);
    this.isFighting = component.GetByName<bool>(ParameterNameEnum.IsFighting);
    this.isCombatIgnored = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
    this.isImmortal = component.GetByName<bool>(ParameterNameEnum.Immortal);
  }

  public void Detach()
  {
    this.Owner = (IEntity) null;
    this.ClearParameters();
  }

  private void ClearParameters()
  {
    this.deadParameter = (IParameter<bool>) null;
    this.healthParameter = (IParameter<float>) null;
    this.cantBlock = (IParameter<bool>) null;
    this.blockStanceParameter = (IParameter<float>) null;
    this.blocktype = (IParameter<BlockTypeEnum>) null;
    this.stamina = (IParameter<float>) null;
    this.isPushed = (IParameter<bool>) null;
    this.isFighting = (IParameter<bool>) null;
    this.isCombatIgnored = (IParameter<bool>) null;
  }
}
