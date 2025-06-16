// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform.GetRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
  [FactoryProxy(typeof (GetRotation))]
  [TaskCategory("Basic/Transform")]
  [TaskDescription("Stores the rotation of the Transform. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class GetRotation : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject targetGameObject;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The rotation of the Transform")]
    [RequiredField]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedQuaternion storeValue;
    private Transform targetTransform;
    private GameObject prevGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "TargetGameObject", this.targetGameObject);
      BehaviorTreeDataWriteUtility.WriteShared<SharedQuaternion>(writer, "StoreValue", this.storeValue);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "TargetGameObject", this.targetGameObject);
      this.storeValue = BehaviorTreeDataReadUtility.ReadShared<SharedQuaternion>(reader, "StoreValue", this.storeValue);
    }

    public override void OnStart()
    {
      GameObject defaultGameObject = this.GetDefaultGameObject(this.targetGameObject.Value);
      if (!((UnityEngine.Object) defaultGameObject != (UnityEngine.Object) this.prevGameObject))
        return;
      this.targetTransform = defaultGameObject.GetComponent<Transform>();
      this.prevGameObject = defaultGameObject;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.targetTransform == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Transform is null");
        return TaskStatus.Failure;
      }
      this.storeValue.Value = this.targetTransform.rotation;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.targetGameObject = (SharedGameObject) null;
      this.storeValue = (SharedQuaternion) Quaternion.identity;
    }
  }
}
