using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Назначить врага")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightSetEnemy))]
  public class MeleeFightSetEnemy : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform EnemyTransform;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool UseCombatService;
    private Pivot pivot;
    private EnemyBase owner;
    private Animator animator;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        pivot = gameObject.GetComponentNonAlloc<Pivot>();
        if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (Pivot).Name + " engine component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
        owner = pivot.GetNpcEnemy();
        if ((UnityEngine.Object) owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (EnemyBase).Name + " engine component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
        animator = pivot.GetAnimator();
        if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (Animator).Name + " engine component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
      }
      if (EnemyTransform == null || (UnityEngine.Object) EnemyTransform.Value == (UnityEngine.Object) null || (UnityEngine.Object) EnemyTransform.Value.GetComponentNonAlloc<EnemyBase>() == (UnityEngine.Object) null)
      {
        if (!UseCombatService.Value)
          owner.Enemy = null;
        owner.RotationTarget = (Transform) null;
        owner.RetreatAngle = new float?();
        owner.DesiredWalkSpeed = 0.0f;
        animator?.SetTrigger("Fight.Triggers/CancelWalk");
        animator?.SetTrigger("Fight.Triggers/CancelAttack");
        return TaskStatus.Success;
      }
      if (!UseCombatService.Value)
        owner.Enemy = EnemyTransform.Value.GetComponentNonAlloc<EnemyBase>();
      owner.RotationTarget = EnemyTransform.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EnemyTransform", EnemyTransform);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "UseCombatService", UseCombatService);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      EnemyTransform = BehaviorTreeDataReadUtility.ReadShared(reader, "EnemyTransform", EnemyTransform);
      UseCombatService = BehaviorTreeDataReadUtility.ReadShared(reader, "UseCombatService", UseCombatService);
    }
  }
}
