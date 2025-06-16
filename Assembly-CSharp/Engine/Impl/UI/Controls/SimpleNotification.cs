using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Services.Notifications;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class SimpleNotification : UIControl, INotification {
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private float time;
	[SerializeField] private float fade;
	private float progress;
	private UIService ui;
	private float alpha = -1f;

	[Inspected] public bool Complete { get; private set; }

	[Inspected] public NotificationEnum Type { get; private set; }

	private void Update() {
		if (!(ui.Active is HudWindow))
			return;
		progress += Time.deltaTime;
		if (progress > (double)time)
			Complete = true;
		else
			SetAlpha(SoundUtility.ComputeFade(progress, time, fade));
	}

	protected override void Awake() {
		base.Awake();
		ui = ServiceLocator.GetService<UIService>();
		SetAlpha(0.0f);
	}

	public void Initialise(NotificationEnum type, object[] values) {
		Type = type;
	}

	public void Shutdown() {
		Destroy(gameObject);
	}

	private void SetAlpha(float value) {
		if (alpha == (double)value)
			return;
		alpha = value;
		canvasGroup.alpha = value;
	}
}