using Engine.Source.Commons;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Audio
{
  public static class SoundUtility
  {
    public static void PlayAndCheck(this AudioSource source)
    {
      if ((UnityEngine.Object) source.clip == (UnityEngine.Object) null)
        Debug.LogError((object) ("Clip not found, gameobject : " + source.gameObject.name), (UnityEngine.Object) source.gameObject);
      source.Play();
    }

    public static void SetTime(AudioSource source, float time)
    {
      int samples = source.clip.samples;
      int num = Mathf.Clamp((int) ((double) time * (double) samples / (double) source.clip.length), 0, samples - 1);
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
      Action complete = null)
    {
      return SoundUtility.PlayAudioClip(target, clip, mixer, volume, 1f, minDistance, maxDistance, spatialize, fadeTime, usePause, context, complete);
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
      Action complete = null)
    {
      return SoundUtility.PlayAudioClip(target, clip, mixer, volume, 1f, spatialize, fadeTime, customSource, usePause, context, complete);
    }

    public static AudioState PlayAudioClip2D(
      AudioClip clip,
      AudioMixerGroup mixer,
      float volume,
      float fadeTime,
      bool usePause = false,
      string context = "",
      Action complete = null)
    {
      return SoundUtility.PlayAudioClip((Transform) null, clip, mixer, volume, 0.0f, 0.0f, 0.0f, false, fadeTime, usePause, context, complete);
    }

    public static float ComputeFade(float progress, float length, float fade)
    {
      return SoundUtility.ComputeFade(progress, length, fade, fade);
    }

    public static float ComputeFade(float progress, float length, float fadeIn, float fideOut)
    {
      progress = Mathf.Min(progress, length);
      return Mathf.Clamp01(Mathf.Min((double) fadeIn > 0.0 ? Mathf.Clamp01(progress / fadeIn) : 1f, (double) fideOut > 0.0 ? 1f - Mathf.Clamp01((progress - (length - fideOut)) / fideOut) : 1f));
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
      Action complete = null)
    {
      AudioState audioState = new AudioState();
      GameObject group = UnityFactory.GetOrCreateGroup("[Sounds]");
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, group.transform, false);
      gameObject.name = clip.name;
      if ((UnityEngine.Object) target != (UnityEngine.Object) null)
      {
        gameObject.transform.position = target.position;
        gameObject.transform.rotation = target.rotation;
      }
      AudioSource component = gameObject.GetComponent<AudioSource>();
      audioState.AudioSource = component;
      component.clip = clip;
      float length = clip.length;
      float fade = SoundUtility.ComputeFade(0.0f, length, fadeTime);
      component.volume = fade * volume;
      component.outputAudioMixerGroup = mixer;
      component.loop = false;
      component.PlayAndCheck();
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name).Append(" , volume : ").Append(volume).Append(" , target : ").Append((UnityEngine.Object) target != (UnityEngine.Object) null ? target.name : "null").Append(" , context : ").Append(context));
      CoroutineService.Instance.Route(SoundUtility.ComputeVolume(target, audioState, volume, fadeTime, length, usePause, complete));
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
      Action complete = null)
    {
      AudioState audioState = new AudioState();
      GameObject gameObject = UnityFactory.Create("[Sounds]", clip.name);
      if ((UnityEngine.Object) target != (UnityEngine.Object) null)
      {
        gameObject.transform.position = target.position;
        gameObject.transform.rotation = target.rotation;
      }
      AudioSource source = gameObject.AddComponent<AudioSource>();
      audioState.AudioSource = source;
      source.clip = clip;
      float length = clip.length;
      float fade = SoundUtility.ComputeFade(0.0f, length, fadeTime);
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
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name).Append(" , volume : ").Append(volume).Append(" , target : ").Append((UnityEngine.Object) target != (UnityEngine.Object) null ? target.name : "null").Append(" , context : ").Append(context));
      CoroutineService.Instance.Route(SoundUtility.ComputeVolume(target, audioState, volume, fadeTime, length, usePause, complete));
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
      Action complete = null)
    {
      AudioState audioState = new AudioState();
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(customSource);
      if ((UnityEngine.Object) target != (UnityEngine.Object) null)
      {
        gameObject.transform.position = target.position;
        gameObject.transform.rotation = target.rotation;
      }
      AudioSource component = gameObject.GetComponent<AudioSource>();
      audioState.AudioSource = component;
      component.clip = clip;
      float length = clip.length;
      float fade = SoundUtility.ComputeFade(0.0f, length, fadeTime);
      component.volume = fade * volume;
      component.spatialBlend = spatialBlend;
      component.outputAudioMixerGroup = mixer;
      component.reverbZoneMix = 1f;
      component.loop = false;
      component.spatialize = spatialize;
      component.PlayAndCheck();
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name).Append(" , volume : ").Append(volume).Append(" , target : ").Append((UnityEngine.Object) target != (UnityEngine.Object) null ? target.name : "null").Append(" , mixer : ").Append(mixer.name).Append(" , context : ").Append(context));
      CoroutineService.Instance.Route(SoundUtility.ComputeVolume(target, audioState, volume, fadeTime, length, usePause, complete));
      return audioState;
    }

    private static IEnumerator ComputeVolume(
      Transform target,
      AudioState audioState,
      float volume,
      float fadeTime,
      float length,
      bool usePause,
      Action complete)
    {
      while (true)
      {
        do
        {
          yield return (object) null;
          if (audioState != null)
          {
            if (InstanceByRequest<EngineApplication>.Instance.ViewEnabled && usePause)
            {
              bool pause = InstanceByRequest<EngineApplication>.Instance.IsPaused && !InstanceByRequest<EngineApplication>.Instance.DontStopLipSyncInPause;
              if (audioState.Pause != pause)
              {
                audioState.Pause = pause;
                if (audioState.Pause)
                  audioState.AudioSource.Pause();
                else
                  audioState.AudioSource.UnPause();
              }
            }
            else
              break;
          }
          else
            goto label_10;
        }
        while (audioState.Pause);
        if (audioState.AudioSource.isPlaying && !audioState.Complete && InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
        {
          if ((UnityEngine.Object) target != (UnityEngine.Object) null)
          {
            audioState.AudioSource.transform.position = target.position;
            audioState.AudioSource.transform.rotation = target.rotation;
          }
          float advance = audioState.AudioSource.time;
          float fade = SoundUtility.ComputeFade(advance, length, fadeTime);
          audioState.AudioSource.volume = fade * volume;
        }
        else
          goto label_11;
      }
label_10:
      yield break;
label_11:
      GameObject go = audioState.AudioSource.gameObject;
      audioState.Complete = true;
      audioState.AudioSource = (AudioSource) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
      Action action = complete;
      if (action != null)
        action();
    }
  }
}
