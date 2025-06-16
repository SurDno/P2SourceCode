using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main;

public class SoundSettingsView : SettingsView {
	[SerializeField] private AudioSource effectSound;
	[SerializeField] private AudioSource voiceSound;
	private FloatSettingsValueView masterVolumeView;
	private FloatSettingsValueView musicVolumeView;
	private FloatSettingsValueView effectsVolumeView;
	private FloatSettingsValueView voiceVolumeView;
	private BoolSettingsValueView subtitlesEnabledView;
	private BoolSettingsValueView dialogSubtitlesEnabledView;

	protected override void Awake() {
		layout = Instantiate(listLayoutPrefab, transform, false);
		this.masterVolumeView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.masterVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Master.Volume}");
		this.masterVolumeView.SetMinValue(0.0f);
		this.masterVolumeView.SetMaxValue(1f);
		this.masterVolumeView.SetValueNameFunction(ValueName);
		this.masterVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MasterVolume);
		this.masterVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
		var masterVolumeView = this.masterVolumeView;
		masterVolumeView.VisibleValueChangeEvent = masterVolumeView.VisibleValueChangeEvent + OnAutoValueChange;
		this.musicVolumeView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.musicVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Music.Volume}");
		this.musicVolumeView.SetMinValue(0.0f);
		this.musicVolumeView.SetMaxValue(1f);
		this.musicVolumeView.SetValueNameFunction(ValueName);
		this.musicVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MusicVolume);
		this.musicVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
		var musicVolumeView = this.musicVolumeView;
		musicVolumeView.VisibleValueChangeEvent = musicVolumeView.VisibleValueChangeEvent + OnAutoValueChange;
		this.effectsVolumeView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.effectsVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Effects.Volume}");
		this.effectsVolumeView.SetMinValue(0.0f);
		this.effectsVolumeView.SetMaxValue(1f);
		this.effectsVolumeView.SetValueNameFunction(ValueName);
		this.effectsVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.EffectsVolume);
		this.effectsVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
		var effectsVolumeView = this.effectsVolumeView;
		effectsVolumeView.VisibleValueChangeEvent = effectsVolumeView.VisibleValueChangeEvent + OnEffectVolumeChange;
		this.voiceVolumeView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.voiceVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Voice.Volume}");
		this.voiceVolumeView.SetMinValue(0.0f);
		this.voiceVolumeView.SetMaxValue(1f);
		this.voiceVolumeView.SetValueNameFunction(ValueName);
		this.voiceVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.VoiceVolume);
		this.voiceVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
		var voiceVolumeView = this.voiceVolumeView;
		voiceVolumeView.VisibleValueChangeEvent = voiceVolumeView.VisibleValueChangeEvent + OnVoiceVolumeChange;
		subtitlesEnabledView = Instantiate(boolValueViewPrefab, layout.Content, false);
		subtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.Subtitles}");
		subtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.SubtitlesEnabled);
		var subtitlesEnabledView1 = subtitlesEnabledView;
		subtitlesEnabledView1.VisibleValueChangeEvent =
			subtitlesEnabledView1.VisibleValueChangeEvent + OnAutoValueChange;
		dialogSubtitlesEnabledView = Instantiate(boolValueViewPrefab, layout.Content, false);
		dialogSubtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.DialogSubtitles}");
		dialogSubtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.DialogSubtitlesEnabled);
		var subtitlesEnabledView2 = dialogSubtitlesEnabledView;
		subtitlesEnabledView2.VisibleValueChangeEvent =
			subtitlesEnabledView2.VisibleValueChangeEvent + OnAutoValueChange;
		base.Awake();
	}

	private void OnAutoValueChange<T>(SettingsValueView<T> view) {
		view.ApplyVisibleValue();
		UpdateViews();
		InstanceByRequest<SoundGameSettings>.Instance.Apply();
		InstanceByRequest<SubtitlesGameSettings>.Instance.Apply();
	}

	protected override void OnButtonReset() {
		masterVolumeView.ResetValue();
		musicVolumeView.ResetValue();
		effectsVolumeView.ResetValue();
		voiceVolumeView.ResetValue();
		subtitlesEnabledView.ResetValue();
		dialogSubtitlesEnabledView.ResetValue();
		InstanceByRequest<SoundGameSettings>.Instance.Apply();
		InstanceByRequest<SubtitlesGameSettings>.Instance.Apply();
		UpdateViews();
	}

	private void OnEffectVolumeChange<T>(SettingsValueView<T> view) {
		voiceSound.Stop();
		effectSound.Play();
		OnAutoValueChange(view);
	}

	protected override void OnEnable() {
		base.OnEnable();
		UpdateViews();
	}

	private void OnVoiceVolumeChange<T>(SettingsValueView<T> view) {
		effectSound.Stop();
		voiceSound.Play();
		OnAutoValueChange(view);
	}

	private void UpdateViews() {
		masterVolumeView.RevertVisibleValue();
		musicVolumeView.RevertVisibleValue();
		effectsVolumeView.RevertVisibleValue();
		voiceVolumeView.RevertVisibleValue();
		subtitlesEnabledView.RevertVisibleValue();
		dialogSubtitlesEnabledView.RevertVisibleValue();
		dialogSubtitlesEnabledView.Interactable = subtitlesEnabledView.VisibleValue;
	}

	private string ValueName(float value) {
		return (value * 100f).ToString();
	}

	private float ValueRound(float value) {
		return Mathf.Round(value * 20f) / 20f;
	}
}