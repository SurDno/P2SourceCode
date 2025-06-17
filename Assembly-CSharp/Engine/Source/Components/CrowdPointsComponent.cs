using System;
using System.Collections.Generic;
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
using UnityEngine;

namespace Engine.Source.Components
{
  [Factory(typeof (ICrowdPointsComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CrowdPointsComponent : EngineComponent, ICrowdPointsComponent, IComponent
  {
    [Inspected]
    private HashSet<CrowdPointInfo> points = [];
    private static List<CrowdPoint> crowdPointsTempList = [];
    [FromThis]
    private RegionComponent region;
    private ILocationComponent location;

    public event Action<ICrowdPointsComponent, bool> ChangePointsEvent;

    public IEnumerable<CrowdPointInfo> Points => points;

    [Inspected]
    public bool PointsReady { get; private set; }

    public void GetEnabledPoints(AreaEnum area, int count, List<CrowdPointInfo> result)
    {
      if (area == AreaEnum.Unknown)
        return;
      if (area < AreaEnum.__EndMasks)
      {
        if (region == null || !(region.RegionMesh != null))
          return;
        for (int index = 0; index < count; ++index)
          result.Add(new CrowdPointInfo {
            GameObject = null,
            Rotation = Quaternion.identity,
            Area = area,
            NotReady = true
          });
      }
      else
      {
        if (area <= AreaEnum.__EndMasks)
          return;
        foreach (CrowdPointInfo point in points)
        {
          if (area == point.Area && point.GameObject != null && point.GameObject.activeInHierarchy)
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
      point = region.RegionMesh.GetRandomPoint();
      center = point;
      NavMeshUtility.SamplePosition(ref point, mask, out radius, 32);
      return RegionUtility.GetRegionByPosition(point) == region;
    }

    private void FindPoint(out int radius, out Vector3 center, out Vector3 point, AreaEnum area)
    {
      int mask = area.ToMask();
      radius = 0;
      center = Vector3.zero;
      point = Vector3.zero;
      for (int index = 0; index < 5; ++index)
      {
        point = region.RegionMesh.GetRandomPoint();
        center = point;
        NavMeshUtility.SamplePosition(ref point, mask, out radius, 32);
        if (RegionUtility.GetRegionByPosition(point) == region)
          return;
      }
      Debug.LogError("Nav mesh point not found , region : " + region.Owner.GetInfo() + " , area : " + area);
    }

    public override void OnAdded()
    {
      base.OnAdded();
      location = LocationItemUtility.FindParentComponent<ILocationComponent>(Owner);
      if (location == null)
        return;
      location.OnHibernationChanged += OnChangeHibernation;
      OnChangeHibernation(location);
    }

    public override void OnRemoved()
    {
      if (location != null)
        location.OnHibernationChanged -= OnChangeHibernation;
      base.OnRemoved();
    }

    private void OnChangeHibernation(ILocationComponent sender)
    {
      points.Clear();
      PointsReady = false;
      if (!location.IsHibernation)
      {
        StaticModelComponent component = Owner.GetComponent<StaticModelComponent>();
        if (component != null)
        {
          SceneAsset sceneAsset = component.SceneAsset;
          if (sceneAsset != null)
          {
            foreach (GameObject rootGameObject in sceneAsset.Scene.GetRootGameObjects())
              ComputeGameObject(rootGameObject);
          }
        }
        else
        {
          GameObject gameObject = ((IEntityView) Owner).GameObject;
          if (gameObject != null)
            ComputeGameObject(gameObject);
        }
        PointsReady = true;
      }
      Action<ICrowdPointsComponent, bool> changePointsEvent = ChangePointsEvent;
      if (changePointsEvent == null)
        return;
      changePointsEvent(this, PointsReady);
    }

    private void ComputeGameObject(GameObject go)
    {
      crowdPointsTempList.Clear();
      Transform transform = go.transform;
      go.GetComponentsInChildren(crowdPointsTempList);
      foreach (CrowdPoint crowdPointsTemp in crowdPointsTempList)
        points.Add(new CrowdPointInfo {
          GameObject = crowdPointsTemp.gameObject,
          Radius = 0,
          CenterPoint = crowdPointsTemp.gameObject.transform.position,
          Position = crowdPointsTemp.gameObject.transform.position,
          Rotation = crowdPointsTemp.gameObject.transform.rotation,
          Area = crowdPointsTemp.Area,
          OnNavMesh = crowdPointsTemp.OnNavMesh,
          EntityPoint = LocationItemUtility.GetFirstEngineObject(crowdPointsTemp.gameObject.transform)
        });
      crowdPointsTempList.Clear();
    }
  }
}
