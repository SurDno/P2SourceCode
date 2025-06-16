// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.POISequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedEntity SearchEntity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat InSearchRadius;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool FindClosest;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt AvoidLastUsedPoiNumber = (SharedInt) 2;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 OutDestination;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedQuaternion OutRotation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Interior;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool SameRegionOnly = (SharedBool) true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool DebugModeOn = (SharedBool) false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedString SearchingArea;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt TotalUnlockedPois;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt PlayablePois;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt PlayablePoisInRadius;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public bool LongAnimationsOnly = false;
    private POIBase lastUsedPOI;
    private List<POIBase> lastUsedPOIs = new List<POIBase>();
    [HideInInspector]
    public POIBase OutPOI;
    [HideInInspector]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public POIAnimationEnum OutPOIAnimation;
    [HideInInspector]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public int OutAnimationIndex;
    [HideInInspector]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public int OutMiddleAnimationsCount;
    [HideInInspector]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public float OutAngle;
    protected List<POIBase> poiCache = new List<POIBase>();
    private int currentChildIndex = 0;
    protected TaskStatus executionStatus = TaskStatus.Inactive;
    private IRegionComponent region;
    private float randomOffsetRadius = 4f;

    public override int CurrentChildIndex() => this.currentChildIndex;

    public override bool CanExecute()
    {
      return this.currentChildIndex < this.children.Count && this.executionStatus != TaskStatus.Failure;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      ++this.currentChildIndex;
      this.executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      this.currentChildIndex = childIndex;
      this.executionStatus = TaskStatus.Inactive;
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
      this.RecountIndoor();
      this.poiCache.Clear();
      if (this.SearchEntity.Entity != null)
      {
        IEntityView entity = (IEntityView) this.SearchEntity.Entity;
        if ((UnityEngine.Object) entity.GameObject != (UnityEngine.Object) null && entity.GameObject.activeInHierarchy)
          this.FindPOIsInGameObject(entity.GameObject, this.poiCache);
      }
      else
        this.poiCache.AddRange((IEnumerable<POIBase>) POIBase.ActivePOIs);
      this.FilterIsChilds(this.poiCache);
      this.FilterIsInterior(this.poiCache, this.Interior.Value ? LocationType.Indoor : LocationType.Region);
      if (!this.LongAnimationsOnly)
        return;
      this.FilterSmallAnimations(this.poiCache);
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
      LocationItemComponent component = EntityUtility.GetEntity(this.gameObject)?.GetComponent<LocationItemComponent>();
      if (component == null)
        return;
      this.Interior.Value = component.IsIndoor;
    }

    public override void OnStart()
    {
      if (this.SearchEntity == null)
        return;
      this.OutPOI = (POIBase) null;
      if (POIBase.ActivePOIs.Count == 0)
        return;
      POISetup character = this.gameObject.GetComponentNonAlloc<POISetup>();
      if ((UnityEngine.Object) character == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain POISetup Unity component"));
      }
      else
      {
        this.FillPOICache();
        IEntity owner = this.Owner.GetComponent<EngineGameObject>()?.Owner;
        CrowdItemComponent component1 = owner?.GetComponent<CrowdItemComponent>();
        if (component1 != null)
          this.SearchingArea.Value = component1.Area.ToString();
        else
          this.SearchingArea = (SharedString) "no area set";
        this.poiCache = this.poiCache.FindAll((Predicate<POIBase>) (x => (UnityEngine.Object) x.LockedBy == (UnityEngine.Object) null));
        this.TotalUnlockedPois = (SharedInt) this.poiCache.Count;
        this.poiCache = this.poiCache.FindAll((Predicate<POIBase>) (x => (x.SupportedAnimations & character.SupportedAnimations) != 0));
        this.PlayablePois = (SharedInt) this.poiCache.Count;
        this.poiCache = this.poiCache.FindAll((Predicate<POIBase>) (x => this.SearchEntity.Entity != null || (double) this.InSearchRadius.Value == 0.0 || (double) (x.transform.position - this.gameObject.transform.position).magnitude < (double) this.InSearchRadius.Value));
        this.PlayablePoisInRadius = (SharedInt) this.poiCache.Count;
        if (this.poiCache.Count > 1)
          this.poiCache = this.poiCache.FindAll((Predicate<POIBase>) (x => !this.lastUsedPOIs.Contains(x)));
        if (this.poiCache.Contains(this.lastUsedPOI))
          this.poiCache.Remove(this.lastUsedPOI);
        if (this.poiCache.Count == 0)
          return;
        if (this.FindClosest.Value)
        {
          Vector3 searchPosition = this.gameObject.transform.position + Quaternion.Euler(0.0f, UnityEngine.Random.value * 360f, 0.0f) * (Vector3.forward * this.randomOffsetRadius);
          this.poiCache.Sort((Comparison<POIBase>) ((c1, c2) => Vector3.Distance(searchPosition, c1.transform.position).CompareTo(Vector3.Distance(searchPosition, c2.transform.position))));
        }
        NavigationComponent component2 = owner?.GetComponent<NavigationComponent>();
        if (component2 != null)
          this.region = component2.Region;
        int num1 = this.FindClosest.Value ? 0 : UnityEngine.Random.Range(0, this.poiCache.Count);
        for (int index1 = 0; index1 < this.poiCache.Count && (UnityEngine.Object) this.OutPOI == (UnityEngine.Object) null; ++index1)
        {
          POIBase poiBase = this.poiCache[(index1 + num1) % this.poiCache.Count];
          if (!((UnityEngine.Object) poiBase.LockedBy != (UnityEngine.Object) null))
          {
            POIAnimationEnum poiAnimationEnum = poiBase.SupportedAnimations & character.SupportedAnimations;
            if (poiAnimationEnum != (POIAnimationEnum) 0)
            {
              float magnitude = (poiBase.transform.position - this.gameObject.transform.position).magnitude;
              if ((this.poiCache.Count <= this.AvoidLastUsedPoiNumber.Value || !this.lastUsedPOIs.Contains(poiBase)) && ((double) this.InSearchRadius.Value <= 0.0 || (double) magnitude <= (double) this.InSearchRadius.Value || this.SearchEntity.Entity != null) && this.CheckRegion(poiBase.transform.position))
              {
                int num2 = UnityEngine.Random.Range(0, 32);
                for (int index2 = 0; index2 < 32; ++index2)
                {
                  int num3 = (num2 + index2) % 32;
                  if ((poiAnimationEnum & (POIAnimationEnum) (1 << num3)) != 0)
                  {
                    POIAnimationEnum animation = (POIAnimationEnum) (1 << num3);
                    POIAnimationSetupBase animationSetup = character.GetAnimationSetup(animation);
                    this.OutAnimationIndex = UnityEngine.Random.Range(0, animationSetup.Elements.Count);
                    if (animationSetup.Elements.Count > this.OutAnimationIndex && animationSetup.Elements[this.OutAnimationIndex] is POIAnimationSetupElementSlow)
                      this.OutMiddleAnimationsCount = (animationSetup.Elements[this.OutAnimationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
                    this.OutAngle = poiBase.GetAngle(this.Owner.gameObject);
                    Vector3 targetPosition;
                    Quaternion targetRotation;
                    poiBase.GetRandomTargetPoint(animation, this.OutAnimationIndex, character, out targetPosition, out targetRotation);
                    this.OutPOI = poiBase;
                    this.lastUsedPOI = poiBase;
                    this.lastUsedPOIs.Add(poiBase);
                    if (this.lastUsedPOIs.Count > this.AvoidLastUsedPoiNumber.Value)
                      this.lastUsedPOIs.RemoveAt(0);
                    this.OutDestination.Value = targetPosition;
                    this.OutRotation.Value = targetRotation;
                    this.OutPOIAnimation = animation;
                    poiBase.Lock(this.gameObject, animation);
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
      this.executionStatus = TaskStatus.Inactive;
      this.currentChildIndex = 0;
      if (!((UnityEngine.Object) this.OutPOI != (UnityEngine.Object) null))
        return;
      this.OutPOI.Unlock(this.gameObject);
    }

    private bool CheckRegion(Vector3 position)
    {
      if (!this.SameRegionOnly.Value || this.region == null)
        return true;
      RegionComponent regionByPosition = RegionUtility.GetRegionByPosition(position);
      return regionByPosition != null && this.region.Region == regionByPosition.Region;
    }

    public void DataWrite(IDataWriter writer)
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
    }

    public void DataRead(IDataReader reader, System.Type type)
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
    }
  }
}
