using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;

public class HerbRootsTrigger : MonoBehaviour
{
  private SphereCollider trigger;

  public event Action PlayerEnterEvent;

  public event Action PlayerExitEvent;

  public float Radius
  {
    get => this.trigger.radius;
    set => this.trigger.radius = value;
  }

  [Inspected]
  public bool IsPlayerInside { get; private set; }

  private void Awake()
  {
    this.trigger = this.gameObject.AddComponent<SphereCollider>();
    this.trigger.isTrigger = true;
    this.trigger.radius = 5f;
  }

  private bool IsPlayer(GameObject gameObject)
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    return player != null && (UnityEngine.Object) ((IEntityView) player).GameObject == (UnityEngine.Object) gameObject;
  }

  private void OnTriggerEnter(Collider collider)
  {
    if (!this.IsPlayer(collider.gameObject))
      return;
    this.IsPlayerInside = true;
    Action playerEnterEvent = this.PlayerEnterEvent;
    if (playerEnterEvent != null)
      playerEnterEvent();
  }

  private void OnTriggerExit(Collider collider)
  {
    if (!this.IsPlayer(collider.gameObject))
      return;
    this.IsPlayerInside = false;
    this.PlayerExitEvent();
  }
}
