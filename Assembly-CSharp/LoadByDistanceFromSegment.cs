using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
using System;
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

  public override float LoadDistance => this.loadDistance;

  public override float UnloadDistance => this.unloadDistance;

  public float LoadIndoorDistance => this.loadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return (double) this.DistanceToSegment(position - this.transform.position, this.segmentStart, this.segmentEnd) < (this.insideIndoor ? (double) this.loadIndoorDistance : (double) this.loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return (double) this.DistanceToSegment(position - this.transform.position, this.segmentStart, this.segmentEnd) > (double) this.unloadDistance;
  }

  private void CheckAndFix()
  {
    if ((double) this.loadIndoorDistance > (double) this.loadDistance)
    {
      Debug.LogError((object) "loadIndoorDistance > loadDistance", (UnityEngine.Object) this.gameObject);
      this.loadIndoorDistance = this.loadDistance;
    }
    if ((double) this.loadDistance <= (double) this.unloadDistance)
      return;
    Debug.LogError((object) "loadDistance > unloadDistance", (UnityEngine.Object) this.gameObject);
    this.unloadDistance = this.loadDistance + 5f;
  }

  public void Attach(IEntity owner)
  {
    this.CheckAndFix();
    this.model = owner.GetComponent<StaticModelComponent>();
    if (this.model != null)
      this.model.NeedLoad = this.on;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged += new Action<bool>(this.OnInsideIndoorChanged);
    this.OnInsideIndoorChanged(false);
    InstanceByRequest<UpdateService>.Instance.RegionDistanceUpdater.AddUpdatable((IUpdatable) this);
  }

  public void Detach()
  {
    InstanceByRequest<UpdateService>.Instance.RegionDistanceUpdater.RemoveUpdatable((IUpdatable) this);
    this.model = (StaticModelComponent) null;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= new Action<bool>(this.OnInsideIndoorChanged);
  }

  public void ComputeUpdate()
  {
    if (this.model == null)
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null || !player.IsEnabledInHierarchy || player.GetComponent<NavigationComponent>().WaitTeleport)
      return;
    Vector3 position = ((IEntityView) player).Position;
    bool flag = this.on;
    if (this.on)
    {
      if (this.IsUnloadCondition(position))
        flag = false;
    }
    else if (this.IsLoadCondition(position))
      flag = true;
    if (flag == this.on)
      return;
    this.on = flag;
    this.model.NeedLoad = this.on;
  }

  private void OnInsideIndoorChanged(bool inside)
  {
    this.insideIndoor = false;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return;
    LocationItemComponent component = player.GetComponent<LocationItemComponent>();
    if (component != null)
      this.insideIndoor = component.IsIndoor;
    else
      Debug.LogError((object) ("LocationItemComponent not found, owner : " + player.GetInfo()));
  }

  public float DistanceToSegment(Vector3 p, Vector3 s0, Vector3 s1)
  {
    Vector3 vector3 = s1 - s0;
    float num1 = Vector3.SqrMagnitude(vector3);
    float num2 = 0.0f;
    if ((double) num1 > 1.0 / 1000.0)
      num2 = Mathf.Clamp01(Vector3.Dot(p - s0, vector3) / num1);
    return (s0 + (s1 - s0) * num2 - p).magnitude;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawLine(this.transform.position + this.segmentStart, this.transform.position + this.segmentEnd);
    Gizmos.color = Color.red;
    this.DrawGizmoCapsule(this.loadDistance);
    Gizmos.color = Color.gray;
    this.DrawGizmoCapsule(this.loadIndoorDistance);
    Gizmos.color = Color.blue;
    this.DrawGizmoCapsule(this.unloadDistance);
  }

  private void DrawGizmoCapsule(float radius)
  {
    Vector3 normalized = (this.segmentEnd - this.segmentStart).normalized;
    Vector3 vector3_1 = Vector3.Cross(Vector3.up, normalized);
    Vector3 position = this.transform.position;
    for (int index = 0; index < 20; ++index)
    {
      float f1 = 0.157079637f * (float) index;
      float f2 = 0.157079637f * (float) (index + 1);
      Vector3 vector3_2 = radius * (vector3_1 * Mathf.Cos(f1) + normalized * Mathf.Sin(f1));
      Vector3 vector3_3 = radius * (vector3_1 * Mathf.Cos(f2) + normalized * Mathf.Sin(f2));
      Gizmos.DrawLine(position + this.segmentStart - vector3_2, position + this.segmentStart - vector3_3);
      Gizmos.DrawLine(position + this.segmentEnd + vector3_2, position + this.segmentEnd + vector3_3);
    }
    Gizmos.DrawLine(position + this.segmentStart + vector3_1 * radius, position + this.segmentEnd + vector3_1 * radius);
    Gizmos.DrawLine(position + this.segmentStart - vector3_1 * radius, position + this.segmentEnd - vector3_1 * radius);
  }
}
