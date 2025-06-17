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
  private List<PlagueWebCell> cellPool = [];
  private List<PlagueWebPoint> pointPool = [];
  private Dictionary<PlagueWebCellId, PlagueWebCell> cells = new();
  private List<PlagueWebPoint> searchBuffer = [];
  private List<PlagueWebLink> stringBuffer = [];
  private int activePointsCount;
  private int activeStringsCount;
  private int visiblePointsCount = 1;
  private float phase;

  public override Vector3 CameraPosition { get; set; }

  public override bool IsActive
  {
    get => enabled;
    set => enabled = value;
  }

  public override IPlagueWebPoint AddPoint(
    Vector3 position,
    Vector3 directionality,
    float strength)
  {
    PlagueWebPoint point;
    if (pointPool.Count > 0)
    {
      int index = pointPool.Count - 1;
      point = pointPool[index];
      pointPool.RemoveAt(index);
    }
    else
      point = new PlagueWebPoint();
    point.Position = position;
    point.Directionality = directionality;
    point.Strength = strength;
    PlacePoint(point);
    return point;
  }

  public void PlacePoint(PlagueWebPoint point)
  {
    PlagueWebCellId key = new PlagueWebCellId(point.Position, CellSize);
    PlagueWebCell plagueWebCell = point.Cell;
    if (plagueWebCell != null)
    {
      if (key == plagueWebCell.Id)
        return;
      plagueWebCell.RemovePoint(point);
    }
    if (!cells.TryGetValue(key, out plagueWebCell))
    {
      if (cellPool.Count > 0)
      {
        int index = cellPool.Count - 1;
        plagueWebCell = cellPool[index];
        cellPool.RemoveAt(index);
      }
      else
        plagueWebCell = new PlagueWebCell();
      plagueWebCell.Id = key;
      plagueWebCell.PlagueWeb = this;
      cells.Add(key, plagueWebCell);
    }
    point.Cell = plagueWebCell;
    plagueWebCell.AddPoint(point);
  }

  public PlagueWebLink AddString(PlagueWebPoint pointA, PlagueWebPoint pointB)
  {
    PlagueWebLink component;
    if (stringBuffer.Count > activeStringsCount)
    {
      component = stringBuffer[activeStringsCount];
    }
    else
    {
      component = Instantiate(linkPrototype.gameObject).GetComponent<PlagueWebLink>();
      component.transform.SetParent(transform, false);
      stringBuffer.Add(component);
    }
    component.BeginAnimation(this, pointA, pointB);
    ++activeStringsCount;
    return component;
  }

  public bool Raycast(PlagueWebPoint pointA, PlagueWebPoint pointB)
  {
    Vector3 origin = pointA.Position + pointA.Directionality;
    Vector3 vector3 = pointB.Position + pointB.Directionality - origin;
    float magnitude = vector3.magnitude;
    Vector3 direction = vector3 / magnitude;
    return Physics.Raycast(origin, direction, magnitude, CollisionMask, QueryTriggerInteraction.Ignore);
  }

  public void RemoveCell(PlagueWebCell cell)
  {
    cells.Remove(cell.Id);
    cell.PlagueWeb = null;
    cellPool.Add(cell);
  }

  public override void RemovePoint(IPlagueWebPoint point)
  {
    if (!(point is PlagueWebPoint point1))
      return;
    for (int index = 0; index < activeStringsCount; ++index)
      stringBuffer[index].OnPointDisable(point1);
    point1.Cell.RemovePoint(point1);
    point1.Cell = null;
    pointPool.Add(point1);
  }

  public void RemoveLink(PlagueWebLink plagueString)
  {
    for (int index = 0; index < activeStringsCount; ++index)
    {
      if (stringBuffer[index] == plagueString)
      {
        stringBuffer[index] = stringBuffer[activeStringsCount - 1];
        stringBuffer[activeStringsCount - 1] = plagueString;
        --activeStringsCount;
        break;
      }
    }
  }

  public void GetPointsInRadius(List<PlagueWebPoint> targetList, Vector3 position, float radius)
  {
    int num1 = Mathf.FloorToInt((position.x - radius) / CellSize);
    int num2 = Mathf.FloorToInt((position.z - radius) / CellSize);
    int num3 = Mathf.FloorToInt((position.x + radius) / CellSize);
    int num4 = Mathf.FloorToInt((position.z + radius) / CellSize);
    for (int x = num1; x <= num3; ++x)
    {
      for (int z = num2; z <= num4; ++z)
      {
        if (cells.TryGetValue(new PlagueWebCellId(x, z), out PlagueWebCell plagueWebCell))
        {
          for (int index = 0; index < plagueWebCell.Points.Count; ++index)
          {
            PlagueWebPoint point = plagueWebCell.Points[index];
            if (point.Strength > 0.0 && Vector3.Distance(position, point.Position) <= (double) radius)
              targetList.Add(point);
          }
        }
      }
    }
  }

  private void Update()
  {
    phase += Time.deltaTime * visiblePointsCount * StringsPerPointsPerSecond;
    if (phase < 1.0)
      return;
    phase = 0.0f;
    GetPointsInRadius(searchBuffer, CameraPosition, ViewRadius);
    visiblePointsCount = searchBuffer.Count > 0 ? searchBuffer.Count : 1;
    if (searchBuffer.Count > 1)
    {
      PlagueWebPoint plagueWebPoint1 = searchBuffer[Random.Range(0, searchBuffer.Count)];
      searchBuffer.Clear();
      GetPointsInRadius(searchBuffer, plagueWebPoint1.Position, MaxLinkLength);
      int index = Random.Range(0, searchBuffer.Count);
      if (searchBuffer[index] == plagueWebPoint1)
      {
        ++index;
        if (index == searchBuffer.Count)
          index = 0;
      }
      PlagueWebPoint plagueWebPoint2 = searchBuffer[index];
      if (!Raycast(plagueWebPoint1, plagueWebPoint2))
      {
        if (plagueWebPoint1.Strength >= (double) plagueWebPoint2.Strength)
          AddString(plagueWebPoint1, plagueWebPoint2);
        else
          AddString(plagueWebPoint2, plagueWebPoint1);
      }
    }
    searchBuffer.Clear();
  }
}
