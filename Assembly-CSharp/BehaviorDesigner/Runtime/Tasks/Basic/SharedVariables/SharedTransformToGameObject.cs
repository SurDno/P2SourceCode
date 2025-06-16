// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables.SharedTransformToGameObject
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
namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
  [FactoryProxy(typeof (SharedTransformToGameObject))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Gets the GameObject from the Transform component. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SharedTransformToGameObject : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The Transform component")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform sharedTransform;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject sharedGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "SharedTransform", this.sharedTransform);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "SharedGameObject", this.sharedGameObject);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.sharedTransform = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "SharedTransform", this.sharedTransform);
      this.sharedGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "SharedGameObject", this.sharedGameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.sharedTransform.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.sharedGameObject.Value = this.sharedTransform.Value.gameObject;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.sharedTransform = (SharedTransform) null;
      this.sharedGameObject = (SharedGameObject) null;
    }
  }
}
