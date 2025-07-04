﻿using System.Collections.Generic;
using Engine.Common;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(typeof (GroupPointsService))]
  public class GroupPointsService : IInitialisable
  {
    private List<GroupPoint> freePoints;

    public void Initialise() => freePoints = [];

    public void Terminate() => freePoints.Clear();

    public void AddPoint(GroupPoint point)
    {
      if (freePoints.Contains(point))
        return;
      freePoints.Add(point);
    }

    public void RemovePoint(GroupPoint point) => freePoints.Remove(point);

    public GroupPoint GetFreePoint()
    {
      int index = Random.Range(0, freePoints.Count);
      if (freePoints.Count <= 0)
        return null;
      GroupPoint freePoint = freePoints[index];
      freePoints.RemoveAt(index);
      return freePoint;
    }
  }
}
