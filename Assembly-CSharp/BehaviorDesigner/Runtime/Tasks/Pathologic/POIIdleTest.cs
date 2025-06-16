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
  [FactoryProxy(typeof (POIIdleTest))]
  public class POIIdleTest : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat InPOITime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedVector3 POIStartPosition;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform poiObj;
    private NpcState npcState;
    private POIAnimationEnum chosenAnimation;
    private int animationIndex;
    private int currentAnimationIndex;
    private int middleAnimationsCount;

    public override void OnAwake()
    {
      npcState = gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) npcState == (UnityEngine.Object) null || !((UnityEngine.Object) poiObj.Value != (UnityEngine.Object) null))
        return;
      POIBase component1 = poiObj.Value.GetComponent<POIBase>();
      POISetup component2 = gameObject.GetComponent<POISetup>();
      POIAnimationEnum poiAnimationEnum = component2.SupportedAnimations & component1.SupportedAnimations;
      chosenAnimation = POIAnimationEnum.Unknown;
      for (int index = 0; index < 32; ++index)
      {
        int num = index % 32;
        if ((poiAnimationEnum & (POIAnimationEnum) (1 << num)) != 0)
          chosenAnimation = (POIAnimationEnum) (1 << num);
      }
      POIAnimationSetupBase animationSetup = component2.GetAnimationSetup(chosenAnimation);
      currentAnimationIndex = animationIndex;
      middleAnimationsCount = 1;
      if (animationSetup is POIAnimationSetupAngle)
        animationIndex = 1;
      if (animationSetup.Elements.Count > animationIndex && animationSetup.Elements[animationIndex] is POIAnimationSetupElementSlow)
        middleAnimationsCount = (animationSetup.Elements[animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
      npcState.PointOfInterest(InPOITime.Value, component1, chosenAnimation, animationIndex, middleAnimationsCount);
      ++animationIndex;
      if (animationIndex >= animationSetup.Elements.Count)
        animationIndex = 0;
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
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InPOITime", InPOITime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "POIStartPosition", POIStartPosition);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PoiObj", poiObj);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      InPOITime = BehaviorTreeDataReadUtility.ReadShared(reader, "InPOITime", InPOITime);
      POIStartPosition = BehaviorTreeDataReadUtility.ReadShared(reader, "POIStartPosition", POIStartPosition);
      poiObj = BehaviorTreeDataReadUtility.ReadShared(reader, "PoiObj", poiObj);
    }
  }
}
