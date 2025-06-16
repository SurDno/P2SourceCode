using System;
using System.Collections.Generic;
using System.Reflection;
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
using Scripts.Tools.Serializations.Converters;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is Fraction related?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsFractionRelated))]
  public class IsFractionRelated : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public FractionRelationEnum Relation;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool IsRelated = true;

    public override TaskStatus OnUpdate()
    {
      IEntity entity1 = EntityUtility.GetEntity(gameObject);
      if (entity1 == null)
      {
        Debug.LogWarning((object) (gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      ParametersComponent component = entity1?.GetComponent<ParametersComponent>();
      if (component == null)
        Debug.LogWarningFormat("{0}: doesn't have parameters", (object) gameObject.name);
      IParameter<FractionEnum> fraction = component?.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
      List<FractionEnum> fractionEnumList = null;
      if (fraction != null)
        fractionEnumList = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find(x => x.Name == fraction.Value)?.Relations?.Find(x => x.Relation == Relation)?.Fractions;
      IEntity entity2 = EntityUtility.GetEntity(Target.Value.gameObject);
      if (entity2 == null)
      {
        Debug.LogWarningFormat("{0}: entity not found", (object) Target.Value.gameObject.name);
        return TaskStatus.Failure;
      }
      bool flag = fractionEnumList != null && fractionEnumList.Contains(FractionsHelper.GetTargetFraction(entity2, entity1));
      return IsRelated.Value ? (flag ? TaskStatus.Success : TaskStatus.Failure) : (flag ? TaskStatus.Failure : TaskStatus.Success);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      DefaultDataWriteUtility.WriteEnum(writer, "Relation", Relation);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "IsRelated", IsRelated);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      Relation = DefaultDataReadUtility.ReadEnum<FractionRelationEnum>(reader, "Relation");
      IsRelated = BehaviorTreeDataReadUtility.ReadShared(reader, "IsRelated", IsRelated);
    }
  }
}
