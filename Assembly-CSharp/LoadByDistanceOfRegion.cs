using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using Scripts.Behaviours.LoadControllers;

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

  public override float LoadDistance => loadDistance;

  public override float UnloadDistance => unloadDistance;

  private bool IsLoadCondition(Vector3 position)
  {
    return (double) (position - region.RegionMesh.Center).magnitude < loadDistance + region.RegionMesh.Radius;
  }

  private bool IsUnloadCondition(Vector3 position)
  {
    return (double) (position - region.RegionMesh.Center).magnitude > unloadDistance + region.RegionMesh.Radius;
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
    if (region == null || (Object) region.RegionMesh == (Object) null || model == null)
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
