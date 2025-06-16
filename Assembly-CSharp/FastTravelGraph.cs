using System;
using System.Collections.Generic;
using Engine.Common.Components.Regions;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fast Travel Graph")]
public class FastTravelGraph : ScriptableObject
{
  [SerializeField]
  private Link[] links;
  private List<Link> flow;
  private FastTravelPointEnum lastOrigin;
  private Queue<FastTravelPointEnum> searchQueue;

  public int GetPath(
    FastTravelPointEnum origin,
    FastTravelPointEnum destination,
    IList<FastTravelPointEnum> result)
  {
    if (result == null)
      return -1;
    result.Clear();
    if (origin == FastTravelPointEnum.None || destination == FastTravelPointEnum.None)
      return -1;
    if (origin == destination)
    {
      result.Add(origin);
      return 0;
    }
    CalculateFlow(origin);
    int index = IndexOf(flow, destination);
    if (index == -1)
      return -1;
    result.Add(destination);
    Link link = flow[index];
    int time = link.Time;
    for (FastTravelPointEnum pointB = link.PointB; pointB != origin; pointB = flow[IndexOf(flow, pointB)].PointB)
      result.Insert(0, pointB);
    result.Insert(0, origin);
    return time;
  }

  private void CalculateFlow(FastTravelPointEnum origin)
  {
    if (flow == null)
      flow = new List<Link>();
    if (origin == lastOrigin)
      return;
    flow.Clear();
    flow.Add(new Link {
      PointA = origin,
      PointB = FastTravelPointEnum.None,
      Time = 0
    });
    if (searchQueue == null)
      searchQueue = new Queue<FastTravelPointEnum>();
    searchQueue.Enqueue(origin);
    while (searchQueue.Count > 0)
    {
      FastTravelPointEnum point1 = searchQueue.Dequeue();
      int time = flow[IndexOf(flow, point1)].Time;
      for (int index1 = 0; index1 < links.Length; ++index1)
      {
        if (links[index1].PointA == point1 || links[index1].PointB == point1)
        {
          FastTravelPointEnum point2 = links[index1].PointA != point1 ? links[index1].PointA : links[index1].PointB;
          int num = links[index1].Time + time;
          int index2 = IndexOf(flow, point2);
          if (index2 == -1)
          {
            flow.Add(new Link {
              PointA = point2,
              PointB = point1,
              Time = num
            });
            searchQueue.Enqueue(point2);
          }
          else
          {
            Link link = flow[index2];
            if (num < link.Time)
            {
              link.Time = num;
              link.PointB = point1;
              flow[index2] = link;
              searchQueue.Enqueue(point2);
            }
          }
        }
      }
    }
  }

  private int IndexOf(List<Link> list, FastTravelPointEnum point)
  {
    for (int index = 0; index < list.Count; ++index)
    {
      if (list[index].PointA == point)
        return index;
    }
    return -1;
  }

  [Serializable]
  private struct Link
  {
    public FastTravelPointEnum PointA;
    public FastTravelPointEnum PointB;
    [Tooltip("Travel time in minutes")]
    public int Time;
  }
}
