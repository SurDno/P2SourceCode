using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
using UnityEngine;

public class LoadByDistanceFromSegment : BaseLoadByDistance, IEntityAttachable, IUpdatable
{
  [SerializeField]
  private float loadDistance;
  [SerializeField]
  private float unloadDistance;
  [SerializeField]
  private float loadIndoorDistance;
  [SerializeField]
  private Vector3 segmentStart;
  [SerializeField]
  private Vector3 segmentEnd;
  [Inspected]
  private bool on;
  [Inspected]
  private StaticModelComponent model;
  [Inspected]
  private bool insideIndoor;

  public override float LoadDistance => loadDistance;

  public override float UnloadDistance => unloadDistance;

  public float LoadIndoorDistance => loadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return DistanceToSegment(position - transform.position, segmentStart, segmentEnd) < (insideIndoor ? loadIndoorDistance : (double) loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return DistanceToSegment(position - transform.position, segmentStart, segmentEnd) > (double) unloadDistance;
  }

  private void CheckAndFix()
  {
    if (loadIndoorDistance > (double) loadDistance)
    {
      Debug.LogError("loadIndoorDistance > loadDistance", gameObject);
      loadIndoorDistance = loadDistance;
    }
    if (loadDistance <= (double) unloadDistance)
      return;
    Debug.LogError("loadDistance > unloadDistance", gameObject);
    unloadDistance = loadDistance + 5f;
  }

  public void Attach(IEntity owner)
  {
    CheckAndFix();
    model = owner.GetComponent<StaticModelComponent>();
    if (model != null)
      model.NeedLoad = on;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged += OnInsideIndoorChanged;
    OnInsideIndoorChanged(false);
    InstanceByRequest<UpdateService>.Instance.RegionDistanceUpdater.AddUpdatable(this);
  }

  public void Detach()
  {
    InstanceByRequest<UpdateService>.Instance.RegionDistanceUpdater.RemoveUpdatable(this);
    model = null;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= OnInsideIndoorChanged;
  }

  public void ComputeUpdate()
  {
    if (model == null)
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null || !player.IsEnabledInHierarchy || player.GetComponent<NavigationComponent>().WaitTeleport)
      return;
    Vector3 position = ((IEntityView) player).Position;
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

  private void OnInsideIndoorChanged(bool inside)
  {
    insideIndoor = false;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return;
    LocationItemComponent component = player.GetComponent<LocationItemComponent>();
    if (component != null)
      insideIndoor = component.IsIndoor;
    else
      Debug.LogError("LocationItemComponent not found, owner : " + player.GetInfo());
  }

  public float DistanceToSegment(Vector3 p, Vector3 s0, Vector3 s1)
  {
    Vector3 vector3 = s1 - s0;
    float num1 = Vector3.SqrMagnitude(vector3);
    float num2 = 0.0f;
    if (num1 > 1.0 / 1000.0)
      num2 = Mathf.Clamp01(Vector3.Dot(p - s0, vector3) / num1);
    return (s0 + (s1 - s0) * num2 - p).magnitude;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawLine(transform.position + segmentStart, transform.position + segmentEnd);
    Gizmos.color = Color.red;
    DrawGizmoCapsule(loadDistance);
    Gizmos.color = Color.gray;
    DrawGizmoCapsule(loadIndoorDistance);
    Gizmos.color = Color.blue;
    DrawGizmoCapsule(unloadDistance);
  }

  private void DrawGizmoCapsule(float radius)
  {
    Vector3 normalized = (segmentEnd - segmentStart).normalized;
    Vector3 vector3_1 = Vector3.Cross(Vector3.up, normalized);
    Vector3 position = transform.position;
    for (int index = 0; index < 20; ++index)
    {
      float f1 = 0.157079637f * index;
      float f2 = 0.157079637f * (index + 1);
      Vector3 vector3_2 = radius * (vector3_1 * Mathf.Cos(f1) + normalized * Mathf.Sin(f1));
      Vector3 vector3_3 = radius * (vector3_1 * Mathf.Cos(f2) + normalized * Mathf.Sin(f2));
      Gizmos.DrawLine(position + segmentStart - vector3_2, position + segmentStart - vector3_3);
      Gizmos.DrawLine(position + segmentEnd + vector3_2, position + segmentEnd + vector3_3);
    }
    Gizmos.DrawLine(position + segmentStart + vector3_1 * radius, position + segmentEnd + vector3_1 * radius);
    Gizmos.DrawLine(position + segmentStart - vector3_1 * radius, position + segmentEnd - vector3_1 * radius);
  }
}
