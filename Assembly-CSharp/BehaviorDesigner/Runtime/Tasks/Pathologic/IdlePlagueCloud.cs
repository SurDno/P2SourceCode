using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Idle cloud")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_IdleIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IdlePlagueCloud))]
  public class IdlePlagueCloud : IdleBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    protected override void DoIdle(NpcState state, float primaryIdleProbability)
    {
      state.IdlePlagueCloud();
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "PrimaryIdleProbability", this.primaryIdleProbability);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "IdleTime", this.idleTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RandomIdle", this.randomIdle);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomIdleMin", this.randomIdleMin);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomIdleMax", this.randomIdleMax);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "MakeObstacle", this.MakeObstacle);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.primaryIdleProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "PrimaryIdleProbability", this.primaryIdleProbability);
      this.idleTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "IdleTime", this.idleTime);
      this.randomIdle = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RandomIdle", this.randomIdle);
      this.randomIdleMin = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomIdleMin", this.randomIdleMin);
      this.randomIdleMax = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomIdleMax", this.randomIdleMax);
      this.MakeObstacle = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "MakeObstacle", this.MakeObstacle);
    }
  }
}
