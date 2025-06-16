using System.Collections.Generic;

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
