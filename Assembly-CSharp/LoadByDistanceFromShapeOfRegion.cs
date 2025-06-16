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

  public override float LoadDistance => this.loadDistance;

  public override float UnloadDistance => this.unloadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return this.loadBounds.Contains(position) && this.CheckInside(position, this.triangles, this.loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return !this.unloadBounds.Contains(position) || !this.CheckInside(position, this.triangles, this.unloadDistance);
  }

  public void Attach(IEntity owner)
  {
    this.model = owner.GetComponent<StaticModelComponent>();
    if (this.model != null)
      this.model.NeedLoad = this.on;
    this.region = owner.GetComponent<RegionComponent>();
  }

  public void Detach()
  {
    this.model = (StaticModelComponent) null;
    this.region = (RegionComponent) null;
  }

  private void Update()
  {
    if (this.region == null || (Object) this.region.RegionMesh == (Object) null)
      return;
    if (!this.initialise)
    {
      this.ComputeBounds();
      this.initialise = true;
    }
    if (this.model == null)
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null || player.GetComponent<NavigationComponent>().WaitTeleport || (double) Random.value > (double) Time.deltaTime / 0.5)
      return;
    this.position = ((IEntityView) player).Position;
    this.position.y = 0.0f;
    bool flag = this.on;
    if (this.on)
    {
      if (this.IsUnloadCondition(this.position))
        flag = false;
    }
    else if (this.IsLoadCondition(this.position))
      flag = true;
    if (flag == this.on)
      return;
    this.on = flag;
    this.model.NeedLoad = this.on;
  }

  private static float SquareDistanceToSegment(Vector2 a, Vector2 b, Vector2 p)
  {
    Vector2 vector2_1 = b - a;
    Vector2 vector2_2 = p - a;
    Vector2 vector2_3 = p - b;
    float num = Vector2.Dot(vector2_2, vector2_1);
    if ((double) num < 0.0)
      return Vector2.Dot(vector2_2, vector2_2);
    return (double) num > (double) Vector2.Dot(vector2_1, vector2_1) ? Vector2.Dot(vector2_3, vector2_3) : Vector2.Dot(vector2_2, vector2_2) - num * num / Vector2.Dot(vector2_1, vector2_1);
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
    return (double) num1 >= 0.0 && (double) num2 >= 0.0 && (double) num1 + (double) num2 <= 1.0;
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
      float segment1 = LoadByDistanceFromShapeOfRegion.SquareDistanceToSegment(vector2_1, vector2_2, p);
      if ((double) segment1 < (double) num2)
        return true;
      if ((double) segment1 < (double) num1)
        num1 = segment1;
      float segment2 = LoadByDistanceFromShapeOfRegion.SquareDistanceToSegment(vector2_2, vector2_3, p);
      if ((double) segment2 < (double) num2)
        return true;
      if ((double) segment2 < (double) num1)
        num1 = segment2;
      float segment3 = LoadByDistanceFromShapeOfRegion.SquareDistanceToSegment(vector2_3, vector2_1, p);
      if ((double) segment3 < (double) num2)
        return true;
      if ((double) segment3 < (double) num1)
        num1 = segment3;
      if (LoadByDistanceFromShapeOfRegion.IsPointInTriangle(p, vector2_1, vector2_2, vector2_3))
        return true;
    }
    return false;
  }

  private void ComputeBounds()
  {
    if (this.region == null || (Object) this.region.RegionMesh == (Object) null)
      return;
    this.triangles = this.region.RegionMesh.Triangles;
    if (this.triangles.Length == 0)
      return;
    this.originalBounds = new Bounds(this.triangles[0], Vector3.zero);
    for (int index = 1; index < this.triangles.Length; ++index)
      this.originalBounds.Encapsulate(this.triangles[index]);
    this.loadBounds = this.originalBounds;
    this.loadBounds.Expand(new Vector3(2f * this.loadDistance, 1000f, 2f * this.loadDistance));
    this.unloadBounds = this.originalBounds;
    this.unloadBounds.Expand(new Vector3(2f * this.unloadDistance, 1000f, 2f * this.unloadDistance));
  }

  private void OnDrawGizmosSelected()
  {
    if (this.triangles == null)
      return;
    Gizmos.color = Color.white;
    Gizmos.DrawWireCube(this.originalBounds.center, this.originalBounds.size);
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(this.loadBounds.center, this.loadBounds.size);
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(this.unloadBounds.center, this.unloadBounds.size);
    Gizmos.color = Color.magenta;
    for (int index = 0; index < this.triangles.Length; index += 3)
    {
      Gizmos.DrawLine(this.triangles[index], this.triangles[index + 1]);
      Gizmos.DrawLine(this.triangles[index], this.triangles[index + 2]);
      Gizmos.DrawLine(this.triangles[index + 1], this.triangles[index + 2]);
    }
    Gizmos.color = Color.white;
    Gizmos.DrawLine(this.position, this.originalBounds.center);
  }
}
