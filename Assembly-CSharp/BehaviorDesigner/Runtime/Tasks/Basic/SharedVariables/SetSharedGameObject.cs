// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables.SetSharedGameObject
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
  [FactoryProxy(typeof (SetSharedGameObject))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Sets the SharedGameObject variable to the specified object. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSharedGameObject : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the SharedGameObject to. If null the variable will be set to the current GameObject")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject targetValue;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The SharedGameObject to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject targetVariable;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Can the target value be null?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool valueCanBeNull;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "TargetValue", this.targetValue);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "TargetVariable", this.targetVariable);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "ValueCanBeNull", this.valueCanBeNull);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetValue = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "TargetValue", this.targetValue);
      this.targetVariable = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "TargetVariable", this.targetVariable);
      this.valueCanBeNull = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "ValueCanBeNull", this.valueCanBeNull);
    }

    public override TaskStatus OnUpdate()
    {
      this.targetVariable.Value = (UnityEngine.Object) this.targetValue.Value != (UnityEngine.Object) null || this.valueCanBeNull.Value ? this.targetValue.Value : this.gameObject;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.valueCanBeNull = (SharedBool) false;
      this.targetValue = (SharedGameObject) null;
      this.targetVariable = (SharedGameObject) null;
    }
  }
}
