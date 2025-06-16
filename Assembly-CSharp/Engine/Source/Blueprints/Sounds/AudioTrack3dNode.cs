using Engine.Common;
using Engine.Source.Audio;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class AudioTrack3dNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("Spatialize")]
    private ValueInput<bool> spatializeInput;
    [Port("FadeTime", 0.5f)]
    private ValueInput<float> fadeTimeInput;
    [Port("MinDistance", 1f)]
    private ValueInput<float> minDistance;
    [Port("MaxDistance", 10f)]
    private ValueInput<float> maxDistance;
    [Port("Loop")]
    private ValueInput<bool> loopInput;
    [Port("Mixer")]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Clip")]
    private ValueInput<AudioClip> clipInput;
    [Port("Target")]
    private ValueInput<Transform> targetInput;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;
    private AudioSource source;
    private bool run;
    private float progress;
    private float sleep;
    private float currentVolume;
    private float lastTime;
    private const float delayDestroy = 10f;

    public bool Play => valueInput.value;

    private void ComputeDestroy(float deltaTime)
    {
      if (!(source != null))
        return;
      sleep += deltaTime;
      if (sleep > 10.0)
      {
        sleep = 0.0f;
        Object.Destroy(source.gameObject);
        source = null;
      }
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      lastTime = Time.time;
      InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.AddUpdatable(this);
    }

    public override void OnGraphStoped()
    {
      InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.RemoveUpdatable(this);
      base.OnGraphStoped();
      if (!(source != null))
        return;
      Object.Destroy(source.gameObject);
      source = null;
    }

    public bool Complete { get; private set; }

    public void Reset()
    {
      if (source == null || !Complete)
        return;
      Complete = false;
      progress = 0.0f;
    }

    public void ComputeUpdate()
    {
      float deltaTime = Time.time - lastTime;
      lastTime = Time.time;
      Update(deltaTime);
    }

    private void Update(float deltaTime)
    {
      if (Complete)
      {
        ComputeDestroy(deltaTime);
      }
      else
      {
        bool loop = loopInput.value;
        float fade = fadeTimeInput.value;
        if (Play)
        {
          if (source == null)
          {
            AudioClip clip = clipInput.value;
            if (clip == null)
              return;
            AudioMixerGroup mixer = mixerInput.value;
            if (mixer == null)
              return;
            currentVolume = 0.0f;
            source = CreateAudioSource(clip, mixer, 0.0f, spatializeInput.value, loop, targetInput.value, minDistance.value, maxDistance.value);
            if (propagationInput.value && targetInput.value != null)
              source.gameObject.AddComponent<SPAudioSource>().Origin = targetInput.value;
            source.PlayAndCheck();
            Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name).Append(" , context : ").Append("(blueprint) ").Append(graph.agent.name));
          }
          else if (targetInput.value != null)
          {
            source.transform.position = targetInput.value.position;
            source.transform.rotation = targetInput.value.rotation;
          }
        }
        if (source == null)
          return;
        if (run)
        {
          progress += deltaTime;
          if (loop)
          {
            progress %= source.clip.length;
          }
          else
          {
            if (progress >= (double) source.clip.length)
            {
              Complete = true;
              run = false;
              source.Stop();
              return;
            }
            SoundUtility.ComputeFade(progress, source.clip.length, fade);
            source.volume = currentVolume * volumeInput.value;
          }
        }
        if (Play)
        {
          if (run)
          {
            if (currentVolume != 1.0)
            {
              currentVolume = Mathf.Clamp01(currentVolume + deltaTime / fade);
              source.volume = currentVolume * volumeInput.value;
            }
          }
          else
          {
            run = true;
            currentVolume = 0.0f;
            source.volume = currentVolume * volumeInput.value;
            source.PlayAndCheck();
            SoundUtility.SetTime(source, progress);
          }
        }
        else if (run)
        {
          if (currentVolume != 0.0)
          {
            currentVolume = Mathf.Clamp01(currentVolume - deltaTime / fade);
            source.volume = currentVolume * volumeInput.value;
          }
          else
          {
            run = false;
            source.Stop();
          }
        }
        if (run)
          sleep = 0.0f;
        else
          ComputeDestroy(deltaTime);
      }
    }

    private static AudioSource CreateAudioSource(
      AudioClip clip,
      AudioMixerGroup mixer,
      float volume,
      bool spatialize,
      bool loop,
      Transform transform,
      float min,
      float max)
    {
      GameObject gameObject = UnityFactory.Create("[Sounds]", clip.name);
      if (transform != null)
        gameObject.transform.position = transform.position;
      float length = clip.length;
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.clip = clip;
      audioSource.volume = volume;
      audioSource.rolloffMode = AudioRolloffMode.Linear;
      audioSource.spatialBlend = 1f;
      audioSource.minDistance = min;
      audioSource.maxDistance = max;
      audioSource.outputAudioMixerGroup = mixer;
      audioSource.loop = loop;
      audioSource.spatialize = spatialize;
      return audioSource;
    }
  }
}
