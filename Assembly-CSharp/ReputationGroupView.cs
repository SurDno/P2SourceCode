using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Services.Saves;
using UnityEngine;

public class ReputationGroupView : MonoBehaviour {
	[SerializeField] private ReputationView reputationViewPrefab;
	[Space] [SerializeField] private RectTransform currentLayout;
	[SerializeField] private RectTransform otherLayout;
	[Space] [SerializeField] private RegionEnum[] regions;
	[SerializeField] private EventView onReputationDown;
	[SerializeField] private EventView onReputationUp;
	private ReputationView[] views;
	private NavigationComponent playerNavigation;
	private RegionComponent currentRegion;
	private bool updatedAfterGameLoad;

	private void Awake() {
		views = new ReputationView[regions.Length];
		for (var index = 0; index < regions.Length; ++index) {
			var reputationView = Instantiate(reputationViewPrefab, transform);
			reputationView.Initialize(regions[index], currentLayout, otherLayout);
			reputationView.ReputationHighDownEvent += OnReputationDown;
			reputationView.ReputationHighUpEvent += OnReputationUp;
			views[index] = reputationView;
		}

		ServiceLocator.GetService<SavesService>().UnloadEvent += OnGameUnloaded;
	}

	private void OnDestroy() {
		ServiceLocator.GetService<SavesService>().UnloadEvent -= OnGameUnloaded;
	}

	private void OnDisable() {
		ServiceLocator.GetService<Simulation>().OnPlayerChanged -= SetPlayer;
		SetPlayer(null);
	}

	private void OnEnable() {
		var service = ServiceLocator.GetService<Simulation>();
		service.OnPlayerChanged += SetPlayer;
		SetPlayer(service.Player);
	}

	private void OnGameUnloaded() {
		updatedAfterGameLoad = false;
	}

	private void SetCurrentRegion(IRegionComponent region) {
		var currentRegionId = region != null ? region.Region : RegionEnum.None;
		for (var index = 0; index < views.Length; ++index)
			views[index].SetCurrentRegion(currentRegionId, !updatedAfterGameLoad);
		updatedAfterGameLoad = true;
	}

	private void SetPlayer(IEntity entity) {
		var component = entity?.GetComponent<NavigationComponent>();
		if (playerNavigation == component)
			return;
		if (playerNavigation != null) {
			playerNavigation.EnterRegionEvent -= OnRegionEnter;
			playerNavigation.ExitRegionEvent -= OnRegionExit;
		}

		playerNavigation = component;
		if (playerNavigation != null) {
			playerNavigation.EnterRegionEvent += OnRegionEnter;
			playerNavigation.ExitRegionEvent += OnRegionExit;
			SetCurrentRegion(playerNavigation.Region);
		} else
			SetCurrentRegion(null);
	}

	private void OnRegionExit(
		ref EventArgument<IEntity, IRegionComponent> eventArguments) {
		if (currentRegion != eventArguments.Target)
			return;
		SetCurrentRegion(null);
	}

	private void OnRegionEnter(
		ref EventArgument<IEntity, IRegionComponent> eventArguments) {
		if (currentRegion == eventArguments.Target)
			return;
		SetCurrentRegion(eventArguments.Target);
	}

	private void OnReputationUp() {
		onReputationUp?.Invoke();
	}

	private void OnReputationDown() {
		onReputationDown?.Invoke();
	}
}