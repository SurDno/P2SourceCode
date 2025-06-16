using System.Collections.Generic;
using UnityEngine;

public class PlagueWeb1 : PlagueWeb
{
  public float ViewRadius = 25f;
  public float MaxLinkLength = 3f;
  public float StringsPerPointsPerSecond = 0.5f;
  public float CellSize = 8f;
  public LayerMask CollisionMask;
  [SerializeField]
  private PlagueWebLink linkPrototype;
  private List<PlagueWebCell> cellPool = new List<PlagueWebCell>();
  private List<PlagueWebPoint> pointPool = new List<PlagueWebPoint>();
  private Dictionary<PlagueWebCellId, PlagueWebCell> cells = new Dictionary<PlagueWebCellId, PlagueWebCell>();
  private List<PlagueWebPoint> searchBuffer = new List<PlagueWebPoint>();
  private List<PlagueWebLink> stringBuffer = new List<PlagueWebLink>();
  private int activePointsCount;
  private int activeStringsCount;
  private int visiblePointsCount = 1;
  private float phase = 0.0f;

  public override Vector3 CameraPosition { get; set; }

  public override bool IsActive
  {
    get => this.enabled;
    set => this.enabled = value;
  }

  public override IPlagueWebPoint AddPoint(
    Vector3 position,
    Vector3 directionality,
    float strength)
  {
    PlagueWebPoint point;
    if (this.pointPool.Count > 0)
    {
      int index = this.pointPool.Count - 1;
      point = this.pointPool[index];
      this.pointPool.RemoveAt(index);
    }
    else
      point = new PlagueWebPoint();
    point.Position = position;
    point.Directionality = directionality;
    point.Strength = strength;
    this.PlacePoint(point);
    return (IPlagueWebPoint) point;
  }

  public void PlacePoint(PlagueWebPoint point)
  {
    PlagueWebCellId key = new PlagueWebCellId(point.Position, this.CellSize);
    PlagueWebCell plagueWebCell = point.Cell;
    if (plagueWebCell != null)
    {
      if (key == plagueWebCell.Id)
        return;
      plagueWebCell.RemovePoint(point);
    }
    if (!this.cells.TryGetValue(key, out plagueWebCell))
    {
      if (this.cellPool.Count > 0)
      {
        int index = this.cellPool.Count - 1;
        plagueWebCell = this.cellPool[index];
        this.cellPool.RemoveAt(index);
      }
      else
        plagueWebCell = new PlagueWebCell();
      plagueWebCell.Id = key;
      plagueWebCell.PlagueWeb = this;
      this.cells.Add(key, plagueWebCell);
    }
    point.Cell = plagueWebCell;
    plagueWebCell.AddPoint(point);
  }

  public PlagueWebLink AddString(PlagueWebPoint pointA, PlagueWebPoint pointB)
  {
    PlagueWebLink component;
    if (this.stringBuffer.Count > this.activeStringsCount)
    {
      component = this.stringBuffer[this.activeStringsCount];
    }
    else
    {
      component = Object.Instantiate<GameObject>(this.linkPrototype.gameObject).GetComponent<PlagueWebLink>();
      component.transform.SetParent(this.transform, false);
      this.stringBuffer.Add(component);
    }
    component.BeginAnimation(this, pointA, pointB);
    ++this.activeStringsCount;
    return component;
  }

  public bool Raycast(PlagueWebPoint pointA, PlagueWebPoint pointB)
  {
    Vector3 origin = pointA.Position + pointA.Directionality;
    Vector3 vector3 = pointB.Position + pointB.Directionality - origin;
    float magnitude = vector3.magnitude;
    Vector3 direction = vector3 / magnitude;
    return Physics.Raycast(origin, direction, magnitude, (int) this.CollisionMask, QueryTriggerInteraction.Ignore);
  }

  public void RemoveCell(PlagueWebCell cell)
  {
    this.cells.Remove(cell.Id);
    cell.PlagueWeb = (PlagueWeb1) null;
    this.cellPool.Add(cell);
  }

  public override void RemovePoint(IPlagueWebPoint point)
  {
    if (!(point is PlagueWebPoint point1))
      return;
    for (int index = 0; index < this.activeStringsCount; ++index)
      this.stringBuffer[index].OnPointDisable(point1);
    point1.Cell.RemovePoint(point1);
    point1.Cell = (PlagueWebCell) null;
    this.pointPool.Add(point1);
  }

  public void RemoveLink(PlagueWebLink plagueString)
  {
    for (int index = 0; index < this.activeStringsCount; ++index)
    {
      if ((Object) this.stringBuffer[index] == (Object) plagueString)
      {
        this.stringBuffer[index] = this.stringBuffer[this.activeStringsCount - 1];
        this.stringBuffer[this.activeStringsCount - 1] = plagueString;
        --this.activeStringsCount;
        break;
      }
    }
  }

  public void GetPointsInRadius(List<PlagueWebPoint> targetList, Vector3 position, float radius)
  {
    int num1 = Mathf.FloorToInt((position.x - radius) / this.CellSize);
    int num2 = Mathf.FloorToInt((position.z - radius) / this.CellSize);
    int num3 = Mathf.FloorToInt((position.x + radius) / this.CellSize);
    int num4 = Mathf.FloorToInt((position.z + radius) / this.CellSize);
    for (int x = num1; x <= num3; ++x)
    {
      for (int z = num2; z <= num4; ++z)
      {
        PlagueWebCell plagueWebCell;
        if (this.cells.TryGetValue(new PlagueWebCellId(x, z), out plagueWebCell))
        {
          for (int index = 0; index < plagueWebCell.Points.Count; ++index)
          {
            PlagueWebPoint point = plagueWebCell.Points[index];
            if ((double) point.Strength > 0.0 && (double) Vector3.Distance(position, point.Position) <= (double) radius)
              targetList.Add(point);
          }
        }
      }
    }
  }

  private void Update()
  {
    this.phase += Time.deltaTime * (float) this.visiblePointsCount * this.StringsPerPointsPerSecond;
    if ((double) this.phase < 1.0)
      return;
    this.phase = 0.0f;
    this.GetPointsInRadius(this.searchBuffer, this.CameraPosition, this.ViewRadius);
    this.visiblePointsCount = this.searchBuffer.Count > 0 ? this.searchBuffer.Count : 1;
    if (this.searchBuffer.Count > 1)
    {
      PlagueWebPoint plagueWebPoint1 = this.searchBuffer[Random.Range(0, this.searchBuffer.Count)];
      this.searchBuffer.Clear();
      this.GetPointsInRadius(this.searchBuffer, plagueWebPoint1.Position, this.MaxLinkLength);
      int index = Random.Range(0, this.searchBuffer.Count);
      if (this.searchBuffer[index] == plagueWebPoint1)
      {
        ++index;
        if (index == this.searchBuffer.Count)
          index = 0;
      }
      PlagueWebPoint plagueWebPoint2 = this.searchBuffer[index];
      if (!this.Raycast(plagueWebPoint1, plagueWebPoint2))
      {
        if ((double) plagueWebPoint1.Strength >= (double) plagueWebPoint2.Strength)
          this.AddString(plagueWebPoint1, plagueWebPoint2);
        else
          this.AddString(plagueWebPoint2, plagueWebPoint1);
      }
    }
    this.searchBuffer.Clear();
  }
}
