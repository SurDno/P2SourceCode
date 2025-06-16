using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public abstract class SoundSettingsWindow : UIWindow
  {
    private CameraKindEnum lastCameraKind;
    [SerializeField]
    [FormerlySerializedAs("AudioSourceEffects")]
    private GameObject audioSourceEffects;
    [SerializeField]
    [FormerlySerializedAs("AudioSourceMusic")]
    private GameObject audioSourceMusic;
    [SerializeField]
    [FormerlySerializedAs("AusioSourceVoice")]
    private GameObject audioSourceVoice;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    [FormerlySerializedAs("Slider_EffectsVolume")]
    private Slider sliderEffectsVolume;
    [SerializeField]
    [FormerlySerializedAs("Slider_MasterVolume")]
    private Slider sliderMasterVolume;
    [SerializeField]
    [FormerlySerializedAs("Slider_MusicVolume")]
    private Slider sliderMusicVolume;
    [SerializeField]
    [FormerlySerializedAs("Slider_VoiceVolume")]
    private Slider sliderVoiceVolume;
    private SoundGameSettings soundGameSettings;

    protected abstract void RegisterLayer();

    public override void Initialize()
    {
      this.RegisterLayer();
      this.soundGameSettings = InstanceByRequest<SoundGameSettings>.Instance;
      this.sliderMasterVolume.onValueChanged.AddListener(new UnityAction<float>(this.Slider_Master_Volume_Value_Changed_Handler));
      this.sliderMusicVolume.onValueChanged.AddListener(new UnityAction<float>(this.Slider_Music_Volume_Value_Changed_Handler));
      this.sliderEffectsVolume.onValueChanged.AddListener(new UnityAction<float>(this.Slider_Effects_Volume_Value_Changed_Handler));
      this.sliderVoiceVolume.onValueChanged.AddListener(new UnityAction<float>(this.Slider_Voice_Volume_Value_Changed_Handler));
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => this.Button_Click_Handler()));
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    private void UpdateSliders()
    {
      this.sliderMasterVolume.value = this.soundGameSettings.MasterVolume.Value;
      this.sliderMusicVolume.value = this.soundGameSettings.MusicVolume.Value;
      this.sliderEffectsVolume.value = this.soundGameSettings.EffectsVolume.Value;
      this.sliderVoiceVolume.value = this.soundGameSettings.VoiceVolume.Value;
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Reset_Click_Handler()
    {
      this.soundGameSettings.MasterVolume.Value = this.soundGameSettings.MasterVolume.DefaultValue;
      this.soundGameSettings.EffectsVolume.Value = this.soundGameSettings.EffectsVolume.DefaultValue;
      this.soundGameSettings.MusicVolume.Value = this.soundGameSettings.MusicVolume.DefaultValue;
      this.soundGameSettings.VoiceVolume.Value = this.soundGameSettings.VoiceVolume.DefaultValue;
      this.soundGameSettings.Apply();
      this.UpdateSliders();
    }

    public void Slider_Master_Volume_Value_Changed_Handler(float value)
    {
      if (!this.TryChangeValue(this.soundGameSettings.MasterVolume, value))
        return;
      this.soundGameSettings.Apply();
    }

    public void Slider_Effects_Volume_Value_Changed_Handler(float value)
    {
      if (!this.TryChangeValue(this.soundGameSettings.EffectsVolume, value))
        return;
      this.soundGameSettings.Apply();
      this.audioSourceVoice.GetComponent<AudioSource>().Stop();
      this.audioSourceEffects.GetComponent<AudioSource>().Play();
    }

    public void Slider_Music_Volume_Value_Changed_Handler(float value)
    {
      if (!this.TryChangeValue(this.soundGameSettings.MusicVolume, value))
        return;
      this.soundGameSettings.Apply();
    }

    public void Slider_Voice_Volume_Value_Changed_Handler(float value)
    {
      if (!this.TryChangeValue(this.soundGameSettings.VoiceVolume, value))
        return;
      this.soundGameSettings.Apply();
      this.audioSourceEffects.GetComponent<AudioSource>().Stop();
      this.audioSourceVoice.GetComponent<AudioSource>().Play();
    }

    private bool TryChangeValue(IValue<float> setting, float value)
    {
      if ((double) setting.Value == (double) value)
        return false;
      setting.Value = value;
      return true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateSliders();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      base.OnDisable();
    }
  }
}
