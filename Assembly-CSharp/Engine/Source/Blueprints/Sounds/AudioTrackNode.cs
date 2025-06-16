using Engine.Common;
using Engine.Source.Audio;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class AudioTrackNode : FlowControlNode, IUpdatable
  {
    [Port("Value")]
    private ValueInput<bool> valueInput;
    [Port("Volume", new object[] {1f})]
    private ValueInput<float> volumeInput;
    [Port("FadeTime", new object[] {0.5f})]
    private ValueInput<float> fadeTimeInput;
    [Port("Loop")]
    private ValueInput<bool> loopInput;
    [Port("Mixer")]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Clip")]
    private ValueInput<AudioClip> clipInput;
    private AudioSource source;
    private bool run;
    private float progress;
    private float sleep;
    private float currentVolume;
    private float lastTime;
    private const float delayDestroy = 10f;

    public bool Play => this.valueInput.value;

    private void ComputeDestroy(float deltaTime)
    {
      if (!((Object) this.source != (Object) null))
        return;
      this.sleep += deltaTime;
      if ((double) this.sleep > 10.0)
      {
        this.sleep = 0.0f;
        Object.Destroy((Object) this.source.gameObject);
        this.source = (AudioSource) null;
      }
    }

    public bool Complete { get; private set; }

    public void Reset()
    {
      if ((Object) this.source == (Object) null || !this.Complete)
        return;
      this.Complete = false;
      this.progress = 0.0f;
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      this.lastTime = Time.time;
      InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.AddUpdatable((IUpdatable) this);
    }

    public override void OnGraphStoped()
    {
      InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.RemoveUpdatable((IUpdatable) this);
      base.OnGraphStoped();
      if (!((Object) this.source != (Object) null))
        return;
      Object.Destroy((Object) this.source.gameObject);
      this.source = (AudioSource) null;
    }

    public void ComputeUpdate()
    {
      float deltaTime = Time.time - this.lastTime;
      this.lastTime = Time.time;
      this.Update(deltaTime);
    }

    private void Update(float deltaTime)
    {
      if (this.Complete)
      {
        this.ComputeDestroy(deltaTime);
      }
      else
      {
        bool loop = this.loopInput.value;
        float fade = this.fadeTimeInput.value;
        if (this.Play && (Object) this.source == (Object) null)
        {
          AudioClip clip = this.clipInput.value;
          if ((Object) clip == (Object) null)
            return;
          AudioMixerGroup mixer = this.mixerInput.value;
          if ((Object) mixer == (Object) null)
            return;
          this.currentVolume = 0.0f;
          this.source = AudioTrackNode.CreateAudioSource(clip, mixer, 0.0f, loop);
          this.source.PlayAndCheck();
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ").Append(clip.name).Append(" , context : ").Append("(blueprint) ").Append(this.graph.agent.name));
        }
        if ((Object) this.source == (Object) null)
          return;
        if (this.run)
        {
          this.progress += deltaTime;
          if (loop)
          {
            this.progress %= this.source.clip.length;
          }
          else
          {
            if ((double) this.progress >= (double) this.source.clip.length)
            {
              this.Complete = true;
              this.run = false;
              this.source.Stop();
              return;
            }
            SoundUtility.ComputeFade(this.progress, this.source.clip.length, fade);
            this.source.volume = this.currentVolume * this.volumeInput.value;
          }
        }
        if (this.Play)
        {
          if (this.run)
          {
            if ((double) this.currentVolume != 1.0)
            {
              this.currentVolume = Mathf.Clamp01(this.currentVolume + deltaTime / fade);
              this.source.volume = this.currentVolume * this.volumeInput.value;
            }
          }
          else
          {
            this.run = true;
            this.currentVolume = 0.0f;
            this.source.volume = this.currentVolume * this.volumeInput.value;
            this.source.PlayAndCheck();
            SoundUtility.SetTime(this.source, this.progress);
          }
        }
        else if (this.run)
        {
          if ((double) this.currentVolume != 0.0)
          {
            this.currentVolume = Mathf.Clamp01(this.currentVolume - deltaTime / fade);
            this.source.volume = this.currentVolume * this.volumeInput.value;
          }
          else
          {
            this.run = false;
            this.source.Stop();
          }
        }
        if (this.run)
          this.sleep = 0.0f;
        else
          this.ComputeDestroy(deltaTime);
      }
    }

    private static AudioSource CreateAudioSource(
      AudioClip clip,
      AudioMixerGroup mixer,
      float volume,
      bool loop)
    {
      GameObject gameObject = UnityFactory.Create("[Sounds]", clip.name);
      float length = clip.length;
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.clip = clip;
      audioSource.volume = volume;
      audioSource.rolloffMode = AudioRolloffMode.Linear;
      audioSource.spatialBlend = 0.0f;
      audioSource.minDistance = 0.0f;
      audioSource.maxDistance = 0.0f;
      audioSource.outputAudioMixerGroup = mixer;
      audioSource.loop = loop;
      return audioSource;
    }
  }
}
