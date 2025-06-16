// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.IsDistanceLess
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is distance to NPC less than?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsDistanceLess))]
  public class IsDistanceLess : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 TargetPosition;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public float Distance;

    public override TaskStatus OnUpdate()
    {
      int num;
      if (this.Target == null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
      {
        if (this.TargetPosition != null)
        {
          Vector3 vector3 = this.TargetPosition.Value;
          num = 0;
        }
        else
          num = 1;
      }
      else
        num = 0;
      if (num != 0)
      {
        Debug.LogWarningFormat("{0}: target is null", (object) this.gameObject.name);
        return TaskStatus.Failure;
      }
      return this.Target != null && (UnityEngine.Object) this.Target.Value != (UnityEngine.Object) null ? ((double) (this.Target.Value.transform.position - this.gameObject.transform.position).magnitude < (double) this.Distance ? TaskStatus.Success : TaskStatus.Failure) : ((double) (this.TargetPosition.Value - this.gameObject.transform.position).magnitude < (double) this.Distance ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "TargetPosition", this.TargetPosition);
      DefaultDataWriteUtility.Write(writer, "Distance", this.Distance);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.TargetPosition = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "TargetPosition", this.TargetPosition);
      this.Distance = DefaultDataReadUtility.Read(reader, "Distance", this.Distance);
    }
  }
}
