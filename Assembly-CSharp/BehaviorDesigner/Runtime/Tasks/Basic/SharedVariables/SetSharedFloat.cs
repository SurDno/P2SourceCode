// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables.SetSharedFloat
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
  [FactoryProxy(typeof (SetSharedFloat))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Sets the SharedFloat variable to the specified object. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSharedFloat : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the SharedFloat to")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat targetValue;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The SharedFloat to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat targetVariable;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "TargetValue", this.targetValue);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "TargetVariable", this.targetVariable);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetValue = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "TargetValue", this.targetValue);
      this.targetVariable = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "TargetVariable", this.targetVariable);
    }

    public override TaskStatus OnUpdate()
    {
      this.targetVariable.Value = this.targetValue.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.targetValue = (SharedFloat) 0.0f;
      this.targetVariable = (SharedFloat) 0.0f;
    }
  }
}
