// Decompiled with JetBrains decompiler
// Type: FastTravelGraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Regions;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Data/Fast Travel Graph")]
public class FastTravelGraph : ScriptableObject
{
  [SerializeField]
  private FastTravelGraph.Link[] links;
  private List<FastTravelGraph.Link> flow;
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
    this.CalculateFlow(origin);
    int index = this.IndexOf(this.flow, destination);
    if (index == -1)
      return -1;
    result.Add(destination);
    FastTravelGraph.Link link = this.flow[index];
    int time = link.Time;
    for (FastTravelPointEnum pointB = link.PointB; pointB != origin; pointB = this.flow[this.IndexOf(this.flow, pointB)].PointB)
      result.Insert(0, pointB);
    result.Insert(0, origin);
    return time;
  }

  private void CalculateFlow(FastTravelPointEnum origin)
  {
    if (this.flow == null)
      this.flow = new List<FastTravelGraph.Link>();
    if (origin == this.lastOrigin)
      return;
    this.flow.Clear();
    this.flow.Add(new FastTravelGraph.Link()
    {
      PointA = origin,
      PointB = FastTravelPointEnum.None,
      Time = 0
    });
    if (this.searchQueue == null)
      this.searchQueue = new Queue<FastTravelPointEnum>();
    this.searchQueue.Enqueue(origin);
    while (this.searchQueue.Count > 0)
    {
      FastTravelPointEnum point1 = this.searchQueue.Dequeue();
      int time = this.flow[this.IndexOf(this.flow, point1)].Time;
      for (int index1 = 0; index1 < this.links.Length; ++index1)
      {
        if (this.links[index1].PointA == point1 || this.links[index1].PointB == point1)
        {
          FastTravelPointEnum point2 = this.links[index1].PointA != point1 ? this.links[index1].PointA : this.links[index1].PointB;
          int num = this.links[index1].Time + time;
          int index2 = this.IndexOf(this.flow, point2);
          if (index2 == -1)
          {
            this.flow.Add(new FastTravelGraph.Link()
            {
              PointA = point2,
              PointB = point1,
              Time = num
            });
            this.searchQueue.Enqueue(point2);
          }
          else
          {
            FastTravelGraph.Link link = this.flow[index2];
            if (num < link.Time)
            {
              link.Time = num;
              link.PointB = point1;
              this.flow[index2] = link;
              this.searchQueue.Enqueue(point2);
            }
          }
        }
      }
    }
  }

  private int IndexOf(List<FastTravelGraph.Link> list, FastTravelPointEnum point)
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
