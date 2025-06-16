using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using Scripts.Behaviours.LoadControllers;
using UnityEngine;

public class LoadByDistanceOfRegion : BaseLoadByDistance, IEntityAttachable
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

  public override float LoadDistance => this.loadDistance;

  public override float UnloadDistance => this.unloadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return (double) (position - this.region.RegionMesh.Center).magnitude < (double) (this.loadDistance + this.region.RegionMesh.Radius);
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return (double) (position - this.region.RegionMesh.Center).magnitude > (double) (this.unloadDistance + this.region.RegionMesh.Radius);
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
    if (this.region == null || (Object) this.region.RegionMesh == (Object) null || this.model == null)
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
