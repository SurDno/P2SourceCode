using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
  [FactoryProxy(typeof (SharedTransformToGameObject))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Gets the GameObject from the Transform component. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SharedTransformToGameObject : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The Transform component")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform sharedTransform;
    [RequiredField]
    [Tooltip("The GameObject to set")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedGameObject sharedGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SharedTransform", sharedTransform);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SharedGameObject", sharedGameObject);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      sharedTransform = BehaviorTreeDataReadUtility.ReadShared(reader, "SharedTransform", sharedTransform);
      sharedGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "SharedGameObject", sharedGameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) sharedTransform.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      sharedGameObject.Value = sharedTransform.Value.gameObject;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      sharedTransform = null;
      sharedGameObject = null;
    }
  }
}
