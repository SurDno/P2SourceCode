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

[TaskDescription("Enable sanitar flamethrower.")]
[TaskCategory("Pathologic/Sanitar")]
[TaskIcon("Pathologic_SanitarIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (FlamethrowerOn))]
public class FlamethrowerOn : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedTransform Target;
  private PivotSanitar pivot;

  public override TaskStatus OnUpdate()
  {
    if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
    {
      this.pivot = this.gameObject.GetComponent<PivotSanitar>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) ("Doesn't contain " + typeof (PivotSanitar).Name + " component"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
    }
    this.pivot.Flamethrower = true;
    this.pivot.TargetObject = this.Target.Value;
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
  }
}
