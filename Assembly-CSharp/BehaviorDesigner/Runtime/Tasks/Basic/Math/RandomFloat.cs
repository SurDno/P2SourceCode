using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
  [FactoryProxy(typeof (RandomFloat))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Sets a random float value")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RandomFloat : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The minimum amount")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat min;
    [Tooltip("The maximum amount")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat max;
    [Tooltip("Is the maximum value inclusive?")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public bool inclusive;
    [Tooltip("The variable to store the result")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat storeResult;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Min", min);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Max", max);
      DefaultDataWriteUtility.Write(writer, "Inclusive", inclusive);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StoreResult", storeResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      min = BehaviorTreeDataReadUtility.ReadShared(reader, "Min", min);
      max = BehaviorTreeDataReadUtility.ReadShared(reader, "Max", max);
      inclusive = DefaultDataReadUtility.Read(reader, "Inclusive", inclusive);
      storeResult = BehaviorTreeDataReadUtility.ReadShared(reader, "StoreResult", storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      if (inclusive)
        storeResult.Value = UnityEngine.Random.Range(min.Value, max.Value + 1f);
      else
        storeResult.Value = UnityEngine.Random.Range(min.Value, max.Value);
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      min.Value = 0.0f;
      max.Value = 0.0f;
      inclusive = false;
      storeResult.Value = 0.0f;
    }
  }
}
