using Cofe.Utility;
using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Components;
using Engine.Source.Services.Notifications;
using Inspectors;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Impl.UI.Controls;

public class ReputationNotification : UIControl, INotification {
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Localizer textRegion;
	[SerializeField] private Localizer nearRegionText;
	[SerializeField] private ProgressView targetReputationView;
	[SerializeField] private ProgressView oldReputationView;
	[SerializeField] private HideableView downEffect;
	[SerializeField] private HideableView upEffect;
	[SerializeField] private AudioClip clip;
	[SerializeField] private AudioMixerGroup mixer;
	[SerializeField] private float time;
	[SerializeField] private float fade;
	[SerializeField] private float minStepAnimation = 0.25f;
	private IParameterValue<float> reputationParameter;
	private float progress;
	private UIService ui;
	private bool play;
	private float alpha = -1f;

	[Inspected] public bool Complete { get; private set; }

	[Inspected] public NotificationEnum Type { get; private set; }

	private void OnReputationChange(float newTarget) {
		var progress1 = targetReputationView.Progress;
		var progress2 = oldReputationView.Progress;
		if (Mathf.Sign(newTarget - progress2) != (double)Mathf.Sign(progress1 - progress2))
			StartChange(progress1, newTarget);
		else
			StartChange(progress2, newTarget);
		progress = Mathf.Min(progress, Mathf.Min(time - progress, fade));
	}

	private void Update() {
		if (Complete || !(ui.Active is HudWindow))
			return;
		if (!play) {
			Play();
			play = true;
		}

		progress += Time.deltaTime;
		if (progress > (double)time) {
			Complete = true;
			reputationParameter.ChangeValueEvent -= OnReputationChange;
		} else
			SetAlpha(SoundUtility.ComputeFade(progress, time, fade));
	}

	private void Play() {
		if (clip == null || mixer == null)
			return;
		SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, context: gameObject.GetFullName());
	}

	protected override void Awake() {
		base.Awake();
		ui = ServiceLocator.GetService<UIService>();
		SetAlpha(0.0f);
	}

	public void Initialise(NotificationEnum type, object[] values) {
		Type = type;
		IRegionComponent result1 = null;
		var result2 = 0.0f;
		ApplyValue(ref result1, values, 0);
		ApplyValue(ref result2, values, 1);
		if (result1 == null)
			Debug.LogError("Notifications : Reputation : No region parameter");
		else {
			textRegion.Signature = GetRegionName(result1);
			foreach (var nearRegion in ServiceLocator.GetService<ISimulation>().Player
				         .GetComponent<PlayerControllerComponent>().GetNearRegions(result1)) {
				var localizer = Instantiate(nearRegionText, nearRegionText.transform.parent, false);
				localizer.Signature = GetRegionName(nearRegion);
				localizer.gameObject.SetActive(true);
			}

			reputationParameter = result1.Reputation;
			reputationParameter.ChangeValueEvent += OnReputationChange;
			StartChange(result2, reputationParameter.Value);
		}
	}

	private void StartChange(float oldReputation, float newReputation) {
		var f = newReputation - oldReputation;
		var num = Mathf.Sign(f);
		if (f * (double)num < minStepAnimation)
			oldReputation = newReputation - minStepAnimation * num;
		oldReputationView.Progress = Mathf.Clamp01(oldReputation);
		targetReputationView.Progress = newReputation;
		downEffect.Visible = num == -1.0;
		upEffect.Visible = num == 1.0;
	}

	private string GetRegionName(IRegionComponent region) {
		var service = ServiceLocator.GetService<LocalizationService>();
		var component = region.GetComponent<IMapItemComponent>();
		string regionName = null;
		if (component != null) {
			var title = component.Title;
			if (title != LocalizedText.Empty)
				regionName = service.GetText(title);
		}

		if (regionName.IsNullOrEmpty())
			regionName = region.Region.ToString();
		return regionName;
	}

	public void Shutdown() {
		Destroy(gameObject);
	}

	private void SetAlpha(float value) {
		if (alpha == (double)value)
			return;
		alpha = value;
		canvasGroup.alpha = alpha;
	}

	private void ApplyValue<T>(ref T result, object[] values, int index) {
		if (index >= values.Length)
			return;
		var obj1 = values[index];
		if (obj1 == null || !(obj1 is T obj2))
			return;
		result = obj2;
	}
}