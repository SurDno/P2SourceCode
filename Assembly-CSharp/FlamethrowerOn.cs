using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

[TaskDescription("Enable sanitar flamethrower.")]
[TaskCategory("Pathologic/Sanitar")]
[TaskIcon("Pathologic_SanitarIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (FlamethrowerOn))]
public class FlamethrowerOn : Action, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public SharedTransform Target;
  private PivotSanitar pivot;

  public override TaskStatus OnUpdate()
  {
    if (pivot == null)
    {
      pivot = gameObject.GetComponent<PivotSanitar>();
      if (pivot == null)
      {
        Debug.LogWarning("Doesn't contain " + typeof (PivotSanitar).Name + " component", gameObject);
        return TaskStatus.Failure;
      }
    }
    pivot.Flamethrower = true;
    pivot.TargetObject = Target.Value;
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
  }
}
