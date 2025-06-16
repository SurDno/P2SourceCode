// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables.SetSharedBool
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
  [FactoryProxy(typeof (SetSharedBool))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Sets the SharedBool variable to the specified object. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSharedBool : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the SharedBool to")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool targetValue;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The SharedBool to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool targetVariable;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "TargetValue", this.targetValue);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "TargetVariable", this.targetVariable);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetValue = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "TargetValue", this.targetValue);
      this.targetVariable = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "TargetVariable", this.targetVariable);
    }

    public override TaskStatus OnUpdate()
    {
      this.targetVariable.Value = this.targetValue.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.targetValue = (SharedBool) false;
      this.targetVariable = (SharedBool) false;
    }
  }
}
