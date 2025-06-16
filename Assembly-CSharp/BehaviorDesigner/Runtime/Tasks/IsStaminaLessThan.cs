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
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsStaminaLessThan))]
  public class IsStaminaLessThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat StaminaThreshold = 0.0f;
    private IParameter<float> staminaParameter;

    public override void OnAwake()
    {
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      EngineGameObject component1 = gameObject.GetComponent<EngineGameObject>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
        return;
      IEntity owner = component1.Owner;
      if (owner == null)
        return;
      ParametersComponent component2 = owner.GetComponent<ParametersComponent>();
      if (component2 == null)
        return;
      staminaParameter = component2.GetByName<float>(ParameterNameEnum.Stamina);
    }

    public override TaskStatus OnUpdate()
    {
      return staminaParameter == null ? TaskStatus.Failure : (staminaParameter.Value <= (double) StaminaThreshold.Value ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StaminaThreshold", StaminaThreshold);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      StaminaThreshold = BehaviorTreeDataReadUtility.ReadShared(reader, "StaminaThreshold", StaminaThreshold);
    }
  }
}
