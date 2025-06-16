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
  [FactoryProxy(typeof (POIIdleTest))]
  public class POIIdleTest : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat InPOITime;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 POIStartPosition;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform poiObj;
    private NpcState npcState;
    private POIAnimationEnum chosenAnimation;
    private int animationIndex = 0;
    private int currentAnimationIndex = 0;
    private int middleAnimationsCount;

    public override void OnAwake()
    {
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) this.npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null || !((UnityEngine.Object) this.poiObj.Value != (UnityEngine.Object) null))
        return;
      POIBase component1 = this.poiObj.Value.GetComponent<POIBase>();
      POISetup component2 = this.gameObject.GetComponent<POISetup>();
      POIAnimationEnum poiAnimationEnum = component2.SupportedAnimations & component1.SupportedAnimations;
      this.chosenAnimation = POIAnimationEnum.Unknown;
      for (int index = 0; index < 32; ++index)
      {
        int num = index % 32;
        if ((poiAnimationEnum & (POIAnimationEnum) (1 << num)) != 0)
          this.chosenAnimation = (POIAnimationEnum) (1 << num);
      }
      POIAnimationSetupBase animationSetup = component2.GetAnimationSetup(this.chosenAnimation);
      this.currentAnimationIndex = this.animationIndex;
      this.middleAnimationsCount = 1;
      if (animationSetup is POIAnimationSetupAngle)
        this.animationIndex = 1;
      if (animationSetup.Elements.Count > this.animationIndex && animationSetup.Elements[this.animationIndex] is POIAnimationSetupElementSlow)
        this.middleAnimationsCount = (animationSetup.Elements[this.animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
      this.npcState.PointOfInterest(this.InPOITime.Value, component1, this.chosenAnimation, this.animationIndex, this.middleAnimationsCount);
      ++this.animationIndex;
      if (this.animationIndex >= animationSetup.Elements.Count)
        this.animationIndex = 0;
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
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "InPOITime", this.InPOITime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "POIStartPosition", this.POIStartPosition);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "PoiObj", this.poiObj);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.InPOITime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "InPOITime", this.InPOITime);
      this.POIStartPosition = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "POIStartPosition", this.POIStartPosition);
      this.poiObj = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "PoiObj", this.poiObj);
    }
  }
}
