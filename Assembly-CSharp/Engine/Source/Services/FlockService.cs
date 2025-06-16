// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.FlockService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (FlockService)})]
  public class FlockService : IInitialisable, IUpdatable
  {
    private HashSet<FlockObject> flocks = new HashSet<FlockObject>();
    private HashSet<LandingSpotObject> zones = new HashSet<LandingSpotObject>();
    private bool needUpdate = false;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      if (!this.needUpdate)
        return;
      this.DistributeLandingZones();
      this.needUpdate = false;
    }

    public void ScareAll()
    {
      foreach (LandingSpotObject zone in this.zones)
        zone.Scare();
    }

    public void RegisterFlock(FlockObject flock)
    {
      if (!this.flocks.Add(flock))
        return;
      this.needUpdate = true;
    }

    public void UnregisterFlock(FlockObject flock)
    {
      if (!this.flocks.Remove(flock))
        return;
      this.needUpdate = true;
    }

    public void RegisterLandingZone(LandingSpotObject zone)
    {
      if (!this.zones.Add(zone))
        return;
      this.needUpdate = true;
    }

    public void UnregisterLandingZone(LandingSpotObject zone)
    {
      if (!this.zones.Remove(zone))
        return;
      zone.Flock = (FlockObject) null;
      this.needUpdate = true;
    }

    private void DistributeLandingZones()
    {
      foreach (LandingSpotObject zone in this.zones)
      {
        if (!zone.Preseted && (UnityEngine.Object) zone.Flock == (UnityEngine.Object) null)
        {
          FlockObject withMinimumZones = this.GetFlockWithMinimumZones(zone);
          if ((UnityEngine.Object) withMinimumZones != (UnityEngine.Object) null)
            withMinimumZones.AddLandingZone(zone);
        }
      }
    }

    private FlockObject GetFlockWithMinimumZones(LandingSpotObject zone)
    {
      int num = int.MaxValue;
      FlockObject withMinimumZones = (FlockObject) null;
      foreach (FlockObject flock in this.flocks)
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
