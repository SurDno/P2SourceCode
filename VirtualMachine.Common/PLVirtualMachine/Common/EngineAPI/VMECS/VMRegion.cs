// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMRegion
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
  [Info("Region", typeof (IRegionComponent))]
  public class VMRegion : VMEngineComponent<IRegionComponent>
  {
    public const string ComponentName = "Region";

    [Property("RegionDiseaseLevel", "")]
    public int RegionDiseaseLevel
    {
      get
      {
        if (this.Component != null)
          return this.Component.DiseaseLevel.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.DiseaseLevel.Value = value;
      }
    }

    [Property("Reputation", "")]
    public float Reputation
    {
      get
      {
        if (this.Component != null)
          return this.Component.Reputation.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Reputation.Value = value;
      }
    }

    [Property("RegionIndex", "", true, 0)]
    public int RegionIndex { get; set; }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.DiseaseLevel.ChangeValueEvent -= new Action<int>(this.OnDiseaseLevelChanged);
      this.Component.Reputation.ChangeValueEvent -= new Action<float>(this.OnReputationChanged);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.DiseaseLevel.ChangeValueEvent += new Action<int>(this.OnDiseaseLevelChanged);
      this.Component.Reputation.ChangeValueEvent += new Action<float>(this.OnReputationChanged);
    }

    [Event("Reputation changed", "level of disease")]
    public event Action<float> ReputationChanged;

    public void OnReputationChanged(float value)
    {
      Action<float> reputationChanged = this.ReputationChanged;
      if (reputationChanged != null)
        reputationChanged(value);
      if (VMGameComponent.Instance == null)
        return;
      VMGameComponent.Instance.OnRegionReputationChanged(this.Component, value);
    }

    [Event("Disease level changed", "level of disease")]
    public event Action<int> DiseaseLevelChanged;

    public void OnDiseaseLevelChanged(int value)
    {
      Action<int> diseaseLevelChanged = this.DiseaseLevelChanged;
      if (diseaseLevelChanged != null)
        diseaseLevelChanged(value);
      VMGameComponent.Instance.OnRegionDiseaseLevelChanged(this.Component, value);
    }
  }
}
