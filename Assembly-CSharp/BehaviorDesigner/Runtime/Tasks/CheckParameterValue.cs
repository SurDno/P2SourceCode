// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.CheckParameterValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CheckParameterValue))]
  public class CheckParameterValue : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public SharedFloat ValueThreshold = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public ParameterNameEnum parName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public CheckParameterValue.CompareEnum compareType;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool PercentToMax = (SharedBool) false;
    private IParameter<float> parameter;
    private IEntity entity;

    public override TaskStatus OnUpdate()
    {
      this.entity = this.Target != null && !((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null) && !((UnityEngine.Object) this.Target.Value.gameObject == (UnityEngine.Object) null) ? EntityUtility.GetEntity(this.Target.Value.gameObject) : EntityUtility.GetEntity(this.gameObject);
      if (this.entity == null)
        return TaskStatus.Failure;
      ParametersComponent component = this.entity.GetComponent<ParametersComponent>();
      if (component != null)
        this.parameter = component.GetByName<float>(this.parName);
      if (this.parameter == null)
        return TaskStatus.Failure;
      if (this.compareType == CheckParameterValue.CompareEnum.More)
        return (double) this.GetParameterValue() > (double) this.ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (this.compareType == CheckParameterValue.CompareEnum.MoreOrEqual)
        return (double) this.GetParameterValue() >= (double) this.ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (this.compareType == CheckParameterValue.CompareEnum.Less)
        return (double) this.GetParameterValue() < (double) this.ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      if (this.compareType == CheckParameterValue.CompareEnum.LessOrEqual)
        return (double) this.GetParameterValue() <= (double) this.ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
      return this.compareType == CheckParameterValue.CompareEnum.Equal ? ((double) this.GetParameterValue() == (double) this.ValueThreshold.Value ? TaskStatus.Success : TaskStatus.Failure) : TaskStatus.Failure;
    }

    private float GetParameterValue()
    {
      return this.PercentToMax.Value ? this.parameter.Value / this.parameter.MaxValue : this.parameter.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "ValueThreshold", this.ValueThreshold);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "ParName", this.parName);
      DefaultDataWriteUtility.WriteEnum<CheckParameterValue.CompareEnum>(writer, "CompareType", this.compareType);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "PercentToMax", this.PercentToMax);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.ValueThreshold = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "ValueThreshold", this.ValueThreshold);
      this.parName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "ParName");
      this.compareType = DefaultDataReadUtility.ReadEnum<CheckParameterValue.CompareEnum>(reader, "CompareType");
      this.PercentToMax = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "PercentToMax", this.PercentToMax);
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
