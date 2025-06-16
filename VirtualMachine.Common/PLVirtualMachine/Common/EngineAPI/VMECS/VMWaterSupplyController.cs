using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("WaterSupplyControllerComponent", typeof (IWaterSupplyControllerComponent))]
  public class VMWaterSupplyController : VMEngineComponent<IWaterSupplyControllerComponent>
  {
    public const string ComponentName = "WaterSupplyControllerComponent";

    [Property("Bullets", "", false, 999999, false)]
    public int Bullets
    {
      get
      {
        if (Component != null)
          return Component.Bullets.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Bullets.Value = value;
      }
    }

    [Property("Durability", "", false, 1f, false)]
    public float Durability
    {
      get
      {
        if (Component != null)
          return Component.Durability.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return 0.0f;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Durability.Value = value;
      }
    }

    [Property("Liquid type", "", false, LiquidTypeEnum.Normal, false)]
    public LiquidTypeEnum LiquidType
    {
      get
      {
        if (Component != null)
          return Component.LiquidType.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return LiquidTypeEnum.None;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.LiquidType.Value = value;
      }
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      base.Clear();
    }

    protected override void Init()
    {
      int num = IsTemplate ? 1 : 0;
    }
  }
}
