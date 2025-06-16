using System;
using System.Collections;
using Engine.Source.Commons;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Engine.Source.Audio;

public static class SoundUtility {
	public static void PlayAndCheck(this AudioSource source) {
		if (source.clip == null)
			Debug.LogError("Clip not found, gameobject : " + source.gameObject.name, source.gameObject);
		source.Play();
	}

	public static void SetTime(AudioSource source, float time) {
		var samples = source.clip.samples;
		var num = Mathf.Clamp((int)(time * (double)samples / source.clip.length), 0, samples - 1);
		source.timeSamples = num;
	}

	public static AudioState PlayAudioClip3D(
		Transform target,
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		float minDistance,
		float maxDistance,
		bool spatialize,
		float fadeTime,
		bool usePause = false,
		string context = "",
		Action complete = null) {
		return PlayAudioClip(target, clip, mixer, volume, 1f, minDistance, maxDistance, spatialize, fadeTime, usePause,
			context, complete);
	}

	public static AudioState PlayAudioClip3D(
		Transform target,
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		bool spatialize,
		float fadeTime,
		GameObject customSource,
		bool usePause = false,
		string context = "",
		Action complete = null) {
		return PlayAudioClip(target, clip, mixer, volume, 1f, spatialize, fadeTime, customSource, usePause, context,
			complete);
	}

	public static AudioState PlayAudioClip2D(
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		float fadeTime,
		bool usePause = false,
		string context = "",
		Action complete = null) {
		return PlayAudioClip(null, clip, mixer, volume, 0.0f, 0.0f, 0.0f, false, fadeTime, usePause, context, complete);
	}

	public static float ComputeFade(float progress, float length, float fade) {
		return ComputeFade(progress, length, fade, fade);
	}

	public static float ComputeFade(float progress, float length, float fadeIn, float fideOut) {
		progress = Mathf.Min(progress, length);
		return Mathf.Clamp01(Mathf.Min(fadeIn > 0.0 ? Mathf.Clamp01(progress / fadeIn) : 1f,
			fideOut > 0.0 ? 1f - Mathf.Clamp01((progress - (length - fideOut)) / fideOut) : 1f));
	}

	public static AudioState PlayAudioClip(
		GameObject prefab,
		Transform target,
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		float fadeTime,
		bool usePause = false,
		string context = "",
		Action complete = null) {
		var audioState = new AudioState();
		var group = UnityFactory.GetOrCreateGroup("[Sounds]");
		var gameObject = Object.Instantiate(prefab, group.transform, false);
		gameObject.name = clip.name;
		if (target != null) {
			gameObject.transform.position = target.position;
			gameObject.transform.rotation = target.rotation;
		}

		var component = gameObject.GetComponent<AudioSource>();
		audioState.AudioSource = component;
		component.clip = clip;
		var length = clip.length;
		var fade = ComputeFade(0.0f, length, fadeTime);
		component.volume = fade * volume;
		component.outputAudioMixerGroup = mixer;
		component.loop = false;
		component.PlayAndCheck();
		Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name)
			.Append(" , volume : ").Append(volume).Append(" , target : ").Append(target != null ? target.name : "null")
			.Append(" , context : ").Append(context));
		CoroutineService.Instance.Route(ComputeVolume(target, audioState, volume, fadeTime, length, usePause,
			complete));
		return audioState;
	}

	private static AudioState PlayAudioClip(
		Transform target,
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		float spatialBlend,
		float minDistance,
		float maxDistance,
		bool spatialize,
		float fadeTime,
		bool usePause,
		string context = "",
		Action complete = null) {
		var audioState = new AudioState();
		var gameObject = UnityFactory.Create("[Sounds]", clip.name);
		if (target != null) {
			gameObject.transform.position = target.position;
			gameObject.transform.rotation = target.rotation;
		}

		var source = gameObject.AddComponent<AudioSource>();
		audioState.AudioSource = source;
		source.clip = clip;
		var length = clip.length;
		var fade = ComputeFade(0.0f, length, fadeTime);
		source.volume = fade * volume;
		source.rolloffMode = AudioRolloffMode.Linear;
		source.spatialBlend = spatialBlend;
		source.minDistance = minDistance;
		source.maxDistance = maxDistance;
		source.outputAudioMixerGroup = mixer;
		source.reverbZoneMix = 1f;
		source.loop = false;
		source.spatialize = spatialize;
		source.PlayAndCheck();
		Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name)
			.Append(" , volume : ").Append(volume).Append(" , target : ").Append(target != null ? target.name : "null")
			.Append(" , context : ").Append(context));
		CoroutineService.Instance.Route(ComputeVolume(target, audioState, volume, fadeTime, length, usePause,
			complete));
		return audioState;
	}

	private static AudioState PlayAudioClip(
		Transform target,
		AudioClip clip,
		AudioMixerGroup mixer,
		float volume,
		float spatialBlend,
		bool spatialize,
		float fadeTime,
		GameObject customSource,
		bool usePause,
		string context = "",
		Action complete = null) {
		var audioState = new AudioState();
		var gameObject = Object.Instantiate(customSource);
		if (target != null) {
			gameObject.transform.position = target.position;
			gameObject.transform.rotation = target.rotation;
		}

		var component = gameObject.GetComponent<AudioSource>();
		audioState.AudioSource = component;
		component.clip = clip;
		var length = clip.length;
		var fade = ComputeFade(0.0f, length, fadeTime);
		component.volume = fade * volume;
		component.spatialBlend = spatialBlend;
		component.outputAudioMixerGroup = mixer;
		component.reverbZoneMix = 1f;
		component.loop = false;
		component.spatialize = spatialize;
		component.PlayAndCheck();
		Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name)
			.Append(" , volume : ").Append(volume).Append(" , target : ").Append(target != null ? target.name : "null")
			.Append(" , mixer : ").Append(mixer.name).Append(" , context : ").Append(context));
		CoroutineService.Instance.Route(ComputeVolume(target, audioState, volume, fadeTime, length, usePause,
			complete));
		return audioState;
	}

	private static IEnumerator ComputeVolume(
		Transform target,
		AudioState audioState,
		float volume,
		float fadeTime,
		float length,
		bool usePause,
		Action complete) {
		while (true) {
			do {
				yield return null;
				if (audioState != null) {
					if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled && usePause) {
						var pause = InstanceByRequest<EngineApplication>.Instance.IsPaused &&
						            !InstanceByRequest<EngineApplication>.Instance.DontStopLipSyncInPause;
						if (audioState.Pause != pause) {
							audioState.Pause = pause;
							if (audioState.Pause)
								audioState.AudioSource.Pause();
							else
								audioState.AudioSource.UnPause();
						}
					} else
						break;
				} else
					goto label_10;
			} while (audioState.Pause);

			if (audioState.AudioSource.isPlaying && !audioState.Complete &&
			    InstanceByRequest<EngineApplication>.Instance.ViewEnabled) {
				if (target != null) {
					audioState.AudioSource.transform.position = target.position;
					audioState.AudioSource.transform.rotation = target.rotation;
				}

				var advance = audioState.AudioSource.time;
				var fade = ComputeFade(advance, length, fadeTime);
				audioState.AudioSource.volume = fade * volume;
			} else
				goto label_11;
		}

		label_10:
		yield break;
		label_11:
		var go = audioState.AudioSource.gameObject;
		audioState.Complete = true;
		audioState.AudioSource = null;
		Object.Destroy(go);
		var action = complete;
		if (action != null)
			action();
	}
}