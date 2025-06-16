using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("POI Idle")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_IdleIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (POIIdle))]
  public class POIIdle : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.CustomTaskReference)]
    [DataWriteProxy(MemberEnum.CustomTaskReference)]
    [CopyableProxy]
    [SerializeField]
    public POISequence ReferencedPOISequence;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat InPOITime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool POILock;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedVector3 POIStartPosition;
    private NpcState npcState;

    public override void OnAwake()
    {
      npcState = gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
        return;
      if (ReferencedPOISequence == null)
      {
        Debug.LogWarning((object) "Poi sequence not set to poi idle!");
      }
      else
      {
        if (!((UnityEngine.Object) ReferencedPOISequence.OutPOI != (UnityEngine.Object) null))
          return;
        npcState.PointOfInterest(InPOITime.Value, ReferencedPOISequence.OutPOI, ReferencedPOISequence.OutPOIAnimation, ReferencedPOISequence.OutAnimationIndex, ReferencedPOISequence.OutMiddleAnimationsCount);
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (npcState.CurrentNpcState != NpcStateEnum.PointOfInterest)
        return TaskStatus.Failure;
      switch (npcState.Status)
      {
        case NpcStateStatusEnum.Success:
          return TaskStatus.Success;
        case NpcStateStatusEnum.Failed:
          return TaskStatus.Failure;
        default:
          return TaskStatus.Running;
      }
    }

    public override void OnEnd()
    {
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskReference(writer, "ReferencedPOISequence", ReferencedPOISequence);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InPOITime", InPOITime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "POILock", POILock);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "POIStartPosition", POIStartPosition);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      ReferencedPOISequence = BehaviorTreeDataReadUtility.ReadTaskReference(reader, "ReferencedPOISequence", ReferencedPOISequence);
      InPOITime = BehaviorTreeDataReadUtility.ReadShared(reader, "InPOITime", InPOITime);
      POILock = BehaviorTreeDataReadUtility.ReadShared(reader, "POILock", POILock);
      POIStartPosition = BehaviorTreeDataReadUtility.ReadShared(reader, "POIStartPosition", POIStartPosition);
    }
  }
}
