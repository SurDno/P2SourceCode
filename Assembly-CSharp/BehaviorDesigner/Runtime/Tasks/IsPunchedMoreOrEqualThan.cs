using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsPunchedMoreOrEqualThan))]
  public class IsPunchedMoreOrEqualThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt PunchedMoreOrEqualThan = 3;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat CalculationTime = 5f;
    private NPCEnemy owner;

    public override void OnAwake() => owner = gameObject.GetComponent<NPCEnemy>();

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) owner == (UnityEngine.Object) null ? TaskStatus.Success : (owner.GetPunchesCount(CalculationTime.Value) >= PunchedMoreOrEqualThan.Value ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PunchedMoreOrEqualThan", PunchedMoreOrEqualThan);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "CalculationTime", CalculationTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      PunchedMoreOrEqualThan = BehaviorTreeDataReadUtility.ReadShared(reader, "PunchedMoreOrEqualThan", PunchedMoreOrEqualThan);
      CalculationTime = BehaviorTreeDataReadUtility.ReadShared(reader, "CalculationTime", CalculationTime);
    }
  }
}
