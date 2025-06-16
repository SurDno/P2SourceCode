// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMRepairable
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("RepairableComponent", typeof (IRepairableComponent))]
  public class VMRepairable : VMEngineComponent<IRepairableComponent>
  {
    public const string ComponentName = "RepairableComponent";

    [Property("Durability", "", false, 1f, false)]
    public float Health
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

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.Durability.ChangeValueEvent -= new Action<float>(this.ChangeDurabilityValueEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.Durability.ChangeValueEvent += new Action<float>(this.ChangeDurabilityValueEvent);
    }

    private void ChangeDurabilityValueEvent(float value)
    {
      Action<float> changeDurability = this.OnChangeDurability;
      if (changeDurability == null)
        return;
      changeDurability(value);
    }

    [Event("Change durability", "Value")]
    public event Action<float> OnChangeDurability;
  }
}
