using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.BehaviorNodes;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=25")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (POISequence))]
  public class POISequence : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedEntity SearchEntity;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat InSearchRadius;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool FindClosest;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt AvoidLastUsedPoiNumber = 2;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedVector3 OutDestination;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedQuaternion OutRotation;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool Interior;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool SameRegionOnly = true;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool DebugModeOn = false;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedString SearchingArea;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt TotalUnlockedPois;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt PlayablePois;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt PlayablePoisInRadius;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public bool LongAnimationsOnly;
    private POIBase lastUsedPOI;
    private List<POIBase> lastUsedPOIs = new List<POIBase>();
    [HideInInspector]
    public POIBase OutPOI;
    [HideInInspector]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public POIAnimationEnum OutPOIAnimation;
    [HideInInspector]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public int OutAnimationIndex;
    [HideInInspector]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public int OutMiddleAnimationsCount;
    [HideInInspector]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public float OutAngle;
    protected List<POIBase> poiCache = new List<POIBase>();
    private int currentChildIndex;
    protected TaskStatus executionStatus = TaskStatus.Inactive;
    private IRegionComponent region;
    private float randomOffsetRadius = 4f;

    public override int CurrentChildIndex() => currentChildIndex;

    public override bool CanExecute()
    {
      return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      ++currentChildIndex;
      executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      currentChildIndex = childIndex;
      executionStatus = TaskStatus.Inactive;
    }

    protected void FindPOIsInGameObject(GameObject searchObject, List<POIBase> result)
    {
      searchObject.GetComponentsInChildren<POIBase>(result);
    }

    protected void FilterIsChilds(List<POIBase> result)
    {
      int index = 0;
      while (index < result.Count)
      {
        if (result[index].IsChildPOI)
        {
          result[index] = result[result.Count - 1];
          result.RemoveAt(result.Count - 1);
        }
        else
          ++index;
      }
    }

    protected virtual void FillPOICache()
    {
      RecountIndoor();
      poiCache.Clear();
      if (SearchEntity.Entity != null)
      {
        IEntityView entity = (IEntityView) SearchEntity.Entity;
        if ((UnityEngine.Object) entity.GameObject != (UnityEngine.Object) null && entity.GameObject.activeInHierarchy)
          FindPOIsInGameObject(entity.GameObject, poiCache);
      }
      else
        poiCache.AddRange(POIBase.ActivePOIs);
      FilterIsChilds(poiCache);
      FilterIsInterior(poiCache, Interior.Value ? LocationType.Indoor : LocationType.Region);
      if (!LongAnimationsOnly)
        return;
      FilterSmallAnimations(poiCache);
    }

    private void FilterIsInterior(List<POIBase> result, LocationType type)
    {
      int index = 0;
      while (index < result.Count)
      {
        if (result[index].LocationType != type)
        {
          result[index] = result[result.Count - 1];
          result.RemoveAt(result.Count - 1);
        }
        else
          ++index;
      }
    }

    protected void FilterSmallAnimations(List<POIBase> result)
    {
      int index = 0;
      while (index < result.Count)
      {
        POIBase poiBase = result[index];
        if (poiBase.SupportedAnimations == POIAnimationEnum.S_SitOnBench)
          ++index;
        else if (poiBase.SupportedAnimations == POIAnimationEnum.S_SitNearWall)
          ++index;
        else if (poiBase.SupportedAnimations == POIAnimationEnum.S_LeanOnWall)
          ++index;
        else if (poiBase.SupportedAnimations == POIAnimationEnum.S_LeanOnTable)
          ++index;
        else if (poiBase.SupportedAnimations == POIAnimationEnum.S_Loot)
        {
          ++index;
        }
        else
        {
          result[index] = result[result.Count - 1];
          result.RemoveAt(result.Count - 1);
        }
      }
    }

    private void RecountIndoor()
    {
      LocationItemComponent component = EntityUtility.GetEntity(gameObject)?.GetComponent<LocationItemComponent>();
      if (component == null)
        return;
      Interior.Value = component.IsIndoor;
    }

    public override void OnStart()
    {
      if (SearchEntity == null)
        return;
      OutPOI = null;
      if (POIBase.ActivePOIs.Count == 0)
        return;
      POISetup character = gameObject.GetComponentNonAlloc<POISetup>();
      if ((UnityEngine.Object) character == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (gameObject.name + ": doesn't contain POISetup Unity component"));
      }
      else
      {
        FillPOICache();
        IEntity owner = Owner.GetComponent<EngineGameObject>()?.Owner;
        CrowdItemComponent component1 = owner?.GetComponent<CrowdItemComponent>();
        if (component1 != null)
          SearchingArea.Value = component1.Area.ToString();
        else
          SearchingArea = "no area set";
        poiCache = poiCache.FindAll(x => (UnityEngine.Object) x.LockedBy == (UnityEngine.Object) null);
        TotalUnlockedPois = poiCache.Count;
        poiCache = poiCache.FindAll(x => (x.SupportedAnimations & character.SupportedAnimations) != 0);
        PlayablePois = poiCache.Count;
        poiCache = poiCache.FindAll(x => SearchEntity.Entity != null || InSearchRadius.Value == 0.0 || (double) (x.transform.position - gameObject.transform.position).magnitude < InSearchRadius.Value);
        PlayablePoisInRadius = poiCache.Count;
        if (poiCache.Count > 1)
          poiCache = poiCache.FindAll(x => !lastUsedPOIs.Contains(x));
        if (poiCache.Contains(lastUsedPOI))
          poiCache.Remove(lastUsedPOI);
        if (poiCache.Count == 0)
          return;
        if (FindClosest.Value)
        {
          Vector3 searchPosition = gameObject.transform.position + Quaternion.Euler(0.0f, UnityEngine.Random.value * 360f, 0.0f) * (Vector3.forward * randomOffsetRadius);
          poiCache.Sort((c1, c2) => Vector3.Distance(searchPosition, c1.transform.position).CompareTo(Vector3.Distance(searchPosition, c2.transform.position)));
        }
        NavigationComponent component2 = owner?.GetComponent<NavigationComponent>();
        if (component2 != null)
          region = component2.Region;
        int num1 = FindClosest.Value ? 0 : UnityEngine.Random.Range(0, poiCache.Count);
        for (int index1 = 0; index1 < poiCache.Count && (UnityEngine.Object) OutPOI == (UnityEngine.Object) null; ++index1)
        {
          POIBase poiBase = poiCache[(index1 + num1) % poiCache.Count];
          if (!((UnityEngine.Object) poiBase.LockedBy != (UnityEngine.Object) null))
          {
            POIAnimationEnum poiAnimationEnum = poiBase.SupportedAnimations & character.SupportedAnimations;
            if (poiAnimationEnum != 0)
            {
              float magnitude = (poiBase.transform.position - gameObject.transform.position).magnitude;
              if ((poiCache.Count <= AvoidLastUsedPoiNumber.Value || !lastUsedPOIs.Contains(poiBase)) && (InSearchRadius.Value <= 0.0 || magnitude <= (double) InSearchRadius.Value || SearchEntity.Entity != null) && CheckRegion(poiBase.transform.position))
              {
                int num2 = UnityEngine.Random.Range(0, 32);
                for (int index2 = 0; index2 < 32; ++index2)
                {
                  int num3 = (num2 + index2) % 32;
                  if ((poiAnimationEnum & (POIAnimationEnum) (1 << num3)) != 0)
                  {
                    POIAnimationEnum animation = (POIAnimationEnum) (1 << num3);
                    POIAnimationSetupBase animationSetup = character.GetAnimationSetup(animation);
                    OutAnimationIndex = UnityEngine.Random.Range(0, animationSetup.Elements.Count);
                    if (animationSetup.Elements.Count > OutAnimationIndex && animationSetup.Elements[OutAnimationIndex] is POIAnimationSetupElementSlow)
                      OutMiddleAnimationsCount = (animationSetup.Elements[OutAnimationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
                    OutAngle = poiBase.GetAngle(Owner.gameObject);
                    Vector3 targetPosition;
                    Quaternion targetRotation;
                    poiBase.GetRandomTargetPoint(animation, OutAnimationIndex, character, out targetPosition, out targetRotation);
                    OutPOI = poiBase;
                    lastUsedPOI = poiBase;
                    lastUsedPOIs.Add(poiBase);
                    if (lastUsedPOIs.Count > AvoidLastUsedPoiNumber.Value)
                      lastUsedPOIs.RemoveAt(0);
                    OutDestination.Value = targetPosition;
                    OutRotation.Value = targetRotation;
                    OutPOIAnimation = animation;
                    poiBase.Lock(gameObject, animation);
                    break;
                  }
                }
              }
            }
          }
        }
      }
    }

    public override void OnEnd()
    {
      executionStatus = TaskStatus.Inactive;
      currentChildIndex = 0;
      if (!((UnityEngine.Object) OutPOI != (UnityEngine.Object) null))
        return;
      OutPOI.Unlock(gameObject);
    }

    private bool CheckRegion(Vector3 position)
    {
      if (!SameRegionOnly.Value || region == null)
        return true;
      RegionComponent regionByPosition = RegionUtility.GetRegionByPosition(position);
      return regionByPosition != null && region.Region == regionByPosition.Region;
    }

    public void DataWrite(IDataWriter writer)
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
    }

    public void DataRead(IDataReader reader, Type type)
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
    }
  }
}
