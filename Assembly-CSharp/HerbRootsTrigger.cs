using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

public class HerbRootsTrigger : MonoBehaviour
{
  private SphereCollider trigger;

  public event Action PlayerEnterEvent;

  public event Action PlayerExitEvent;

  public float Radius
  {
    get => trigger.radius;
    set => trigger.radius = value;
  }

  [Inspected]
  public bool IsPlayerInside { get; private set; }

  private void Awake()
  {
    trigger = gameObject.AddComponent<SphereCollider>();
    trigger.isTrigger = true;
    trigger.radius = 5f;
  }

  private bool IsPlayer(GameObject gameObject)
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    return player != null && ((IEntityView) player).GameObject == gameObject;
  }

  private void OnTriggerEnter(Collider collider)
  {
    if (!IsPlayer(collider.gameObject))
      return;
    IsPlayerInside = true;
    Action playerEnterEvent = PlayerEnterEvent;
    if (playerEnterEvent != null)
      playerEnterEvent();
  }

  private void OnTriggerExit(Collider collider)
  {
    if (!IsPlayer(collider.gameObject))
      return;
    IsPlayerInside = false;
    PlayerExitEvent();
  }
}
