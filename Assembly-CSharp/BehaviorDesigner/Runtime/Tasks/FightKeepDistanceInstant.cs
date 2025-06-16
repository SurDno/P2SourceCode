using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Movement")]
  [TaskDescription("Держаться от противника на растоянии")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightKeepDistanceInstant))]
  public class FightKeepDistanceInstant : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat KeepDistanceTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat KeepDistance = 7f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool UseStrafe = false;
    [Tooltip("Надо ли прицеливаться из оружия.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool Aim = false;
    private NpcState npcState;

    public override void OnStart()
    {
      if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) gameObject);
          return;
        }
      }
      npcState.FightKeepDistance(KeepDistance.Value, UseStrafe.Value, Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) npcState == (UnityEngine.Object) null ? TaskStatus.Failure : TaskStatus.Success;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "KeepDistanceTime", KeepDistanceTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "KeepDistance", KeepDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "UseStrafe", UseStrafe);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      KeepDistanceTime = BehaviorTreeDataReadUtility.ReadShared(reader, "KeepDistanceTime", KeepDistanceTime);
      KeepDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "KeepDistance", KeepDistance);
      UseStrafe = BehaviorTreeDataReadUtility.ReadShared(reader, "UseStrafe", UseStrafe);
      Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
    }
  }
}
