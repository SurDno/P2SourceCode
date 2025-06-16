using System;
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

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CheckParameterValue))]
  public class CheckParameterValue : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat ValueThreshold = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public ParameterNameEnum parName;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public CompareEnum compareType;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool PercentToMax = false;
    private IParameter<float> parameter;
    private IEntity entity;

    public override TaskStatus OnUpdate()
    {
      entity = Target != null && !((UnityEngine.Object) Target.Value == (UnityEngine.Object) null) && !((UnityEngine.Object) Target.Value.gameObject == (UnityEngine.Object) null) ? EntityUtility.GetEntity(Target.Value.gameObject) : EntityUtility.GetEntity(gameObject);
      if (entity == null)
        return TaskStatus.Failure;
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component != null)
        parameter = component.GetByName<float>(parName);
      if (parameter == null)
        return TaskStatus.Failure;
      if (compareType == CompareEnum.More)
        return GetParameterValue() > (double) ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (compareType == CompareEnum.MoreOrEqual)
        return GetParameterValue() >= (double) ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (compareType == CompareEnum.Less)
        return GetParameterValue() < (double) ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (compareType == CompareEnum.LessOrEqual)
        return GetParameterValue() <= (double) ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      return compareType == CompareEnum.Equal ? (GetParameterValue() == (double) ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure) : TaskStatus.Failure;
    }

    private float GetParameterValue()
    {
      return PercentToMax.Value ? parameter.Value / parameter.MaxValue : parameter.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "ValueThreshold", ValueThreshold);
      DefaultDataWriteUtility.WriteEnum(writer, "ParName", parName);
      DefaultDataWriteUtility.WriteEnum(writer, "CompareType", compareType);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PercentToMax", PercentToMax);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      ValueThreshold = BehaviorTreeDataReadUtility.ReadShared(reader, "ValueThreshold", ValueThreshold);
      parName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ParName");
      compareType = DefaultDataReadUtility.ReadEnum<CompareEnum>(reader, "CompareType");
      PercentToMax = BehaviorTreeDataReadUtility.ReadShared(reader, "PercentToMax", PercentToMax);
    }

    public enum CompareEnum
    {
      More,
      MoreOrEqual,
      Less,
      LessOrEqual,
      Equal,
    }
  }
}
