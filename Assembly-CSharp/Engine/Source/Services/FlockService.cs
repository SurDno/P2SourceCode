using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Commons;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (FlockService))]
  public class FlockService : IInitialisable, IUpdatable
  {
    private HashSet<FlockObject> flocks = new HashSet<FlockObject>();
    private HashSet<LandingSpotObject> zones = new HashSet<LandingSpotObject>();
    private bool needUpdate;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void ComputeUpdate()
    {
      if (!needUpdate)
        return;
      DistributeLandingZones();
      needUpdate = false;
    }

    public void ScareAll()
    {
      foreach (LandingSpotObject zone in zones)
        zone.Scare();
    }

    public void RegisterFlock(FlockObject flock)
    {
      if (!flocks.Add(flock))
        return;
      needUpdate = true;
    }

    public void UnregisterFlock(FlockObject flock)
    {
      if (!flocks.Remove(flock))
        return;
      needUpdate = true;
    }

    public void RegisterLandingZone(LandingSpotObject zone)
    {
      if (!zones.Add(zone))
        return;
      needUpdate = true;
    }

    public void UnregisterLandingZone(LandingSpotObject zone)
    {
      if (!zones.Remove(zone))
        return;
      zone.Flock = null;
      needUpdate = true;
    }

    private void DistributeLandingZones()
    {
      foreach (LandingSpotObject zone in zones)
      {
        if (!zone.Preseted && (UnityEngine.Object) zone.Flock == (UnityEngine.Object) null)
        {
          FlockObject withMinimumZones = GetFlockWithMinimumZones(zone);
          if ((UnityEngine.Object) withMinimumZones != (UnityEngine.Object) null)
            withMinimumZones.AddLandingZone(zone);
        }
      }
    }

    private FlockObject GetFlockWithMinimumZones(LandingSpotObject zone)
    {
      int num = int.MaxValue;
      FlockObject withMinimumZones = null;
      foreach (FlockObject flock in flocks)
      {
        if (flock.GetZonesCount() < num && (zone.SupportedFlocks & flock.FlockType) != 0)
        {
          num = flock.GetZonesCount();
          withMinimumZones = flock;
        }
      }
      return withMinimumZones;
    }
  }
}
