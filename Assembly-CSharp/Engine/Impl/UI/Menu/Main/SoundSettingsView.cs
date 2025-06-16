using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class SoundSettingsView : SettingsView
  {
    [SerializeField]
    private AudioSource effectSound;
    [SerializeField]
    private AudioSource voiceSound;
    private FloatSettingsValueView masterVolumeView;
    private FloatSettingsValueView musicVolumeView;
    private FloatSettingsValueView effectsVolumeView;
    private FloatSettingsValueView voiceVolumeView;
    private BoolSettingsValueView subtitlesEnabledView;
    private BoolSettingsValueView dialogSubtitlesEnabledView;

    protected override void Awake()
    {
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.masterVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.masterVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Master.Volume}");
      this.masterVolumeView.SetMinValue(0.0f);
      this.masterVolumeView.SetMaxValue(1f);
      this.masterVolumeView.SetValueNameFunction(new Func<float, string>(this.ValueName));
      this.masterVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MasterVolume);
      this.masterVolumeView.SetValueValidationFunction(new Func<float, float>(this.ValueRound), 0.05f);
      FloatSettingsValueView masterVolumeView = this.masterVolumeView;
      masterVolumeView.VisibleValueChangeEvent = masterVolumeView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
      this.musicVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.musicVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Music.Volume}");
      this.musicVolumeView.SetMinValue(0.0f);
      this.musicVolumeView.SetMaxValue(1f);
      this.musicVolumeView.SetValueNameFunction(new Func<float, string>(this.ValueName));
      this.musicVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MusicVolume);
      this.musicVolumeView.SetValueValidationFunction(new Func<float, float>(this.ValueRound), 0.05f);
      FloatSettingsValueView musicVolumeView = this.musicVolumeView;
      musicVolumeView.VisibleValueChangeEvent = musicVolumeView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
      this.effectsVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.effectsVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Effects.Volume}");
      this.effectsVolumeView.SetMinValue(0.0f);
      this.effectsVolumeView.SetMaxValue(1f);
      this.effectsVolumeView.SetValueNameFunction(new Func<float, string>(this.ValueName));
      this.effectsVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.EffectsVolume);
      this.effectsVolumeView.SetValueValidationFunction(new Func<float, float>(this.ValueRound), 0.05f);
      FloatSettingsValueView effectsVolumeView = this.effectsVolumeView;
      effectsVolumeView.VisibleValueChangeEvent = effectsVolumeView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnEffectVolumeChange<float>);
      this.voiceVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.voiceVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Voice.Volume}");
      this.voiceVolumeView.SetMinValue(0.0f);
      this.voiceVolumeView.SetMaxValue(1f);
      this.voiceVolumeView.SetValueNameFunction(new Func<float, string>(this.ValueName));
      this.voiceVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.VoiceVolume);
      this.voiceVolumeView.SetValueValidationFunction(new Func<float, float>(this.ValueRound), 0.05f);
      FloatSettingsValueView voiceVolumeView = this.voiceVolumeView;
      voiceVolumeView.VisibleValueChangeEvent = voiceVolumeView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnVoiceVolumeChange<float>);
      this.subtitlesEnabledView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.subtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.Subtitles}");
      this.subtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.SubtitlesEnabled);
      BoolSettingsValueView subtitlesEnabledView1 = this.subtitlesEnabledView;
      subtitlesEnabledView1.VisibleValueChangeEvent = subtitlesEnabledView1.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(this.OnAutoValueChange<bool>);
      this.dialogSubtitlesEnabledView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.dialogSubtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.DialogSubtitles}");
      this.dialogSubtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.DialogSubtitlesEnabled);
      BoolSettingsValueView subtitlesEnabledView2 = this.dialogSubtitlesEnabledView;
      subtitlesEnabledView2.VisibleValueChangeEvent = subtitlesEnabledView2.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(this.OnAutoValueChange<bool>);
      base.Awake();
    }

    private void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      view.ApplyVisibleValue();
      this.UpdateViews();
      InstanceByRequest<SoundGameSettings>.Instance.Apply();
      InstanceByRequest<SubtitlesGameSettings>.Instance.Apply();
    }

    protected override void OnButtonReset()
    {
      this.masterVolumeView.ResetValue();
      this.musicVolumeView.ResetValue();
      this.effectsVolumeView.ResetValue();
      this.voiceVolumeView.ResetValue();
      this.subtitlesEnabledView.ResetValue();
      this.dialogSubtitlesEnabledView.ResetValue();
      InstanceByRequest<SoundGameSettings>.Instance.Apply();
      InstanceByRequest<SubtitlesGameSettings>.Instance.Apply();
      this.UpdateViews();
    }

    private void OnEffectVolumeChange<T>(SettingsValueView<T> view)
    {
      this.voiceSound.Stop();
      this.effectSound.Play();
      this.OnAutoValueChange<T>(view);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateViews();
    }

    private void OnVoiceVolumeChange<T>(SettingsValueView<T> view)
    {
      this.effectSound.Stop();
      this.voiceSound.Play();
      this.OnAutoValueChange<T>(view);
    }

    private void UpdateViews()
    {
      this.masterVolumeView.RevertVisibleValue();
      this.musicVolumeView.RevertVisibleValue();
      this.effectsVolumeView.RevertVisibleValue();
      this.voiceVolumeView.RevertVisibleValue();
      this.subtitlesEnabledView.RevertVisibleValue();
      this.dialogSubtitlesEnabledView.RevertVisibleValue();
      this.dialogSubtitlesEnabledView.Interactable = this.subtitlesEnabledView.VisibleValue;
    }

    private string ValueName(float value) => (value * 100f).ToString();

    private float ValueRound(float value) => Mathf.Round(value * 20f) / 20f;
  }
}
