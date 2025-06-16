using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=25")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (POISequenceGroupAnswer))]
  public class POISequenceGroupAnswer : POISequence, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedGameObject CallingPOI;

    public override void OnStart()
    {
      OutPOI = null;
      POISetup component1 = gameObject.GetComponent<POISetup>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (gameObject.name + ": doesn't contain POISetup Unity component"));
      }
      else
      {
        IEntity owner = Owner.GetComponent<EngineGameObject>()?.Owner;
        if ((UnityEngine.Object) CallingPOI.Value == (UnityEngine.Object) null)
          return;
        POIBase component2 = CallingPOI.Value.GetComponent<POIBase>();
        POIAnimationEnum poiAnimationEnum = component2.SupportedAnimations & component1.SupportedAnimations;
        int num1 = UnityEngine.Random.Range(0, 32);
        for (int index = 0; index < 32; ++index)
        {
          int num2 = (num1 + index) % 32;
          if ((poiAnimationEnum & (POIAnimationEnum) (1 << num2)) != 0)
          {
            POIAnimationEnum animation = (POIAnimationEnum) (1 << num2);
            POIAnimationSetupBase animationSetup = component1.GetAnimationSetup(animation);
            OutAnimationIndex = UnityEngine.Random.Range(0, animationSetup.Elements.Count);
            if (animationSetup.Elements.Count > OutAnimationIndex && animationSetup.Elements[OutAnimationIndex] is POIAnimationSetupElementSlow)
              OutMiddleAnimationsCount = (animationSetup.Elements[OutAnimationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
            OutAngle = component2.GetAngle(Owner.gameObject);
            Vector3 targetPosition;
            Quaternion targetRotation;
            component2.GetRandomTargetPoint(animation, OutAnimationIndex, component1, out targetPosition, out targetRotation);
            OutPOI = component2;
            OutDestination.Value = targetPosition;
            OutRotation.Value = targetRotation;
            OutPOIAnimation = animation;
          }
        }
      }
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
      DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchEntity", SearchEntity);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InSearchRadius", InSearchRadius);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "FindClosest", FindClosest);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "AvoidLastUsedPoiNumber", AvoidLastUsedPoiNumber);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "OutDestination", OutDestination);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "OutRotation", OutRotation);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Interior", Interior);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SameRegionOnly", SameRegionOnly);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "DebugModeOn", DebugModeOn);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchingArea", SearchingArea);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TotalUnlockedPois", TotalUnlockedPois);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PlayablePois", PlayablePois);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PlayablePoisInRadius", PlayablePoisInRadius);
      DefaultDataWriteUtility.Write(writer, "LongAnimationsOnly", LongAnimationsOnly);
      DefaultDataWriteUtility.WriteEnum(writer, "OutPOIAnimation", OutPOIAnimation);
      DefaultDataWriteUtility.Write(writer, "OutAnimationIndex", OutAnimationIndex);
      DefaultDataWriteUtility.Write(writer, "OutMiddleAnimationsCount", OutMiddleAnimationsCount);
      DefaultDataWriteUtility.Write(writer, "OutAngle", OutAngle);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "CallingPOI", CallingPOI);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
      abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
      SearchEntity = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchEntity", SearchEntity);
      InSearchRadius = BehaviorTreeDataReadUtility.ReadShared(reader, "InSearchRadius", InSearchRadius);
      FindClosest = BehaviorTreeDataReadUtility.ReadShared(reader, "FindClosest", FindClosest);
      AvoidLastUsedPoiNumber = BehaviorTreeDataReadUtility.ReadShared(reader, "AvoidLastUsedPoiNumber", AvoidLastUsedPoiNumber);
      OutDestination = BehaviorTreeDataReadUtility.ReadShared(reader, "OutDestination", OutDestination);
      OutRotation = BehaviorTreeDataReadUtility.ReadShared(reader, "OutRotation", OutRotation);
      Interior = BehaviorTreeDataReadUtility.ReadShared(reader, "Interior", Interior);
      SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared(reader, "SameRegionOnly", SameRegionOnly);
      DebugModeOn = BehaviorTreeDataReadUtility.ReadShared(reader, "DebugModeOn", DebugModeOn);
      SearchingArea = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchingArea", SearchingArea);
      TotalUnlockedPois = BehaviorTreeDataReadUtility.ReadShared(reader, "TotalUnlockedPois", TotalUnlockedPois);
      PlayablePois = BehaviorTreeDataReadUtility.ReadShared(reader, "PlayablePois", PlayablePois);
      PlayablePoisInRadius = BehaviorTreeDataReadUtility.ReadShared(reader, "PlayablePoisInRadius", PlayablePoisInRadius);
      LongAnimationsOnly = DefaultDataReadUtility.Read(reader, "LongAnimationsOnly", LongAnimationsOnly);
      OutPOIAnimation = DefaultDataReadUtility.ReadEnum<POIAnimationEnum>(reader, "OutPOIAnimation");
      OutAnimationIndex = DefaultDataReadUtility.Read(reader, "OutAnimationIndex", OutAnimationIndex);
      OutMiddleAnimationsCount = DefaultDataReadUtility.Read(reader, "OutMiddleAnimationsCount", OutMiddleAnimationsCount);
      OutAngle = DefaultDataReadUtility.Read(reader, "OutAngle", OutAngle);
      CallingPOI = BehaviorTreeDataReadUtility.ReadShared(reader, "CallingPOI", CallingPOI);
    }
  }
}
