using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
  [FactoryProxy(typeof (SharedGameObjectToTransform))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Gets the Transform from the GameObject. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SharedGameObjectToTransform : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject to get the Transform of")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedGameObject sharedGameObject;
    [RequiredField]
    [Tooltip("The Transform to set")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform sharedTransform;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SharedGameObject", sharedGameObject);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SharedTransform", sharedTransform);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      sharedGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "SharedGameObject", sharedGameObject);
      sharedTransform = BehaviorTreeDataReadUtility.ReadShared(reader, "SharedTransform", sharedTransform);
    }

    public override TaskStatus OnUpdate()
    {
      if (sharedGameObject.Value == null)
        return TaskStatus.Failure;
      sharedTransform.Value = sharedGameObject.Value.GetComponent<Transform>();
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      sharedGameObject = null;
      sharedTransform = null;
    }
  }
}
