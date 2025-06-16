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
  public class SharedGameObjectToTransform : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject to get the Transform of")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject sharedGameObject;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The Transform to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform sharedTransform;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "SharedGameObject", this.sharedGameObject);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "SharedTransform", this.sharedTransform);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.sharedGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "SharedGameObject", this.sharedGameObject);
      this.sharedTransform = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "SharedTransform", this.sharedTransform);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.sharedGameObject.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.sharedTransform.Value = this.sharedGameObject.Value.GetComponent<Transform>();
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.sharedGameObject = (SharedGameObject) null;
      this.sharedTransform = (SharedTransform) null;
    }
  }
}
