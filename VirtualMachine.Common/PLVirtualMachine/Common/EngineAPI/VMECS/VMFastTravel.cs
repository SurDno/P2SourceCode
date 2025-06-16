// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMFastTravel
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
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
        if (this.Component != null)
          return this.Component.CanFastTravel.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CanFastTravel.Value = value;
      }
    }

    [Property("FastTravelPointIndex", "", false, FastTravelPointEnum.None, false)]
    public FastTravelPointEnum FastTravelPointIndex
    {
      get
      {
        if (this.Component != null)
          return this.Component.FastTravelPointIndex.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return FastTravelPointEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.FastTravelPointIndex.Value = value;
      }
    }

    [Property("FastTravelPrice", "", false, 1, false)]
    public int FastTravelPrice
    {
      get
      {
        if (this.Component != null)
          return this.Component.FastTravelPrice.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.FastTravelPrice.Value = value;
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.TravelToPoint -= new Action<FastTravelPointEnum, TimeSpan>(this.FireTravelToPoint);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.TravelToPoint += new Action<FastTravelPointEnum, TimeSpan>(this.FireTravelToPoint);
    }

    private void FireTravelToPoint(FastTravelPointEnum target, TimeSpan travelTime)
    {
      Action<FastTravelPointEnum, GameTime> travelToPoint = this.TravelToPoint;
      if (travelToPoint == null)
        return;
      travelToPoint(target, new GameTime((GameTime) travelTime));
    }

    [Event("TravelToPoint", "Travel target, Travel time")]
    public event Action<FastTravelPointEnum, GameTime> TravelToPoint;
  }
}
