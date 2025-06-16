using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
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

  public override float LoadDistance => loadDistance;

  public override float UnloadDistance => unloadDistance;

  public float LoadIndoorDistance => loadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return (position - transform.position).magnitude < (insideIndoor ? loadIndoorDistance : (double) loadDistance);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return (position - transform.position).magnitude > (double) unloadDistance;
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

  public void Detach()
  {
    model = null;
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= OnInsideIndoorChanged;
  }

  private void Update()
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
}
