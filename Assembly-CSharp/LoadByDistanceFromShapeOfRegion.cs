using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
using UnityEngine;

public class LoadByDistanceFromShapeOfRegion : BaseLoadByDistance, IEntityAttachable
{
  [SerializeField]
  private float loadDistance;
  [SerializeField]
  private float unloadDistance;
  [Inspected]
  private bool on;
  [Inspected]
  private StaticModelComponent model;
  [Inspected]
  private RegionComponent region;
  [Inspected]
  private Bounds originalBounds;
  [Inspected]
  private Bounds loadBounds;
  [Inspected]
  private Bounds unloadBounds;
  [Inspected]
  private Vector3[] triangles;
  [Inspected]
  private Vector3 position;
  private bool initialise;

  public override float LoadDistance => loadDistance;

  public override float UnloadDistance => unloadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return loadBounds.Contains(position) && CheckInside(position, triangles, loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return !unloadBounds.Contains(position) || !CheckInside(position, triangles, unloadDistance);
  }

  public void Attach(IEntity owner)
  {
    model = owner.GetComponent<StaticModelComponent>();
    if (model != null)
      model.NeedLoad = on;
    region = owner.GetComponent<RegionComponent>();
  }

  public void Detach()
  {
    model = null;
    region = null;
  }

  private void Update()
  {
    if (region == null || region.RegionMesh == null)
      return;
    if (!initialise)
    {
      ComputeBounds();
      initialise = true;
    }
    if (model == null)
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null || player.GetComponent<NavigationComponent>().WaitTeleport || Random.value > Time.deltaTime / 0.5)
      return;
    position = ((IEntityView) player).Position;
    position.y = 0.0f;
    bool flag = on;
    if (on)
    {
      if (IsUnloadCondition(position))
        flag = false;
    }
    else if (IsLoadCondition(position))
      flag = true;
    if (flag == on)
      return;
    on = flag;
    model.NeedLoad = on;
  }

  private static float SquareDistanceToSegment(Vector2 a, Vector2 b, Vector2 p)
  {
    Vector2 vector2_1 = b - a;
    Vector2 vector2_2 = p - a;
    Vector2 vector2_3 = p - b;
    float num = Vector2.Dot(vector2_2, vector2_1);
    if (num < 0.0)
      return Vector2.Dot(vector2_2, vector2_2);
    return num > (double) Vector2.Dot(vector2_1, vector2_1) ? Vector2.Dot(vector2_3, vector2_3) : Vector2.Dot(vector2_2, vector2_2) - num * num / Vector2.Dot(vector2_1, vector2_1);
  }

  private static bool IsPointInTriangle(Vector2 p, Vector2 t0, Vector2 t1, Vector2 t2)
  {
    Vector2 lhs1 = t1 - t0;
    Vector2 lhs2 = t2 - t0;
    Vector2 lhs3 = p - t0;
    Vector2 rhs1 = new Vector2(-lhs1.y, lhs1.x);
    Vector2 rhs2 = new Vector2(-lhs2.y, lhs2.x);
    float num1 = Vector2.Dot(lhs3, rhs2) / Vector2.Dot(lhs1, rhs2);
    float num2 = Vector2.Dot(lhs3, rhs1) / Vector2.Dot(lhs2, rhs1);
    return num1 >= 0.0 && num2 >= 0.0 && num1 + (double) num2 <= 1.0;
  }

  private bool CheckInside(Vector3 position, Vector3[] triangles, float loadDistance)
  {
    float num1 = float.MaxValue;
    float num2 = loadDistance * loadDistance;
    Vector2 p = new Vector2(position.x, position.z);
    for (int index = 0; index < triangles.Length; index += 3)
    {
      Vector2 vector2_1 = new Vector2(triangles[index].x, triangles[index].z);
      Vector2 vector2_2 = new Vector2(triangles[index + 1].x, triangles[index + 1].z);
      Vector2 vector2_3 = new Vector2(triangles[index + 2].x, triangles[index + 2].z);
      float segment1 = SquareDistanceToSegment(vector2_1, vector2_2, p);
      if (segment1 < (double) num2)
        return true;
      if (segment1 < (double) num1)
        num1 = segment1;
      float segment2 = SquareDistanceToSegment(vector2_2, vector2_3, p);
      if (segment2 < (double) num2)
        return true;
      if (segment2 < (double) num1)
        num1 = segment2;
      float segment3 = SquareDistanceToSegment(vector2_3, vector2_1, p);
      if (segment3 < (double) num2)
        return true;
      if (segment3 < (double) num1)
        num1 = segment3;
      if (IsPointInTriangle(p, vector2_1, vector2_2, vector2_3))
        return true;
    }
    return false;
  }

  private void ComputeBounds()
  {
    if (region == null || region.RegionMesh == null)
      return;
    triangles = region.RegionMesh.Triangles;
    if (triangles.Length == 0)
      return;
    originalBounds = new Bounds(triangles[0], Vector3.zero);
    for (int index = 1; index < triangles.Length; ++index)
      originalBounds.Encapsulate(triangles[index]);
    loadBounds = originalBounds;
    loadBounds.Expand(new Vector3(2f * loadDistance, 1000f, 2f * loadDistance));
    unloadBounds = originalBounds;
    unloadBounds.Expand(new Vector3(2f * unloadDistance, 1000f, 2f * unloadDistance));
  }

  private void OnDrawGizmosSelected()
  {
    if (triangles == null)
      return;
    Gizmos.color = Color.white;
    Gizmos.DrawWireCube(originalBounds.center, originalBounds.size);
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(loadBounds.center, loadBounds.size);
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(unloadBounds.center, unloadBounds.size);
    Gizmos.color = Color.magenta;
    for (int index = 0; index < triangles.Length; index += 3)
    {
      Gizmos.DrawLine(triangles[index], triangles[index + 1]);
      Gizmos.DrawLine(triangles[index], triangles[index + 2]);
      Gizmos.DrawLine(triangles[index + 1], triangles[index + 2]);
    }
    Gizmos.color = Color.white;
    Gizmos.DrawLine(position, originalBounds.center);
  }
}
