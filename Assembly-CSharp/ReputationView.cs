using System;
using Engine.Common.Commons;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using UnityEngine;

public class ReputationView : MonoBehaviour {
	[SerializeField] private RectTransform currentRect;
	[SerializeField] private RectTransform otherRect;
	[SerializeField] private StringView nameView;
	[SerializeField] private HideableView currentView;
	[SerializeField] private ProgressView valueView;
	[Space] [SerializeField] private EventView upLowView;
	[SerializeField] private EventView upMediumView;
	[SerializeField] private EventView upHighView;
	[SerializeField] private EventView downLowView;
	[SerializeField] private EventView downMediumView;
	[SerializeField] private EventView downHighView;
	[Space] [SerializeField] private float mediumThreshold;
	[SerializeField] private float highThreshold;
	private RegionEnum regionId;
	private RegionComponent region;
	private bool updatedAfterGameLoad;

	public event Action ReputationHighDownEvent;

	public event Action ReputationHighUpEvent;

	private void Awake() {
		ServiceLocator.GetService<SavesService>().UnloadEvent += OnGameUnloaded;
	}

	public void Initialize(
		RegionEnum regionId,
		RectTransform currentLayout,
		RectTransform otherLayout) {
		this.regionId = regionId;
		currentRect.SetParent(currentLayout, false);
		otherRect.SetParent(otherLayout, false);
		UpdateRegion();
	}

	private void OnDestroy() {
		ServiceLocator.GetService<SavesService>().UnloadEvent -= OnGameUnloaded;
	}

	private void OnDisable() {
		SetRegion(null);
	}

	private void OnEnable() {
		UpdateRegion();
	}

	private void OnGameUnloaded() {
		updatedAfterGameLoad = false;
	}

	private void SetValue(float value) {
		var flag = !updatedAfterGameLoad || ServiceLocator.GetService<NotificationService>()
			.TypeOrLayerBlocked(NotificationEnum.Reputation);
		if (!flag) {
			var num = value - valueView.Progress;
			if (num <= -(double)highThreshold) {
				downHighView.Invoke();
				var reputationHighDownEvent = ReputationHighDownEvent;
				if (reputationHighDownEvent != null)
					reputationHighDownEvent();
			} else if (num <= -(double)mediumThreshold)
				downMediumView.Invoke();
			else if (num < 0.0)
				downLowView.Invoke();
			else if (num >= (double)highThreshold) {
				upHighView.Invoke();
				var reputationHighUpEvent = ReputationHighUpEvent;
				if (reputationHighUpEvent != null)
					reputationHighUpEvent();
			} else if (num >= (double)mediumThreshold)
				upMediumView.Invoke();
			else if (num > 0.0)
				upLowView.Invoke();
		}

		valueView.Progress = value;
		if (flag)
			valueView.SkipAnimation();
		updatedAfterGameLoad = true;
	}

	public void SetCurrentRegion(RegionEnum currentRegionId, bool instant) {
		currentView.Visible = regionId == currentRegionId;
		if (!instant)
			return;
		currentView.SkipAnimation();
	}

	private void SetRegion(RegionComponent newRegion) {
		if (region != null) {
			if (region.Reputation != null)
				region.Reputation.ChangeValueEvent -= SetValue;
			else
				Debug.LogError("region.Reputation == null , разобраться");
		}

		region = newRegion;
		if (region == null)
			return;
		nameView.StringValue = RegionUtility.GetRegionTitle(region);
		SetValue(region.Reputation.Value);
		region.Reputation.ChangeValueEvent += SetValue;
	}

	private void UpdateRegion() {
		SetRegion(RegionUtility.GetRegionByName(regionId));
	}
}