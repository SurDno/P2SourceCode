using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsPunchedMoreOrEqualThan))]
  public class IsPunchedMoreOrEqualThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt PunchedMoreOrEqualThan = (SharedInt) 3;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat CalculationTime = (SharedFloat) 5f;
    private NPCEnemy owner;

    public override void OnAwake() => this.owner = this.gameObject.GetComponent<NPCEnemy>();

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) this.owner == (UnityEngine.Object) null ? TaskStatus.Success : (this.owner.GetPunchesCount(this.CalculationTime.Value) >= this.PunchedMoreOrEqualThan.Value ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PunchedMoreOrEqualThan", this.PunchedMoreOrEqualThan);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "CalculationTime", this.CalculationTime);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.PunchedMoreOrEqualThan = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PunchedMoreOrEqualThan", this.PunchedMoreOrEqualThan);
      this.CalculationTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "CalculationTime", this.CalculationTime);
    }
  }
}
