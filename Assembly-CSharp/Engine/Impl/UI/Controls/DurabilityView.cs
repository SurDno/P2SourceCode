using System.Collections.Generic;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class DurabilityView : EntityViewBase, IChangeParameterListener {
	[SerializeField] private HideableView hideableView;
	[SerializeField] private ProgressViewBase solidView;
	[SerializeField] private ProgressViewBase repairableView;
	[SerializeField] private ProgressViewBase valueView;
	[SerializeField] private List<ProgressViewBase> thresholdViews = new();
	[SerializeField] private float defaultValue;
	private IParameter<float> parameter;
	private RepairableComponent repairable;
	private RepairableSettings repairableSettings;
	private int thresholdPrototypeCount = -1;

	public List<ProgressViewBase> ThresholdViews => thresholdViews;

	protected override void ApplyValue() {
		if (parameter != null)
			parameter.RemoveListener(this);
		if (Value == null) {
			parameter = null;
			repairable = null;
			repairableSettings = null;
		} else {
			parameter = Value.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
			repairable = Value.GetComponent<RepairableComponent>();
			repairableSettings = repairable?.Settings;
		}

		if (parameter != null)
			parameter.AddListener(this);
		ApplyParameterValue();
		if (!(hideableView != null))
			return;
		hideableView.Visible = parameter != null;
	}

	private void ApplyParameterValue() {
		ApplyRepairable();
		if (!(valueView != null))
			return;
		valueView.Progress = CurrentDurability();
	}

	private float CurrentDurability() {
		return Mathf.Clamp01(parameter != null ? parameter.Value : defaultValue);
	}

	private void ApplyRepairable() {
		RepairableLevel repairableLevel1 = null;
		RepairableLevel repairableLevel2 = null;
		var levels = repairableSettings?.Levels;
		if (levels == null || levels.Count == 0)
			SetThresholdCount(0);
		else {
			if (SetThresholdCount(levels.Count))
				for (var index = 0; index < levels.Count; ++index) {
					var repairableLevel3 = levels[index];
					if (repairableLevel3 == null || repairableLevel3.MaxDurability == 1.0)
						thresholdViews[index].gameObject.SetActive(false);
					else {
						var maxDurability = repairableLevel3.MaxDurability;
						if (index < thresholdViews.Count && thresholdViews[index] != null)
							thresholdViews[index].Progress = maxDurability;
					}
				}

			repairableLevel1 = repairable.TargetLevel();
			repairableLevel2 = repairable.BaseLevel();
		}

		if (repairableView != null)
			repairableView.Progress = repairableLevel1 != null ? repairableLevel1.MaxDurability : 0.0f;
		if (!(solidView != null))
			return;
		solidView.Progress = repairableLevel2 != null ? repairableLevel2.MaxDurability : 0.0f;
	}

	private void OnDestroy() {
		Value = null;
	}

	private bool SetThresholdCount(int count) {
		if (thresholdViews == null)
			return false;
		if (thresholdPrototypeCount == -1)
			thresholdPrototypeCount = thresholdViews.Count;
		if (thresholdPrototypeCount == 0)
			return false;
		while (thresholdViews.Count < count) {
			var thresholdView = thresholdViews[thresholdViews.Count % thresholdPrototypeCount];
			var progressViewBase = Instantiate(thresholdView);
			progressViewBase.transform.SetParent(thresholdView.transform.parent, false);
			thresholdViews.Add(progressViewBase);
		}

		for (var index = 0; index < thresholdViews.Count; ++index)
			thresholdViews[index].gameObject.SetActive(index < count);
		return true;
	}

	public void OnParameterChanged(IParameter parameter) {
		if (parameter != this.parameter)
			return;
		ApplyParameterValue();
	}
}