using System;
using Cofe.Loggers;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("FastTravelComponent", typeof (IFastTravelComponent))]
  public class VMFastTravel : VMEngineComponent<IFastTravelComponent>
  {
    public const string ComponentName = "FastTravelComponent";

    [Property("CanFastTravel", "", false, false, false)]
    public bool CanFastTravel
    {
      get
      {
        if (Component != null)
          return Component.CanFastTravel.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.CanFastTravel.Value = value;
      }
    }

    [Property("FastTravelPointIndex", "", false, FastTravelPointEnum.None, false)]
    public FastTravelPointEnum FastTravelPointIndex
    {
      get
      {
        if (Component != null)
          return Component.FastTravelPointIndex.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return FastTravelPointEnum.None;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.FastTravelPointIndex.Value = value;
      }
    }

    [Property("FastTravelPrice", "", false, 1, false)]
    public int FastTravelPrice
    {
      get
      {
        if (Component != null)
          return Component.FastTravelPrice.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.FastTravelPrice.Value = value;
      }
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.TravelToPoint -= FireTravelToPoint;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.TravelToPoint += FireTravelToPoint;
    }

    private void FireTravelToPoint(FastTravelPointEnum target, TimeSpan travelTime)
    {
      Action<FastTravelPointEnum, GameTime> travelToPoint = TravelToPoint;
      if (travelToPoint == null)
        return;
      travelToPoint(target, new GameTime(travelTime));
    }

    [Event("TravelToPoint", "Travel target, Travel time")]
    public event Action<FastTravelPointEnum, GameTime> TravelToPoint;
  }
}
