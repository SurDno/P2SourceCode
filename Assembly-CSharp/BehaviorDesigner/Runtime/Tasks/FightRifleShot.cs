using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using System.Collections.Generic;
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float aimingTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float aimingTimeMinimum = 0.5f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float attackCooldown;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float cancelRange = 2f;
    private bool attackInited = false;
    private bool shotDone = false;
    private float aimingTimePassed;
    private float cooldownTimePassed;

    public override void OnStart()
    {
      base.OnStart();
      this.attackInited = false;
      this.shotDone = false;
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return TaskStatus.Running;
      if ((UnityEngine.Object) this.owner.gameObject == (UnityEngine.Object) null || !this.owner.gameObject.activeSelf)
        return TaskStatus.Failure;
      Vector3 vector3 = this.owner.Enemy.transform.position - this.owner.transform.position;
      if (this.attackInited)
        this.aimingTimePassed += deltaTime;
      if (this.shotDone)
        this.cooldownTimePassed += deltaTime;
      if ((!this.attackInited || this.shotDone && !this.owner.IsAttacking) && (double) vector3.magnitude < (double) this.cancelRange)
      {
        this.owner.SetAiming(false);
        return TaskStatus.Success;
      }
      if (this.shotDone && !this.owner.IsAttacking && (double) this.cooldownTimePassed >= (double) this.attackCooldown)
      {
        this.owner.SetAiming(false);
        return TaskStatus.Success;
      }
      if (!this.attackInited && !this.owner.IsAttacking && !this.owner.IsPushing && !this.owner.IsReacting && !this.owner.IsQuickBlock && this.CheckShootingLine())
      {
        this.attackInited = true;
        this.aimingTimePassed = 0.0f;
        this.owner.SetAiming(true);
      }
      if (this.attackInited && !this.shotDone && ((double) this.aimingTimePassed >= (double) this.aimingTime || (double) vector3.magnitude < (double) this.cancelRange && (double) this.aimingTimePassed > (double) this.aimingTimeMinimum))
      {
        this.owner.TriggerAction(WeaponActionEnum.RifleFire);
        this.shotDone = true;
        this.cooldownTimePassed = 0.0f;
      }
      return TaskStatus.Running;
    }

    private bool CheckShootingLine()
    {
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return false;
      Vector3 position = this.gameObject.transform.position;
      Vector3 direction = this.gameObject.transform.forward;
      Pivot component = this.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.ShootStart != (UnityEngine.Object) null)
      {
        position = component.ShootStart.transform.position;
        GameObject chest = this.owner.Enemy.GetComponent<Pivot>()?.Chest;
        if ((UnityEngine.Object) chest != (UnityEngine.Object) null)
          direction = (chest.transform.position - component.ShootStart.transform.position).normalized;
      }
      LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
      LayerMask ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
      List<RaycastHit> result = new List<RaycastHit>();
      PhysicsUtility.Raycast(result, position, direction, 50f, -1 ^ (int) triggerInteractLayer ^ (int) ragdollLayer, QueryTriggerInteraction.Ignore);
      for (int index = 0; index < result.Count; ++index)
      {
        GameObject gameObject = result[index].collider.gameObject;
        if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) this.gameObject))
          return (UnityEngine.Object) gameObject == (UnityEngine.Object) this.owner.Enemy.gameObject;
      }
      return false;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.Write(writer, "AimingTime", this.aimingTime);
      DefaultDataWriteUtility.Write(writer, "AimingTimeMinimum", this.aimingTimeMinimum);
      DefaultDataWriteUtility.Write(writer, "AttackCooldown", this.attackCooldown);
      DefaultDataWriteUtility.Write(writer, "CancelRange", this.cancelRange);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.aimingTime = DefaultDataReadUtility.Read(reader, "AimingTime", this.aimingTime);
      this.aimingTimeMinimum = DefaultDataReadUtility.Read(reader, "AimingTimeMinimum", this.aimingTimeMinimum);
      this.attackCooldown = DefaultDataReadUtility.Read(reader, "AttackCooldown", this.attackCooldown);
      this.cancelRange = DefaultDataReadUtility.Read(reader, "CancelRange", this.cancelRange);
    }
  }
}
