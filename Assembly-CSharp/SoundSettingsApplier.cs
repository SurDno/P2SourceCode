using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettingsApplier : MonoBehaviour {
	[SerializeField] private AudioMixer masterMixer;
	[SerializeField] private AudioMixer[] effectsMixers;
	[SerializeField] private AudioMixer[] musicMixers;
	[SerializeField] private AudioMixer[] voiceMixers;

	private void Start() {
		Apply();
		InstanceByRequest<SoundGameSettings>.Instance.OnApply += Apply;
	}

	private void OnDestroy() {
		InstanceByRequest<SoundGameSettings>.Instance.OnApply -= Apply;
	}

	private void Apply() {
		Apply(masterMixer, InstanceByRequest<SoundGameSettings>.Instance.MasterVolume.Value);
		Apply(effectsMixers, InstanceByRequest<SoundGameSettings>.Instance.EffectsVolume.Value);
		Apply(musicMixers, InstanceByRequest<SoundGameSettings>.Instance.MusicVolume.Value);
		Apply(voiceMixers, InstanceByRequest<SoundGameSettings>.Instance.VoiceVolume.Value);
	}

	private void Apply(AudioMixer mixer, float value) {
		mixer.SetFloat("Master+Base{0}::Volume", ValueToVolume(value));
	}

	private void Apply(AudioMixer[] mixers, float value) {
		var volume = ValueToVolume(value);
		foreach (var mixer in mixers)
			mixer.SetFloat("Master+Base{0}::Volume", volume);
	}

	private float ValueToVolume(float value) {
		value = Mathf.Clamp(value, 0.0001f, 1f);
		return Mathf.Log10(value) * 20f;
	}
}