using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get Puppet Master transform")]
  [TaskCategory("Pathologic/Conversion")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetPivotChestTransform))]
  public class GetPivotChestTransform : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Result;

    public override void OnStart()
    {
      if (!((UnityEngine.Object) Target.Value == (UnityEngine.Object) null))
        return;
      Debug.LogWarningFormat("{0}: null Target", (object) gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      Pivot componentInChildren = Target.Value.GetComponentInChildren<Pivot>();
      if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null || (UnityEngine.Object) componentInChildren.Chest == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0}: no puppet master inside", (object) gameObject.name);
        return TaskStatus.Failure;
      }
      Result.Value = componentInChildren.Chest.transform;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
    }
  }
}
