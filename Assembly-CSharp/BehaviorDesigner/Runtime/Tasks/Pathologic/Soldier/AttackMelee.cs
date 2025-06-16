using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Soldier
{
  [TaskDescription("Soldier melee attack (can be called from idle and attack stance)")]
  [TaskCategory("Pathologic/Soldier")]
  [TaskIcon("Pathologic_SoldierIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (AttackMelee))]
  public class AttackMelee : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat RotationAngularVelocity = 360f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat MeleeAttackDistance = 2f;
    private PivotSoldier pivot;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RotationAngularVelocity", RotationAngularVelocity);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "MeleeAttackDistance", MeleeAttackDistance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      RotationAngularVelocity = BehaviorTreeDataReadUtility.ReadShared(reader, "RotationAngularVelocity", RotationAngularVelocity);
      MeleeAttackDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MeleeAttackDistance", MeleeAttackDistance);
    }
  }
}
