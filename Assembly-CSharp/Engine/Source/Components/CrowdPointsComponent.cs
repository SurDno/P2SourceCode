using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Services.Engine.Assets;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Utilities;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (ICrowdPointsComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdPointsComponent : EngineComponent, ICrowdPointsComponent, IComponent
  {
    [Inspected]
    private HashSet<CrowdPointInfo> points = new HashSet<CrowdPointInfo>();
    private static List<CrowdPoint> crowdPointsTempList = new List<CrowdPoint>();
    [FromThis]
    private RegionComponent region;
    private ILocationComponent location;

    public event Action<ICrowdPointsComponent, bool> ChangePointsEvent;

    public IEnumerable<CrowdPointInfo> Points => (IEnumerable<CrowdPointInfo>) this.points;

    [Inspected]
    public bool PointsReady { get; private set; }

    public void GetEnabledPoints(AreaEnum area, int count, List<CrowdPointInfo> result)
    {
      if (area == AreaEnum.Unknown)
        return;
      if (area < AreaEnum.__EndMasks)
      {
        if (this.region == null || !((UnityEngine.Object) this.region.RegionMesh != (UnityEngine.Object) null))
          return;
        for (int index = 0; index < count; ++index)
          result.Add(new CrowdPointInfo()
          {
            GameObject = (GameObject) null,
            Rotation = Quaternion.identity,
            Area = area,
            NotReady = true
          });
      }
      else
      {
        if (area <= AreaEnum.__EndMasks)
          return;
        foreach (CrowdPointInfo point in this.points)
        {
          if (area == point.Area && (UnityEngine.Object) point.GameObject != (UnityEngine.Object) null && point.GameObject.activeInHierarchy)
            result.Add(point);
        }
      }
    }

    public bool TryFindPoint(out int radius, out Vector3 center, out Vector3 point, AreaEnum area)
    {
      int mask = area.ToMask();
      radius = 0;
      center = Vector3.zero;
      point = Vector3.zero;
      point = this.region.RegionMesh.GetRandomPoint();
      center = point;
      NavMeshUtility.SamplePosition(ref point, mask, out radius, 32);
      return RegionUtility.GetRegionByPosition(point) == this.region;
    }

    private void FindPoint(out int radius, out Vector3 center, out Vector3 point, AreaEnum area)
    {
      int mask = area.ToMask();
      radius = 0;
      center = Vector3.zero;
      point = Vector3.zero;
      for (int index = 0; index < 5; ++index)
      {
        point = this.region.RegionMesh.GetRandomPoint();
        center = point;
        NavMeshUtility.SamplePosition(ref point, mask, out radius, 32);
        if (RegionUtility.GetRegionByPosition(point) == this.region)
          return;
      }
      Debug.LogError((object) ("Nav mesh point not found , region : " + this.region.Owner.GetInfo() + " , area : " + (object) area));
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.location = LocationItemUtility.FindParentComponent<ILocationComponent>(this.Owner);
      if (this.location == null)
        return;
      this.location.OnHibernationChanged += new Action<ILocationComponent>(this.OnChangeHibernation);
      this.OnChangeHibernation(this.location);
    }

    public override void OnRemoved()
    {
      if (this.location != null)
        this.location.OnHibernationChanged -= new Action<ILocationComponent>(this.OnChangeHibernation);
      base.OnRemoved();
    }

    private void OnChangeHibernation(ILocationComponent sender)
    {
      this.points.Clear();
      this.PointsReady = false;
      if (!this.location.IsHibernation)
      {
        StaticModelComponent component = this.Owner.GetComponent<StaticModelComponent>();
        if (component != null)
        {
          SceneAsset sceneAsset = component.SceneAsset;
          if (sceneAsset != null)
          {
            foreach (GameObject rootGameObject in sceneAsset.Scene.GetRootGameObjects())
              this.ComputeGameObject(rootGameObject);
          }
        }
        else
        {
          GameObject gameObject = ((IEntityView) this.Owner).GameObject;
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
            this.ComputeGameObject(gameObject);
        }
        this.PointsReady = true;
      }
      Action<ICrowdPointsComponent, bool> changePointsEvent = this.ChangePointsEvent;
      if (changePointsEvent == null)
        return;
      changePointsEvent((ICrowdPointsComponent) this, this.PointsReady);
    }

    private void ComputeGameObject(GameObject go)
    {
      CrowdPointsComponent.crowdPointsTempList.Clear();
      Transform transform = go.transform;
      go.GetComponentsInChildren<CrowdPoint>(CrowdPointsComponent.crowdPointsTempList);
      foreach (CrowdPoint crowdPointsTemp in CrowdPointsComponent.crowdPointsTempList)
        this.points.Add(new CrowdPointInfo()
        {
          GameObject = crowdPointsTemp.gameObject,
          Radius = 0,
          CenterPoint = crowdPointsTemp.gameObject.transform.position,
          Position = crowdPointsTemp.gameObject.transform.position,
          Rotation = crowdPointsTemp.gameObject.transform.rotation,
          Area = crowdPointsTemp.Area,
          OnNavMesh = crowdPointsTemp.OnNavMesh,
          EntityPoint = LocationItemUtility.GetFirstEngineObject(crowdPointsTemp.gameObject.transform)
        });
      CrowdPointsComponent.crowdPointsTempList.Clear();
    }
  }
}
