// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.IsFractionRelated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is Fraction related?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsFractionRelated))]
  public class IsFractionRelated : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public FractionRelationEnum Relation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool IsRelated = (SharedBool) true;

    public override TaskStatus OnUpdate()
    {
      IEntity entity1 = EntityUtility.GetEntity(this.gameObject);
      if (entity1 == null)
      {
        Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      ParametersComponent component = entity1?.GetComponent<ParametersComponent>();
      if (component == null)
        Debug.LogWarningFormat("{0}: doesn't have parameters", (object) this.gameObject.name);
      IParameter<FractionEnum> fraction = component?.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
      List<FractionEnum> fractionEnumList = (List<FractionEnum>) null;
      if (fraction != null)
        fractionEnumList = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find((Predicate<FractionSettings>) (x => x.Name == fraction.Value))?.Relations?.Find((Predicate<FractionRelationGroup>) (x => x.Relation == this.Relation))?.Fractions;
      IEntity entity2 = EntityUtility.GetEntity(this.Target.Value.gameObject);
      if (entity2 == null)
      {
        Debug.LogWarningFormat("{0}: entity not found", (object) this.Target.Value.gameObject.name);
        return TaskStatus.Failure;
      }
      bool flag = fractionEnumList != null && fractionEnumList.Contains(FractionsHelper.GetTargetFraction(entity2, entity1));
      return this.IsRelated.Value ? (flag ? TaskStatus.Success : TaskStatus.Failure) : (flag ? TaskStatus.Failure : TaskStatus.Success);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      DefaultDataWriteUtility.WriteEnum<FractionRelationEnum>(writer, "Relation", this.Relation);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "IsRelated", this.IsRelated);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.Relation = DefaultDataReadUtility.ReadEnum<FractionRelationEnum>(reader, "Relation");
      this.IsRelated = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "IsRelated", this.IsRelated);
    }
  }
}
