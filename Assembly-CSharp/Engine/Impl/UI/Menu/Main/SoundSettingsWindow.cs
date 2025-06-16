using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main;

public abstract class SoundSettingsWindow : UIWindow {
	private CameraKindEnum lastCameraKind;

	[SerializeField] [FormerlySerializedAs("AudioSourceEffects")]
	private GameObject audioSourceEffects;

	[SerializeField] [FormerlySerializedAs("AudioSourceMusic")]
	private GameObject audioSourceMusic;

	[SerializeField] [FormerlySerializedAs("AusioSourceVoice")]
	private GameObject audioSourceVoice;

	[Header("Sounds")] [SerializeField] [FormerlySerializedAs("ClickSound")]
	private AudioClip clickSound;

	[SerializeField] [FormerlySerializedAs("Slider_EffectsVolume")]
	private Slider sliderEffectsVolume;

	[SerializeField] [FormerlySerializedAs("Slider_MasterVolume")]
	private Slider sliderMasterVolume;

	[SerializeField] [FormerlySerializedAs("Slider_MusicVolume")]
	private Slider sliderMusicVolume;

	[SerializeField] [FormerlySerializedAs("Slider_VoiceVolume")]
	private Slider sliderVoiceVolume;

	private SoundGameSettings soundGameSettings;

	protected abstract void RegisterLayer();

	public override void Initialize() {
		RegisterLayer();
		soundGameSettings = InstanceByRequest<SoundGameSettings>.Instance;
		sliderMasterVolume.onValueChanged.AddListener(Slider_Master_Volume_Value_Changed_Handler);
		sliderMusicVolume.onValueChanged.AddListener(Slider_Music_Volume_Value_Changed_Handler);
		sliderEffectsVolume.onValueChanged.AddListener(Slider_Effects_Volume_Value_Changed_Handler);
		sliderVoiceVolume.onValueChanged.AddListener(Slider_Voice_Volume_Value_Changed_Handler);
		var componentsInChildren = GetComponentsInChildren<Button>(true);
		for (var index = 0; index < componentsInChildren.Length; ++index) {
			componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(eventData => Button_Click_Handler());
			componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
		}

		base.Initialize();
	}

	private void UpdateSliders() {
		sliderMasterVolume.value = soundGameSettings.MasterVolume.Value;
		sliderMusicVolume.value = soundGameSettings.MusicVolume.Value;
		sliderEffectsVolume.value = soundGameSettings.EffectsVolume.Value;
		sliderVoiceVolume.value = soundGameSettings.VoiceVolume.Value;
	}

	public void Button_Click_Handler() {
		if (!gameObject.activeInHierarchy)
			return;
		gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
	}

	public void Button_Back_Click_Handler() {
		ServiceLocator.GetService<UIService>().Pop();
	}

	public void Button_Reset_Click_Handler() {
		soundGameSettings.MasterVolume.Value = soundGameSettings.MasterVolume.DefaultValue;
		soundGameSettings.EffectsVolume.Value = soundGameSettings.EffectsVolume.DefaultValue;
		soundGameSettings.MusicVolume.Value = soundGameSettings.MusicVolume.DefaultValue;
		soundGameSettings.VoiceVolume.Value = soundGameSettings.VoiceVolume.DefaultValue;
		soundGameSettings.Apply();
		UpdateSliders();
	}

	public void Slider_Master_Volume_Value_Changed_Handler(float value) {
		if (!TryChangeValue(soundGameSettings.MasterVolume, value))
			return;
		soundGameSettings.Apply();
	}

	public void Slider_Effects_Volume_Value_Changed_Handler(float value) {
		if (!TryChangeValue(soundGameSettings.EffectsVolume, value))
			return;
		soundGameSettings.Apply();
		audioSourceVoice.GetComponent<AudioSource>().Stop();
		audioSourceEffects.GetComponent<AudioSource>().Play();
	}

	public void Slider_Music_Volume_Value_Changed_Handler(float value) {
		if (!TryChangeValue(soundGameSettings.MusicVolume, value))
			return;
		soundGameSettings.Apply();
	}

	public void Slider_Voice_Volume_Value_Changed_Handler(float value) {
		if (!TryChangeValue(soundGameSettings.VoiceVolume, value))
			return;
		soundGameSettings.Apply();
		audioSourceEffects.GetComponent<AudioSource>().Stop();
		audioSourceVoice.GetComponent<AudioSource>().Play();
	}

	private bool TryChangeValue(IValue<float> setting, float value) {
		if (setting.Value == (double)value)
			return false;
		setting.Value = value;
		return true;
	}

	protected override void OnEnable() {
		base.OnEnable();
		UpdateSliders();
		lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
		ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		CursorService.Instance.Free = CursorService.Instance.Visible = true;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		base.OnDisable();
	}
}