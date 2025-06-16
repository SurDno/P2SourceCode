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
using System;

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
      if ((UnityEngine.Object) this.gameObject.GetComponent<EnemyBase>() == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      CombatStyleEnum styleName = CombatStyleEnum.Default;
      IEntity owner = this.Owner.GetComponent<EngineGameObject>().Owner;
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
      IndividualCombatSettings individualCombatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find((Predicate<IndividualCombatSettings>) (x => x.Name == styleName));
      return individualCombatSettings == null ? TaskStatus.Failure : (individualCombatSettings.CanWatchFight ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
