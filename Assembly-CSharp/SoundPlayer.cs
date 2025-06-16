using System.Collections;
using System.Collections.Generic;
using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {
	[SerializeField] private bool playInstantly;

	[EnumFlag(typeof(TimesOfDay))] [SerializeField]
	private TimesOfDay timesOfDay;

	[SerializeField] public List<AudioClip> clips;
	[SerializeField] private MinMaxPair delay;
	[SerializeField] private AudioSource audioSource;

	private IEnumerator Start() {
		if (audioSource == null) {
			Debug.LogError(
				"Null audio source, gameobject (need to fix by level designer), trying to get component: " +
				this.GetInfo(), gameObject);
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null) {
				Debug.LogError("Can't get component: " + this.GetInfo(), gameObject);
				yield break;
			}
		}

		if (audioSource.clip != null)
			Debug.LogError("clip != null, gameobject : " + gameObject.name, gameObject);
		else if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio) {
			clips.Cleanup();
			if (clips.Count != 0) {
				var wait = new WaitForSeconds(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance
					.SoundUpdateDelay);
				while (true) {
					do {
						do {
							bool inTimeOfDay;
							do {
								do {
									if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio) {
										yield return wait;
										UpdateEnable();
									} else
										goto label_9;
								} while (!audioSource.enabled);

								var timeOfDay = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
								inTimeOfDay = TimesOfDayUtility.HasValue(timesOfDay, timeOfDay);
							} while (!inTimeOfDay);

							if (!(audioSource.clip != null))
								goto label_19;
						} while (audioSource.isPlaying);

						if (audioSource.clip.loadState != AudioDataLoadState.Unloaded) {
							if (audioSource.clip.loadState == AudioDataLoadState.Failed)
								goto label_17;
						} else
							goto label_15;
					} while (audioSource.clip.loadState == AudioDataLoadState.Loading);

					goto label_20;
					label_9:
					yield return wait;
					continue;
					label_15:
					audioSource.clip.LoadAudioData();
					continue;
					label_17:
					audioSource.clip = null;
					continue;
					label_19:
					var clip = clips.Random();
					audioSource.clip = clip;
					continue;
					label_20:
					audioSource.PlayAndCheck();
					Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ")
						.Append(audioSource.clip.name).Append(" , context : ").GetFullName(gameObject));
					var delay2 = (playInstantly ? 0.0f : audioSource.clip.length) + Random.Range(delay.Min, delay.Max);
					yield return new WaitForSeconds(delay2);
				}
			}
		}
	}

	private void UpdateEnable() {
		var flag = (transform.position - EngineApplication.PlayerPosition).sqrMagnitude <
		           (double)(audioSource.maxDistance * audioSource.maxDistance);
		if (audioSource.enabled == flag)
			return;
		audioSource.enabled = flag;
	}
}