using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.BehaviorNodes;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject CallingPOI;

    public override void OnStart()
    {
      this.OutPOI = (POIBase) null;
      POISetup component1 = this.gameObject.GetComponent<POISetup>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain POISetup Unity component"));
      }
      else
      {
        IEntity owner = this.Owner.GetComponent<EngineGameObject>()?.Owner;
        if ((UnityEngine.Object) this.CallingPOI.Value == (UnityEngine.Object) null)
          return;
        POIBase component2 = this.CallingPOI.Value.GetComponent<POIBase>();
        POIAnimationEnum poiAnimationEnum = component2.SupportedAnimations & component1.SupportedAnimations;
        int num1 = UnityEngine.Random.Range(0, 32);
        for (int index = 0; index < 32; ++index)
        {
          int num2 = (num1 + index) % 32;
          if ((poiAnimationEnum & (POIAnimationEnum) (1 << num2)) != 0)
          {
            POIAnimationEnum animation = (POIAnimationEnum) (1 << num2);
            POIAnimationSetupBase animationSetup = component1.GetAnimationSetup(animation);
            this.OutAnimationIndex = UnityEngine.Random.Range(0, animationSetup.Elements.Count);
            if (animationSetup.Elements.Count > this.OutAnimationIndex && animationSetup.Elements[this.OutAnimationIndex] is POIAnimationSetupElementSlow)
              this.OutMiddleAnimationsCount = (animationSetup.Elements[this.OutAnimationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
            this.OutAngle = component2.GetAngle(this.Owner.gameObject);
            Vector3 targetPosition;
            Quaternion targetRotation;
            component2.GetRandomTargetPoint(animation, this.OutAnimationIndex, component1, out targetPosition, out targetRotation);
            this.OutPOI = component2;
            this.OutDestination.Value = targetPosition;
            this.OutRotation.Value = targetRotation;
            this.OutPOIAnimation = animation;
          }
        }
      }
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
      DefaultDataWriteUtility.WriteEnum<AbortType>(writer, "AbortType", this.abortType);
      BehaviorTreeDataWriteUtility.WriteShared<SharedEntity>(writer, "SearchEntity", this.SearchEntity);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "InSearchRadius", this.InSearchRadius);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "FindClosest", this.FindClosest);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "AvoidLastUsedPoiNumber", this.AvoidLastUsedPoiNumber);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "OutDestination", this.OutDestination);
      BehaviorTreeDataWriteUtility.WriteShared<SharedQuaternion>(writer, "OutRotation", this.OutRotation);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Interior", this.Interior);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "SameRegionOnly", this.SameRegionOnly);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "DebugModeOn", this.DebugModeOn);
      BehaviorTreeDataWriteUtility.WriteShared<SharedString>(writer, "SearchingArea", this.SearchingArea);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "TotalUnlockedPois", this.TotalUnlockedPois);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PlayablePois", this.PlayablePois);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PlayablePoisInRadius", this.PlayablePoisInRadius);
      DefaultDataWriteUtility.Write(writer, "LongAnimationsOnly", this.LongAnimationsOnly);
      DefaultDataWriteUtility.WriteEnum<POIAnimationEnum>(writer, "OutPOIAnimation", this.OutPOIAnimation);
      DefaultDataWriteUtility.Write(writer, "OutAnimationIndex", this.OutAnimationIndex);
      DefaultDataWriteUtility.Write(writer, "OutMiddleAnimationsCount", this.OutMiddleAnimationsCount);
      DefaultDataWriteUtility.Write(writer, "OutAngle", this.OutAngle);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "CallingPOI", this.CallingPOI);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
      this.abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
      this.SearchEntity = BehaviorTreeDataReadUtility.ReadShared<SharedEntity>(reader, "SearchEntity", this.SearchEntity);
      this.InSearchRadius = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "InSearchRadius", this.InSearchRadius);
      this.FindClosest = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "FindClosest", this.FindClosest);
      this.AvoidLastUsedPoiNumber = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "AvoidLastUsedPoiNumber", this.AvoidLastUsedPoiNumber);
      this.OutDestination = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "OutDestination", this.OutDestination);
      this.OutRotation = BehaviorTreeDataReadUtility.ReadShared<SharedQuaternion>(reader, "OutRotation", this.OutRotation);
      this.Interior = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Interior", this.Interior);
      this.SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "SameRegionOnly", this.SameRegionOnly);
      this.DebugModeOn = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "DebugModeOn", this.DebugModeOn);
      this.SearchingArea = BehaviorTreeDataReadUtility.ReadShared<SharedString>(reader, "SearchingArea", this.SearchingArea);
      this.TotalUnlockedPois = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "TotalUnlockedPois", this.TotalUnlockedPois);
      this.PlayablePois = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PlayablePois", this.PlayablePois);
      this.PlayablePoisInRadius = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PlayablePoisInRadius", this.PlayablePoisInRadius);
      this.LongAnimationsOnly = DefaultDataReadUtility.Read(reader, "LongAnimationsOnly", this.LongAnimationsOnly);
      this.OutPOIAnimation = DefaultDataReadUtility.ReadEnum<POIAnimationEnum>(reader, "OutPOIAnimation");
      this.OutAnimationIndex = DefaultDataReadUtility.Read(reader, "OutAnimationIndex", this.OutAnimationIndex);
      this.OutMiddleAnimationsCount = DefaultDataReadUtility.Read(reader, "OutMiddleAnimationsCount", this.OutMiddleAnimationsCount);
      this.OutAngle = DefaultDataReadUtility.Read(reader, "OutAngle", this.OutAngle);
      this.CallingPOI = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "CallingPOI", this.CallingPOI);
    }
  }
}
