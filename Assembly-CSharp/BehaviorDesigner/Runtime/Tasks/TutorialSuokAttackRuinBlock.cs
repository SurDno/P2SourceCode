using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Tuorial Suok")]
  [TaskDescription("Атаковать только RuinBlock")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (TutorialSuokAttackRuinBlock))]
  public class TutorialSuokAttackRuinBlock : 
    FightBase,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    private float nextAttackTime;

    public override void OnStart()
    {
      base.OnStart();
      nextAttackTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) owner.gameObject == (UnityEngine.Object) null || !owner.gameObject.activeSelf || (UnityEngine.Object) owner?.Enemy == (UnityEngine.Object) null || owner.IsReacting)
        return TaskStatus.Failure;
      if ((double) Time.time < nextAttackTime || (double) (owner.Enemy.transform.position - owner.transform.position).magnitude >= 2.0 || !(owner.Enemy is PlayerEnemy))
        return TaskStatus.Running;
      PlayerEnemy enemy = (PlayerEnemy) owner.Enemy;
      if (owner.IsPushing || owner.IsReacting || owner.IsAttacking)
        return TaskStatus.Running;
      nextAttackTime = Time.time + UnityEngine.Random.Range(4f, 6f);
      owner.TriggerAction(WeaponActionEnum.Uppercut);
      owner.RotationTarget = (Transform) null;
      owner.RotateByPath = false;
      return TaskStatus.Running;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    }
  }
}
