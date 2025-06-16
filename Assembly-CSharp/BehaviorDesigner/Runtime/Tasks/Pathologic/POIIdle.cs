using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("POI Idle")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_IdleIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (POIIdle))]
  public class POIIdle : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.CustomTaskReference)]
    [DataWriteProxy(MemberEnum.CustomTaskReference)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public POISequence ReferencedPOISequence;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat InPOITime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool POILock;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 POIStartPosition;
    private NpcState npcState;

    public override void OnAwake()
    {
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) this.npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        return;
      if (this.ReferencedPOISequence == null)
      {
        Debug.LogWarning((object) "Poi sequence not set to poi idle!");
      }
      else
      {
        if (!((UnityEngine.Object) this.ReferencedPOISequence.OutPOI != (UnityEngine.Object) null))
          return;
        this.npcState.PointOfInterest(this.InPOITime.Value, this.ReferencedPOISequence.OutPOI, this.ReferencedPOISequence.OutPOIAnimation, this.ReferencedPOISequence.OutAnimationIndex, this.ReferencedPOISequence.OutMiddleAnimationsCount);
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (this.npcState.CurrentNpcState != NpcStateEnum.PointOfInterest)
        return TaskStatus.Failure;
      switch (this.npcState.Status)
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
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskReference<POISequence>(writer, "ReferencedPOISequence", this.ReferencedPOISequence);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "InPOITime", this.InPOITime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "POILock", this.POILock);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "POIStartPosition", this.POIStartPosition);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.ReferencedPOISequence = BehaviorTreeDataReadUtility.ReadTaskReference<POISequence>(reader, "ReferencedPOISequence", this.ReferencedPOISequence);
      this.InPOITime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "InPOITime", this.InPOITime);
      this.POILock = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "POILock", this.POILock);
      this.POIStartPosition = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "POIStartPosition", this.POIStartPosition);
    }
  }
}
