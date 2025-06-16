using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("can fight?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CanWatchFight))]
  public class CanWatchFight : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public override TaskStatus OnUpdate()
    {
      if (gameObject.GetComponent<EnemyBase>() == null)
        return TaskStatus.Failure;
      CombatStyleEnum styleName = CombatStyleEnum.Default;
      IEntity owner = Owner.GetComponent<EngineGameObject>().Owner;
      if (owner != null)
      {
        ParametersComponent component = owner.GetComponent<ParametersComponent>();
        if (component != null)
        {
          IParameter<CombatStyleEnum> byName = component.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle);
          if (byName != null)
            styleName = byName.Value;
        }
      }
      IndividualCombatSettings individualCombatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find(x => x.Name == styleName);
      return individualCombatSettings == null ? TaskStatus.Failure : (individualCombatSettings.CanWatchFight ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    }
  }
}
