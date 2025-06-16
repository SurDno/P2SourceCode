// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables.CompareSharedTransform
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
  [FactoryProxy(typeof (CompareSharedTransform))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CompareSharedTransform : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The first variable to compare")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform variable;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to compare to")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform compareTo;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Variable", this.variable);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "CompareTo", this.compareTo);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.variable = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Variable", this.variable);
      this.compareTo = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "CompareTo", this.compareTo);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.variable.Value == (UnityEngine.Object) null && (UnityEngine.Object) this.compareTo.Value != (UnityEngine.Object) null)
        return TaskStatus.Failure;
      return (UnityEngine.Object) this.variable.Value == (UnityEngine.Object) null && (UnityEngine.Object) this.compareTo.Value == (UnityEngine.Object) null ? TaskStatus.Success : (((object) this.variable.Value).Equals((object) this.compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure);
    }

    public override void OnReset()
    {
      this.variable = (SharedTransform) null;
      this.compareTo = (SharedTransform) null;
    }
  }
}
