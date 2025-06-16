using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
using System;
using UnityEngine;

public class LoadByDistance : BaseLoadByDistance, IEntityAttachable
{
  [SerializeField]
  private float loadDistance;
  [SerializeField]
  private float unloadDistance;
  [SerializeField]
  private float loadIndoorDistance;
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
    return (double) (position - this.transform.position).magnitude < (this.insideIndoor ? (double) this.loadIndoorDistance : (double) this.loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return (double) (position - this.transform.position).magnitude > (double) this.unloadDistance;
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

  public void Detach()
  {
    this.model = (StaticModelComponent) null;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= new Action<bool>(this.OnInsideIndoorChanged);
  }

  private void Update()
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
}
