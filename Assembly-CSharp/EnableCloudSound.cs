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
using Action = BehaviorDesigner.Runtime.Tasks.Action;

[TaskDescription("Enable PlagueCloud sound.")]
[TaskCategory("Pathologic/PlagueCloud")]
[TaskIcon("Pathologic_PlagueCloudIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (EnableCloudSound))]
public class EnableCloudSound : Action, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [SerializeField]
  public SharedBool Enabled;
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public SharedFloat MaxDistance = 50f;
  private PivotCloud pivot;

  public override TaskStatus OnUpdate()
  {
    if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
    {
      pivot = gameObject.GetComponent<PivotCloud>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (PivotCloud).Name + " component"), (UnityEngine.Object) gameObject);
        return TaskStatus.Failure;
      }
    }
    pivot.EnableSound(Enabled.Value);
    pivot.SetSoundMaxDistance(MaxDistance.Value);
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Enabled", Enabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "MaxDistance", MaxDistance);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Enabled = BehaviorTreeDataReadUtility.ReadShared(reader, "Enabled", Enabled);
    MaxDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MaxDistance", MaxDistance);
  }
}
