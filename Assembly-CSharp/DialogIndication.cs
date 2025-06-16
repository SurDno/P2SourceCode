using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

[DisallowMultipleComponent]
public class DialogIndication : MonoBehaviour, IEntityAttachable {
	[Inspected] private IEntity owner;
	[Inspected] private SpeakingComponent speakingComponent;
	private FocusEffect effect;
	private bool speakingAvailable;
	private bool compassEnabled;
	private bool visible;

	void IEntityAttachable.Attach(IEntity owner) {
		SetOwner(owner);
	}

	void IEntityAttachable.Detach() {
		SetOwner(null);
	}

	private void OnEnable() {
		var service = ServiceLocator.GetService<QuestCompassService>();
		if (service == null)
			return;
		OnEnableChanged(service.IsEnabled);
		service.OnEnableChanged += OnEnableChanged;
	}

	private void OnDisable() {
		var service = ServiceLocator.GetService<QuestCompassService>();
		if (service == null)
			return;
		service.OnEnableChanged -= OnEnableChanged;
		OnEnableChanged(false);
	}

	private void SetOwner(IEntity value) {
		if (owner == value)
			return;
		owner = value;
		SetSpeakingComponent(owner?.GetComponent<SpeakingComponent>());
	}

	private void SetSpeakingComponent(SpeakingComponent value) {
		if (speakingComponent == value)
			return;
		if (speakingComponent != null)
			speakingComponent.OnSpeakAvailableChange -= SetSpeakingAvailable;
		speakingComponent = value;
		if (speakingComponent != null) {
			SetSpeakingAvailable(speakingComponent.SpeakAvailable);
			speakingComponent.OnSpeakAvailableChange += SetSpeakingAvailable;
		} else
			SetSpeakingAvailable(false);
	}

	private void SetSpeakingAvailable(bool value) {
		if (speakingAvailable == value)
			return;
		speakingAvailable = value;
		UpdateVisibility();
	}

	private void OnEnableChanged(bool value) {
		if (compassEnabled == value)
			return;
		compassEnabled = value;
		UpdateVisibility();
	}

	private void SetVisibility(bool value) {
		if (visible == value)
			return;
		visible = value;
		if (effect == null) {
			if (!visible)
				return;
			effect = GetComponent<FocusEffect>();
			if (effect == null)
				effect = gameObject.AddComponent<FocusEffect>();
		} else
			effect.enabled = visible;
	}

	private void UpdateVisibility() {
		SetVisibility(speakingAvailable && compassEnabled);
	}
}