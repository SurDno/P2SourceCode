// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.DurabilityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class DurabilityView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    private HideableView hideableView = (HideableView) null;
    [SerializeField]
    private ProgressViewBase solidView = (ProgressViewBase) null;
    [SerializeField]
    private ProgressViewBase repairableView = (ProgressViewBase) null;
    [SerializeField]
    private ProgressViewBase valueView = (ProgressViewBase) null;
    [SerializeField]
    private List<ProgressViewBase> thresholdViews = new List<ProgressViewBase>();
    [SerializeField]
    private float defaultValue = 0.0f;
    private IParameter<float> parameter;
    private RepairableComponent repairable;
    private RepairableSettings repairableSettings;
    private int thresholdPrototypeCount = -1;

    public List<ProgressViewBase> ThresholdViews => this.thresholdViews;

    protected override void ApplyValue()
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      if (this.Value == null)
      {
        this.parameter = (IParameter<float>) null;
        this.repairable = (RepairableComponent) null;
        this.repairableSettings = (RepairableSettings) null;
      }
      else
      {
        this.parameter = this.Value.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
        this.repairable = this.Value.GetComponent<RepairableComponent>();
        this.repairableSettings = this.repairable?.Settings;
      }
      if (this.parameter != null)
        this.parameter.AddListener((IChangeParameterListener) this);
      this.ApplyParameterValue();
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = this.parameter != null;
    }

    private void ApplyParameterValue()
    {
      this.ApplyRepairable();
      if (!((Object) this.valueView != (Object) null))
        return;
      this.valueView.Progress = this.CurrentDurability();
    }

    private float CurrentDurability()
    {
      return Mathf.Clamp01(this.parameter != null ? this.parameter.Value : this.defaultValue);
    }

    private void ApplyRepairable()
    {
      RepairableLevel repairableLevel1 = (RepairableLevel) null;
      RepairableLevel repairableLevel2 = (RepairableLevel) null;
      List<RepairableLevel> levels = this.repairableSettings?.Levels;
      if (levels == null || levels.Count == 0)
      {
        this.SetThresholdCount(0);
      }
      else
      {
        if (this.SetThresholdCount(levels.Count))
        {
          for (int index = 0; index < levels.Count; ++index)
          {
            RepairableLevel repairableLevel3 = levels[index];
            if (repairableLevel3 == null || (double) repairableLevel3.MaxDurability == 1.0)
            {
              this.thresholdViews[index].gameObject.SetActive(false);
            }
            else
            {
              float maxDurability = repairableLevel3.MaxDurability;
              if (index < this.thresholdViews.Count && (Object) this.thresholdViews[index] != (Object) null)
                this.thresholdViews[index].Progress = maxDurability;
            }
          }
        }
        repairableLevel1 = this.repairable.TargetLevel();
        repairableLevel2 = this.repairable.BaseLevel();
      }
      if ((Object) this.repairableView != (Object) null)
        this.repairableView.Progress = repairableLevel1 != null ? repairableLevel1.MaxDurability : 0.0f;
      if (!((Object) this.solidView != (Object) null))
        return;
      this.solidView.Progress = repairableLevel2 != null ? repairableLevel2.MaxDurability : 0.0f;
    }

    private void OnDestroy() => this.Value = (IEntity) null;

    private bool SetThresholdCount(int count)
    {
      if (this.thresholdViews == null)
        return false;
      if (this.thresholdPrototypeCount == -1)
        this.thresholdPrototypeCount = this.thresholdViews.Count;
      if (this.thresholdPrototypeCount == 0)
        return false;
      while (this.thresholdViews.Count < count)
      {
        ProgressViewBase thresholdView = this.thresholdViews[this.thresholdViews.Count % this.thresholdPrototypeCount];
        ProgressViewBase progressViewBase = Object.Instantiate<ProgressViewBase>(thresholdView);
        progressViewBase.transform.SetParent(thresholdView.transform.parent, false);
        this.thresholdViews.Add(progressViewBase);
      }
      for (int index = 0; index < this.thresholdViews.Count; ++index)
        this.thresholdViews[index].gameObject.SetActive(index < count);
      return true;
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter != this.parameter)
        return;
      this.ApplyParameterValue();
    }
  }
}
