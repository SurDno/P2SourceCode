using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Target weapon On")]
  [TaskCategory("Pathologic/Conversion")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetTransformPosition))]
  public class GetTransformPosition : Action, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public SharedVector3 Result;

    public override void OnStart()
    {
      if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0}: null Target", (object) gameObject.name);
      }
      else
      {
        Vector3 vector3 = Result.Value;
        if (true)
          return;
        Debug.LogWarningFormat("{0}: null Result", (object) gameObject.name);
      }
    }

    public override TaskStatus OnUpdate()
    {
      int num;
      if (!((UnityEngine.Object) Target.Value == (UnityEngine.Object) null))
      {
        Vector3 vector3 = Result.Value;
        num = 0;
      }
      else
        num = 1;
      if (num != 0)
        return TaskStatus.Failure;
      Result.Value = Target.Value.position;
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
