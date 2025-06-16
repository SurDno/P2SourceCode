using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("дождаться, пока не повернулись к врагу")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightTurnToEnemy))]
  public class MeleeFightTurnToEnemy : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public float AngleDelta = 3f;
    private EnemyBase owner;

    public override void OnStart()
    {
      owner = gameObject.GetComponentNonAlloc<EnemyBase>();
      if (!((UnityEngine.Object) owner == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (EnemyBase).Name + " engine component"), (UnityEngine.Object) gameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) owner == (UnityEngine.Object) null || (UnityEngine.Object) owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      Vector3 forward = owner.Enemy.transform.position - transform.position;
      forward = new Vector3(forward.x, 0.0f, forward.z);
      forward.Normalize();
      return (double) Quaternion.Angle(transform.rotation, Quaternion.LookRotation(forward)) < AngleDelta ? TaskStatus.Success : TaskStatus.Running;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.Write(writer, "AngleDelta", AngleDelta);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      AngleDelta = DefaultDataReadUtility.Read(reader, "AngleDelta", AngleDelta);
    }
  }
}
