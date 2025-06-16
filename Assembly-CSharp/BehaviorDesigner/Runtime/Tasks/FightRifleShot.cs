using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("атака самопалом")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightRifleShot))]
  public class FightRifleShot : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    private float aimingTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    private float aimingTimeMinimum = 0.5f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    private float attackCooldown;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private float cancelRange = 2f;
    private bool attackInited;
    private bool shotDone;
    private float aimingTimePassed;
    private float cooldownTimePassed;

    public override void OnStart()
    {
      base.OnStart();
      attackInited = false;
      shotDone = false;
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if (owner.Enemy == null)
        return TaskStatus.Failure;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return TaskStatus.Running;
      if (owner.gameObject == null || !owner.gameObject.activeSelf)
        return TaskStatus.Failure;
      Vector3 vector3 = owner.Enemy.transform.position - owner.transform.position;
      if (attackInited)
        aimingTimePassed += deltaTime;
      if (shotDone)
        cooldownTimePassed += deltaTime;
      if ((!attackInited || shotDone && !owner.IsAttacking) && vector3.magnitude < (double) cancelRange)
      {
        owner.SetAiming(false);
        return TaskStatus.Success;
      }
      if (shotDone && !owner.IsAttacking && cooldownTimePassed >= (double) attackCooldown)
      {
        owner.SetAiming(false);
        return TaskStatus.Success;
      }
      if (!attackInited && !owner.IsAttacking && !owner.IsPushing && !owner.IsReacting && !owner.IsQuickBlock && CheckShootingLine())
      {
        attackInited = true;
        aimingTimePassed = 0.0f;
        owner.SetAiming(true);
      }
      if (attackInited && !shotDone && (aimingTimePassed >= (double) aimingTime || vector3.magnitude < (double) cancelRange && aimingTimePassed > (double) aimingTimeMinimum))
      {
        owner.TriggerAction(WeaponActionEnum.RifleFire);
        shotDone = true;
        cooldownTimePassed = 0.0f;
      }
      return TaskStatus.Running;
    }

    private bool CheckShootingLine()
    {
      if (owner.Enemy == null)
        return false;
      Vector3 position = this.gameObject.transform.position;
      Vector3 direction = this.gameObject.transform.forward;
      Pivot component = this.gameObject.GetComponent<Pivot>();
      if (component != null && component.ShootStart != null)
      {
        position = component.ShootStart.transform.position;
        GameObject chest = owner.Enemy.GetComponent<Pivot>()?.Chest;
        if (chest != null)
          direction = (chest.transform.position - component.ShootStart.transform.position).normalized;
      }
      LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
      LayerMask ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
      List<RaycastHit> result = new List<RaycastHit>();
      PhysicsUtility.Raycast(result, position, direction, 50f, -1 ^ triggerInteractLayer ^ ragdollLayer, QueryTriggerInteraction.Ignore);
      for (int index = 0; index < result.Count; ++index)
      {
        GameObject gameObject = result[index].collider.gameObject;
        if (!(gameObject == this.gameObject))
          return gameObject == owner.Enemy.gameObject;
      }
      return false;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.Write(writer, "AimingTime", aimingTime);
      DefaultDataWriteUtility.Write(writer, "AimingTimeMinimum", aimingTimeMinimum);
      DefaultDataWriteUtility.Write(writer, "AttackCooldown", attackCooldown);
      DefaultDataWriteUtility.Write(writer, "CancelRange", cancelRange);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      aimingTime = DefaultDataReadUtility.Read(reader, "AimingTime", aimingTime);
      aimingTimeMinimum = DefaultDataReadUtility.Read(reader, "AimingTimeMinimum", aimingTimeMinimum);
      attackCooldown = DefaultDataReadUtility.Read(reader, "AttackCooldown", attackCooldown);
      cancelRange = DefaultDataReadUtility.Read(reader, "CancelRange", cancelRange);
    }
  }
}
