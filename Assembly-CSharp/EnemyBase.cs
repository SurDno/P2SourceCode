using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
  private float blockNormalizedTime;
  private IParameter<float> blockStanceParameter;
  private bool blockStance;
  private IParameter<BlockTypeEnum> blocktype;
  private IParameter<float> stamina;
  private IParameter<bool> isPushed;
  private bool isFaint;
  private IParameter<bool> isFighting;
  private IParameter<bool> isCombatIgnored;
  private IParameter<bool> isImmortal;
  protected float currentWalkSpeed;
  protected float desiredWalkSpeed;
  private float? retreatAngle;
  private bool rotateByPath;
  private Transform rotationTarget;
  private EnemyBase enemy;
  [Inspected]
  private HashSet<EnemyBase> attackers = new HashSet<EnemyBase>();

  public event Action<EnemyBase> WasPrepunchEvent;

  public void FireWasWasPrepunchEvent()
  {
    Action<EnemyBase> wasPrepunchEvent = WasPrepunchEvent;
    if (wasPrepunchEvent == null)
      return;
    wasPrepunchEvent(this);
  }

  public event Action<EnemyBase> WasPunchedEvent;

  public void FireWasPunchedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasPunchedEvent = WasPunchedEvent;
    if (wasPunchedEvent == null)
      return;
    wasPunchedEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToSurrenderEvent;

  public void FireWasPunchedToSurrenderEvent(EnemyBase enemy)
  {
    Action<EnemyBase> toSurrenderEvent = WasPunchedToSurrenderEvent;
    if (toSurrenderEvent == null)
      return;
    toSurrenderEvent(enemy);
  }

  public event Action<EnemyBase> WasLowStaminaPunchedEvent;

  public void FireWasLowStaminaPunchedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> staminaPunchedEvent = WasLowStaminaPunchedEvent;
    if (staminaPunchedEvent == null)
      return;
    staminaPunchedEvent(enemy);
  }

  public event Action<EnemyBase> WasStaggeredEvent;

  public void FireWasStaggeredEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasStaggeredEvent = WasStaggeredEvent;
    if (wasStaggeredEvent == null)
      return;
    wasStaggeredEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToBlockEvent;

  public void FireWasPunchedToBlockEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToBlockEvent = WasPunchedToBlockEvent;
    if (punchedToBlockEvent == null)
      return;
    punchedToBlockEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToQuickBlock;

  public void FireWasPunchedToQuickBlockEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToQuickBlock = WasPunchedToQuickBlock;
    if (punchedToQuickBlock == null)
      return;
    punchedToQuickBlock(enemy);
  }

  public event Action<EnemyBase> WasPunchedToDodgeEvent;

  public void FireWasPunchedToDodgeEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToDodgeEvent = WasPunchedToDodgeEvent;
    if (punchedToDodgeEvent == null)
      return;
    punchedToDodgeEvent(enemy);
  }

  public event Action<EnemyBase> WasPunchedToStaggerEvent;

  public void FireWasPunchedToStaggerEvent(EnemyBase enemy)
  {
    Action<EnemyBase> punchedToStaggerEvent = WasPunchedToStaggerEvent;
    if (punchedToStaggerEvent == null)
      return;
    punchedToStaggerEvent(enemy);
  }

  public event Action<EnemyBase> WasPushedEvent;

  public void FireWasPushedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> wasPushedEvent = WasPushedEvent;
    if (wasPushedEvent == null)
      return;
    wasPushedEvent(enemy);
  }

  public event Action<EnemyBase> WasKnockDownedEvent;

  public void FireWasKnockDownedEvent(EnemyBase enemy)
  {
    Action<EnemyBase> knockDownedEvent = WasKnockDownedEvent;
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
    Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum> punchEvent = PunchEvent;
    if (punchEvent == null)
      return;
    punchEvent(weaponEntity, punch, reaction, weapon, subtype);
  }

  public event Action<WeaponEnum> PunchHitEvent;

  public void FirePunchHitEvent(WeaponEnum weapon)
  {
    Action<WeaponEnum> punchHitEvent = PunchHitEvent;
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
    if (attackers.Count < 2)
      return Vector3.zero;
    float num1 = 0.5f;
    float num2 = 2f;
    foreach (EnemyBase attacker1 in attackers)
    {
      if (!(attacker1 == attacker))
      {
        Vector3 vector3 = attacker.transform.position - attacker1.transform.position;
        float num3 = num1 * num1 / vector3.sqrMagnitude;
        if (num3 >= 0.15999999642372131)
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
    Vector3 normalized = (position - transform.position).normalized;
    float y = Vector3.Cross(normalized, transform.forward).y;
    if (Vector3.Dot(normalized, transform.forward) > 0.0)
    {
      if (Mathf.Abs(y) < (double) Mathf.Sin(0.2617994f))
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
        bool flag = y < 0.0;
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
    if (!(pivot != null) || !(pivot.SoundBank != null))
      return;
    IEntity entity = EntityUtility.GetEntity(gameObject);
    if (entity != null)
    {
      LipSyncComponent component = entity.GetComponent<LipSyncComponent>();
      NPCSoundBankCrySettings soundBankCrySettings = pivot.SoundBank.CombatCries.Find(x => x.Name == cryEnum);
      if (component != null && soundBankCrySettings != null && soundBankCrySettings.Chance > (double) Random.value)
      {
        LipSyncObjectSerializable description = soundBankCrySettings.Description;
        component.Play3D(description.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.AudioSourceForNpcCombatReplics, true);
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
    Enemy = null;
    ServiceLocator.GetService<CombatService>()?.UnregisterCharacter(this);
  }

  public void RegisterAttacker(EnemyBase enemy)
  {
    if (attackers.Add(enemy))
      return;
    Debug.LogWarning("RegisterAttacker " + gameObject.name + " already cotains " + enemy.gameObject.name);
  }

  public void UnregisterAttacker(EnemyBase enemy)
  {
    if (attackers.Remove(enemy))
      return;
    Debug.LogWarning("UnregisterAttacker " + gameObject.name + " doesn't cotain " + enemy.gameObject.name);
  }

  [Inspected]
  public EnemyBase Enemy
  {
    get => enemy;
    set
    {
      enemy?.UnregisterAttacker(this);
      enemy = value;
      enemy?.RegisterAttacker(this);
    }
  }

  public bool IsDead => deadParameter != null && deadParameter.Value;

  private IParameter<float> Health => healthParameter;

  public IEntity Owner { get; private set; }

  public bool CantBlock
  {
    get => cantBlock != null && cantBlock.Value;
    set
    {
      if (cantBlock == null)
        return;
      cantBlock.Value = value;
    }
  }

  public float BlockNormalizedTime
  {
    get => blockNormalizedTime;
    set => blockNormalizedTime = value;
  }

  public virtual bool BlockStance
  {
    get => blockStance;
    set
    {
      blockStance = value;
      if (blockStanceParameter == null)
        return;
      blockStanceParameter.Value = blockStance ? 1f : 0.0f;
    }
  }

  public BlockTypeEnum BlockType
  {
    get => blocktype == null ? BlockTypeEnum.None : blocktype.Value;
    set
    {
      if (blocktype == null)
        return;
      blocktype.Value = value;
    }
  }

  public float Stamina
  {
    get => stamina == null ? 0.0f : stamina.Value;
    set
    {
      if (stamina == null)
        return;
      stamina.Value = value;
    }
  }

  public bool IsPushed
  {
    get => isPushed != null && isPushed.Value;
    set
    {
      if (isPushed == null)
        return;
      isPushed.Value = value;
    }
  }

  public bool IsFighting
  {
    get => isFighting != null && isFighting.Value;
    set
    {
      if (isFighting == null)
        return;
      isFighting.Value = value;
    }
  }

  public bool IsCombatIgnored
  {
    get => isCombatIgnored != null && isCombatIgnored.Value;
    set
    {
      if (isCombatIgnored == null)
        return;
      isCombatIgnored.Value = value;
    }
  }

  public bool IsImmortal
  {
    get => isImmortal != null && isImmortal.Value;
    set
    {
      if (isImmortal == null)
        return;
      isImmortal.Value = value;
    }
  }

  public bool IsFaint
  {
    get => isFaint;
    set => isFaint = value;
  }

  public Transform RotationTarget
  {
    get => rotationTarget;
    set => rotationTarget = value;
  }

  public bool RotateByPath
  {
    get => rotateByPath;
    set => rotateByPath = value;
  }

  public float? RetreatAngle
  {
    get => retreatAngle;
    set => retreatAngle = value;
  }

  [Inspected]
  public float DesiredWalkSpeed
  {
    get => desiredWalkSpeed;
    set => desiredWalkSpeed = value;
  }

  [Inspected]
  public float CurrentWalkSpeed
  {
    get => currentWalkSpeed;
    set => currentWalkSpeed = value;
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
    if (BlockType == BlockTypeEnum.Block)
      BlockNormalizedTime += Time.fixedDeltaTime;
    else
      BlockNormalizedTime = Mathf.Max(BlockNormalizedTime - 5f * Time.fixedDeltaTime, 0.0f);
  }

  public float LastThrowAngle { get; private set; }

  public float LastThrowV { get; private set; }

  public void EnqueueProjectileThrow(float angle, float v)
  {
    LastThrowAngle = angle;
    LastThrowV = v;
  }

  public void Attach(IEntity owner)
  {
    Owner = owner;
    ClearParameters();
    ParametersComponent component = Owner.GetComponent<ParametersComponent>();
    if (component == null)
      return;
    deadParameter = component.GetByName<bool>(ParameterNameEnum.Dead);
    healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
    cantBlock = component.GetByName<bool>(ParameterNameEnum.BlockDisabled);
    blockStanceParameter = component.GetByName<float>(ParameterNameEnum.Block);
    blocktype = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
    stamina = component.GetByName<float>(ParameterNameEnum.Stamina);
    isPushed = component.GetByName<bool>(ParameterNameEnum.MovementControlBlock);
    isFighting = component.GetByName<bool>(ParameterNameEnum.IsFighting);
    isCombatIgnored = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
    isImmortal = component.GetByName<bool>(ParameterNameEnum.Immortal);
  }

  public void Detach()
  {
    Owner = null;
    ClearParameters();
  }

  private void ClearParameters()
  {
    deadParameter = null;
    healthParameter = null;
    cantBlock = null;
    blockStanceParameter = null;
    blocktype = null;
    stamina = null;
    isPushed = null;
    isFighting = null;
    isCombatIgnored = null;
  }
}
