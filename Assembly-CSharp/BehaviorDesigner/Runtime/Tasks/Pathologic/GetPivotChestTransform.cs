using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get Puppet Master transform")]
  [TaskCategory("Pathologic/Conversion")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetPivotChestTransform))]
  public class GetPivotChestTransform : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public SharedTransform Result;

    public override void OnStart()
    {
      if (!((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null))
        return;
      Debug.LogWarningFormat("{0}: null Target", (object) this.gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      Pivot componentInChildren = this.Target.Value.GetComponentInChildren<Pivot>();
      if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null || (UnityEngine.Object) componentInChildren.Chest == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0}: no puppet master inside", (object) this.gameObject.name);
        return TaskStatus.Failure;
      }
      this.Result.Value = componentInChildren.Chest.transform;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Result", this.Result);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
    }
  }
}
