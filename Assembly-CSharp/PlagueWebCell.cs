// Decompiled with JetBrains decompiler
// Type: PlagueWebCell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class PlagueWebCell
{
  public PlagueWebCellId Id;
  public List<PlagueWebPoint> Points = new List<PlagueWebPoint>();
  public PlagueWeb1 PlagueWeb;

  public void AddPoint(PlagueWebPoint point) => this.Points.Add(point);

  public void PlacePoint(PlagueWebPoint point) => this.PlagueWeb.PlacePoint(point);

  public void RemovePoint(PlagueWebPoint point)
  {
    this.Points.Remove(point);
    if (this.Points.Count != 0)
      return;
    this.PlagueWeb.RemoveCell(this);
  }
}
