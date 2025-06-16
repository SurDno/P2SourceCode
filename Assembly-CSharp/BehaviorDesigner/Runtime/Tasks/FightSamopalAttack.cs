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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float prepareTime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private float aimingTime;
    private bool shotInited = false;
    private bool shotDone = false;
    private float prepareTimeLeft;
    private float aimingTimeLeft;

    public override void OnStart()
    {
      base.OnStart();
      this.prepareTimeLeft = this.prepareTime;
      this.shotInited = false;
      this.shotDone = false;
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (this.shotDone && !this.owner.IsAttacking)
        return TaskStatus.Success;
      if (this.owner.IsReacting)
        return TaskStatus.Running;
      if (!this.owner.IsPushing)
        this.prepareTimeLeft -= deltaTime;
      if (this.shotInited)
        this.aimingTimeLeft -= deltaTime;
      if (!this.shotInited && (double) this.prepareTimeLeft <= 0.0 && !this.owner.IsAttacking && !this.owner.IsPushing && !this.owner.IsReacting && !this.owner.IsQuickBlock)
      {
        this.owner.TriggerAction(WeaponActionEnum.SamopalAim);
        this.shotInited = true;
        this.aimingTimeLeft = this.aimingTime;
      }
      if (this.shotInited && !this.shotDone && (double) this.aimingTimeLeft < 0.0)
      {
        this.owner.TriggerAction(WeaponActionEnum.SamopalFire);
        this.shotDone = true;
      }
      return TaskStatus.Running;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.Write(writer, "PrepareTime", this.prepareTime);
      DefaultDataWriteUtility.Write(writer, "AimingTime", this.aimingTime);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.prepareTime = DefaultDataReadUtility.Read(reader, "PrepareTime", this.prepareTime);
      this.aimingTime = DefaultDataReadUtility.Read(reader, "AimingTime", this.aimingTime);
    }
  }
}
