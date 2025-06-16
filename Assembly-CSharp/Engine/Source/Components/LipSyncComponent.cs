using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Components
{
  [Factory(typeof (ILipSyncComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LipSyncComponent : EngineComponent, ILipSyncComponent, IComponent
  {
    private AudioState audioState;
    private int lastIndex;

    public event Action PlayCompleteEvent;

    [Inspected]
    public bool IsPlaying
    {
      get => this.audioState != null && (UnityEngine.Object) this.audioState.AudioSource != (UnityEngine.Object) null;
    }

    public bool IsIndoor
    {
      get
      {
        LocationItemComponent component = this.Owner?.GetComponent<LocationItemComponent>();
        return component != null && component.IsIndoor;
      }
    }

    public void Play(ILipSyncObject lipSync, bool usePause)
    {
      this.Stop();
      AudioMixerGroup mixer = this.IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
        return;
      this.audioState = SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, usePause, this.Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = this.PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        this.audioState = (AudioState) null;
      }));
      this.AddSubtitles(lipSyncInfo.Tag, this.audioState);
      this.PlayLipSync(this.audioState, lipSyncInfo);
    }

    public void Play3D(ILipSyncObject lipSync, bool usePause)
    {
      this.Play3D(lipSync, 1f, 30f, usePause);
    }

    public void Play3D(
      ILipSyncObject lipSync,
      float minDistance,
      float maxDistance,
      bool usePause)
    {
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.Owner.Name + ": gameobject not found"));
      }
      else
      {
        Transform transform = gameObject.transform;
        Pivot component = gameObject.GetComponent<Pivot>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          Debug.LogWarning((object) (this.Owner.Name + ": Pivot not found"));
        if ((UnityEngine.Object) component?.Head != (UnityEngine.Object) null)
          transform = component?.Head?.transform;
        this.Play(lipSync, transform, minDistance, maxDistance, usePause);
      }
    }

    public void Play3D(ILipSyncObject lipSync, GameObject customSource, bool usePause)
    {
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.Owner.Name + ": gameobject not found"));
      }
      else
      {
        Transform transform = gameObject.transform;
        Pivot component = gameObject?.GetComponent<Pivot>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          Debug.LogWarning((object) (this.Owner.Name + ": Pivot not found"));
        if ((UnityEngine.Object) component?.Head != (UnityEngine.Object) null)
          transform = component?.Head?.transform;
        this.Play(lipSync, transform, customSource, usePause);
      }
    }

    public void Play(ILipSyncObject lipSync, Transform transform, bool usePause)
    {
      this.Play(lipSync, transform, 1f, 30f, usePause);
    }

    public void Play(
      ILipSyncObject lipSync,
      Transform transform,
      float minDistance,
      float maxDistance,
      bool usePause)
    {
      this.Stop();
      AudioMixerGroup mixer = this.IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync, this.lastIndex, out this.lastIndex);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
        return;
      this.audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, 1f, minDistance, maxDistance, false, 0.0f, usePause, this.Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = this.PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        this.audioState = (AudioState) null;
      }));
      this.AddSubtitles(lipSyncInfo.Tag, this.audioState);
      this.PlayLipSync(this.audioState, lipSyncInfo);
    }

    public void Play(
      ILipSyncObject lipSync,
      Transform transform,
      GameObject customSource,
      bool usePause)
    {
      this.Stop();
      AudioMixerGroup mixer = this.IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync, this.lastIndex, out this.lastIndex);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
        return;
      this.audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, 1f, false, 0.0f, customSource, usePause, this.Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = this.PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        this.audioState = (AudioState) null;
      }));
      this.AddSubtitles(lipSyncInfo.Tag, this.audioState);
      this.PlayLipSync(this.audioState, lipSyncInfo);
    }

    private void PlayLipSync(AudioState audioState, LipSyncInfo lipsync)
    {
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableLipSync)
        return;
      GameObject gameObject = ((IEntityView) this.Owner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      byte[] data = lipsync.Data;
      if (data == null)
        return;
      Lipsync component = gameObject.GetComponent<Lipsync>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      component.Play(audioState, data);
    }

    [Inspected]
    private void PlayTest()
    {
      Guid id = new Guid("dccf3739-64a7-c054-ba71-6e6e9925eeed");
      ILipSyncObject template = ServiceLocator.GetService<ITemplateService>().GetTemplate<ILipSyncObject>(id);
      if (template == null)
        return;
      this.Play(template, true);
    }

    public override void OnAdded()
    {
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      base.OnAdded();
    }

    public override void OnRemoved()
    {
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
    }

    private void OnGameObjectChangedEvent()
    {
      if (!((UnityEngine.Object) ((IEntityView) this.Owner).GameObject == (UnityEngine.Object) null))
        return;
      this.Stop();
    }

    public void Stop()
    {
      if (this.audioState != null)
      {
        this.audioState.Complete = true;
        if ((UnityEngine.Object) this.audioState.AudioSource != (UnityEngine.Object) null)
          this.audioState.AudioSource.Stop();
        this.audioState = (AudioState) null;
      }
      ServiceLocator.GetService<SubtitlesService>().RemoveSubtitles(this.Owner);
    }

    private void AddSubtitles(string tag, AudioState state)
    {
      if ((double) (EngineApplication.PlayerPosition with
      {
        y = 0.0f
      } - ((IEntityView) this.Owner).Position with
      {
        y = 0.0f
      }).magnitude > (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.EntitySubtitlesDistanceMax)
        return;
      ServiceLocator.GetService<SubtitlesService>().AddSubtitles(this.Owner, tag, state, (UnityEngine.Object) ((IEntityView) this.Owner).GameObject);
    }
  }
}
