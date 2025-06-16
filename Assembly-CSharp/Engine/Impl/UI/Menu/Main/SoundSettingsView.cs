using Engine.Source.Commons;
using Engine.Source.Settings;

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
      layout = UnityEngine.Object.Instantiate<LayoutContainer>(listLayoutPrefab, this.transform, false);
      this.masterVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(floatValueViewPrefab, (Transform) layout.Content, false);
      this.masterVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Master.Volume}");
      this.masterVolumeView.SetMinValue(0.0f);
      this.masterVolumeView.SetMaxValue(1f);
      this.masterVolumeView.SetValueNameFunction(ValueName);
      this.masterVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MasterVolume);
      this.masterVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
      FloatSettingsValueView masterVolumeView = this.masterVolumeView;
      masterVolumeView.VisibleValueChangeEvent = masterVolumeView.VisibleValueChangeEvent + OnAutoValueChange;
      this.musicVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(floatValueViewPrefab, (Transform) layout.Content, false);
      this.musicVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Music.Volume}");
      this.musicVolumeView.SetMinValue(0.0f);
      this.musicVolumeView.SetMaxValue(1f);
      this.musicVolumeView.SetValueNameFunction(ValueName);
      this.musicVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.MusicVolume);
      this.musicVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
      FloatSettingsValueView musicVolumeView = this.musicVolumeView;
      musicVolumeView.VisibleValueChangeEvent = musicVolumeView.VisibleValueChangeEvent + OnAutoValueChange;
      this.effectsVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(floatValueViewPrefab, (Transform) layout.Content, false);
      this.effectsVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Effects.Volume}");
      this.effectsVolumeView.SetMinValue(0.0f);
      this.effectsVolumeView.SetMaxValue(1f);
      this.effectsVolumeView.SetValueNameFunction(ValueName);
      this.effectsVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.EffectsVolume);
      this.effectsVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
      FloatSettingsValueView effectsVolumeView = this.effectsVolumeView;
      effectsVolumeView.VisibleValueChangeEvent = effectsVolumeView.VisibleValueChangeEvent + OnEffectVolumeChange;
      this.voiceVolumeView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(floatValueViewPrefab, (Transform) layout.Content, false);
      this.voiceVolumeView.SetName("{UI.Menu.Main.Settings.Audio.Voice.Volume}");
      this.voiceVolumeView.SetMinValue(0.0f);
      this.voiceVolumeView.SetMaxValue(1f);
      this.voiceVolumeView.SetValueNameFunction(ValueName);
      this.voiceVolumeView.SetSetting(InstanceByRequest<SoundGameSettings>.Instance.VoiceVolume);
      this.voiceVolumeView.SetValueValidationFunction(ValueRound, 0.05f);
      FloatSettingsValueView voiceVolumeView = this.voiceVolumeView;
      voiceVolumeView.VisibleValueChangeEvent = voiceVolumeView.VisibleValueChangeEvent + OnVoiceVolumeChange;
      subtitlesEnabledView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(boolValueViewPrefab, (Transform) layout.Content, false);
      subtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.Subtitles}");
      subtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.SubtitlesEnabled);
      BoolSettingsValueView subtitlesEnabledView1 = subtitlesEnabledView;
      subtitlesEnabledView1.VisibleValueChangeEvent = subtitlesEnabledView1.VisibleValueChangeEvent + OnAutoValueChange;
      dialogSubtitlesEnabledView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(boolValueViewPrefab, (Transform) layout.Content, false);
      dialogSubtitlesEnabledView.SetName("{UI.Menu.Main.Settings.Audio.DialogSubtitles}");
      dialogSubtitlesEnabledView.SetSetting(InstanceByRequest<SubtitlesGameSettings>.Instance.DialogSubtitlesEnabled);
      BoolSettingsValueView subtitlesEnabledView2 = dialogSubtitlesEnabledView;
      subtitlesEnabledView2.VisibleValueChangeEvent = subtitlesEnabledView2.VisibleValueChangeEvent + OnAutoValueChange;
      base.Awake();
    }

    private void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      view.ApplyVisibleValue();
      UpdateViews();
      InstanceByRequest<SoundGameSettings>.Instance.Apply();
      InstanceByRequest<SubtitlesGameSettings>.Instance.Apply();
    }

    protected override void OnButtonReset()
    {
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

    private void OnEffectVolumeChange<T>(SettingsValueView<T> view)
    {
      voiceSound.Stop();
      effectSound.Play();
      OnAutoValueChange(view);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      UpdateViews();
    }

    private void OnVoiceVolumeChange<T>(SettingsValueView<T> view)
    {
      effectSound.Stop();
      voiceSound.Play();
      OnAutoValueChange(view);
    }

    private void UpdateViews()
    {
      masterVolumeView.RevertVisibleValue();
      musicVolumeView.RevertVisibleValue();
      effectsVolumeView.RevertVisibleValue();
      voiceVolumeView.RevertVisibleValue();
      subtitlesEnabledView.RevertVisibleValue();
      dialogSubtitlesEnabledView.RevertVisibleValue();
      dialogSubtitlesEnabledView.Interactable = subtitlesEnabledView.VisibleValue;
    }

    private string ValueName(float value) => (value * 100f).ToString();

    private float ValueRound(float value) => Mathf.Round(value * 20f) / 20f;
  }
}
