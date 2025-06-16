using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsHealthLessThan))]
  public class IsHealthLessThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat HealthThreshold = (SharedFloat) 0.0f;
    private IParameter<float> healthParameter;

    public override void OnAwake()
    {
      IEntity owner = this.gameObject.GetComponent<EngineGameObject>().Owner;
      if (owner == null)
        return;
      ParametersComponent component = owner.GetComponent<ParametersComponent>();
      if (component == null)
        return;
      this.healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
    }

    public override TaskStatus OnUpdate()
    {
      return this.healthParameter == null ? TaskStatus.Failure : ((double) this.healthParameter.Value <= (double) this.HealthThreshold.Value ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "HealthThreshold", this.HealthThreshold);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.HealthThreshold = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "HealthThreshold", this.HealthThreshold);
    }
  }
}
