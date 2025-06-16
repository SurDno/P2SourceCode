// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.GroupPointsService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (GroupPointsService)})]
  public class GroupPointsService : IInitialisable
  {
    private List<GroupPoint> freePoints;

    public void Initialise() => this.freePoints = new List<GroupPoint>();

    public void Terminate() => this.freePoints.Clear();

    public void AddPoint(GroupPoint point)
    {
      if (this.freePoints.Contains(point))
        return;
      this.freePoints.Add(point);
    }

    public void RemovePoint(GroupPoint point) => this.freePoints.Remove(point);

    public GroupPoint GetFreePoint()
    {
      int index = UnityEngine.Random.Range(0, this.freePoints.Count);
      if (this.freePoints.Count <= 0)
        return (GroupPoint) null;
      GroupPoint freePoint = this.freePoints[index];
      this.freePoints.RemoveAt(index);
      return freePoint;
    }
  }
}
