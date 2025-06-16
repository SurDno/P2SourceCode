// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMWaterSupplyController
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
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
        if (this.Component != null)
          return this.Component.Bullets.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Bullets.Value = value;
      }
    }

    [Property("Durability", "", false, 1f, false)]
    public float Durability
    {
      get
      {
        if (this.Component != null)
          return this.Component.Durability.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Durability.Value = value;
      }
    }

    [Property("Liquid type", "", false, LiquidTypeEnum.Normal, false)]
    public LiquidTypeEnum LiquidType
    {
      get
      {
        if (this.Component != null)
          return this.Component.LiquidType.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return LiquidTypeEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.LiquidType.Value = value;
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      base.Clear();
    }

    protected override void Init()
    {
      int num = this.IsTemplate ? 1 : 0;
    }
  }
}
