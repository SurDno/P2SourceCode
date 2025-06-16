using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("атака самопалом")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightSamopalAttack))]
  public class FightSamopalAttack : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    private float prepareTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private float aimingTime;
    private bool shotInited;
    private bool shotDone;
    private float prepareTimeLeft;
    private float aimingTimeLeft;

    public override void OnStart()
    {
      base.OnStart();
      prepareTimeLeft = prepareTime;
      shotInited = false;
      shotDone = false;
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if (owner.Enemy == null)
        return TaskStatus.Failure;
      if (shotDone && !owner.IsAttacking)
        return TaskStatus.Success;
      if (owner.IsReacting)
        return TaskStatus.Running;
      if (!owner.IsPushing)
        prepareTimeLeft -= deltaTime;
      if (shotInited)
        aimingTimeLeft -= deltaTime;
      if (!shotInited && prepareTimeLeft <= 0.0 && !owner.IsAttacking && !owner.IsPushing && !owner.IsReacting && !owner.IsQuickBlock)
      {
        owner.TriggerAction(WeaponActionEnum.SamopalAim);
        shotInited = true;
        aimingTimeLeft = aimingTime;
      }
      if (shotInited && !shotDone && aimingTimeLeft < 0.0)
      {
        owner.TriggerAction(WeaponActionEnum.SamopalFire);
        shotDone = true;
      }
      return TaskStatus.Running;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.Write(writer, "PrepareTime", prepareTime);
      DefaultDataWriteUtility.Write(writer, "AimingTime", aimingTime);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      prepareTime = DefaultDataReadUtility.Read(reader, "PrepareTime", prepareTime);
      aimingTime = DefaultDataReadUtility.Read(reader, "AimingTime", aimingTime);
    }
  }
}
