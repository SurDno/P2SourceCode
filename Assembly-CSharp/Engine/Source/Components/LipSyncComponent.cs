using System;
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
      get => audioState != null && audioState.AudioSource != null;
    }

    public bool IsIndoor
    {
      get
      {
        LocationItemComponent component = Owner?.GetComponent<LocationItemComponent>();
        return component != null && component.IsIndoor;
      }
    }

    public void Play(ILipSyncObject lipSync, bool usePause)
    {
      Stop();
      AudioMixerGroup mixer = IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if (mixer == null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if (clip == null)
        return;
      audioState = SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, usePause, Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        audioState = null;
      }));
      AddSubtitles(lipSyncInfo.Tag, audioState);
      PlayLipSync(audioState, lipSyncInfo);
    }

    public void Play3D(ILipSyncObject lipSync, bool usePause)
    {
      Play3D(lipSync, 1f, 30f, usePause);
    }

    public void Play3D(
      ILipSyncObject lipSync,
      float minDistance,
      float maxDistance,
      bool usePause)
    {
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      if (gameObject == null)
      {
        Debug.LogWarning(Owner.Name + ": gameobject not found");
      }
      else
      {
        Transform transform = gameObject.transform;
        Pivot component = gameObject.GetComponent<Pivot>();
        if (component == null)
          Debug.LogWarning(Owner.Name + ": Pivot not found");
        if (component?.Head != null)
          transform = component?.Head?.transform;
        Play(lipSync, transform, minDistance, maxDistance, usePause);
      }
    }

    public void Play3D(ILipSyncObject lipSync, GameObject customSource, bool usePause)
    {
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      if (gameObject == null)
      {
        Debug.LogWarning(Owner.Name + ": gameobject not found");
      }
      else
      {
        Transform transform = gameObject.transform;
        Pivot component = gameObject?.GetComponent<Pivot>();
        if (component == null)
          Debug.LogWarning(Owner.Name + ": Pivot not found");
        if (component?.Head != null)
          transform = component?.Head?.transform;
        Play(lipSync, transform, customSource, usePause);
      }
    }

    public void Play(ILipSyncObject lipSync, Transform transform, bool usePause)
    {
      Play(lipSync, transform, 1f, 30f, usePause);
    }

    public void Play(
      ILipSyncObject lipSync,
      Transform transform,
      float minDistance,
      float maxDistance,
      bool usePause)
    {
      Stop();
      AudioMixerGroup mixer = IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if (mixer == null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync, lastIndex, out lastIndex);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if (clip == null)
        return;
      audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, 1f, minDistance, maxDistance, false, 0.0f, usePause, Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        audioState = null;
      }));
      AddSubtitles(lipSyncInfo.Tag, audioState);
      PlayLipSync(audioState, lipSyncInfo);
    }

    public void Play(
      ILipSyncObject lipSync,
      Transform transform,
      GameObject customSource,
      bool usePause)
    {
      Stop();
      AudioMixerGroup mixer = IsIndoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcSpeechOutdoorMixer;
      if (mixer == null)
        return;
      LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync, lastIndex, out lastIndex);
      if (lipSyncInfo == null)
        return;
      AudioClip clip = lipSyncInfo.Clip.Value;
      if (clip == null)
        return;
      audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, 1f, false, 0.0f, customSource, usePause, Owner.GetInfo(), (Action) (() =>
      {
        Action playCompleteEvent = PlayCompleteEvent;
        if (playCompleteEvent != null)
          playCompleteEvent();
        audioState = null;
      }));
      AddSubtitles(lipSyncInfo.Tag, audioState);
      PlayLipSync(audioState, lipSyncInfo);
    }

    private void PlayLipSync(AudioState audioState, LipSyncInfo lipsync)
    {
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableLipSync)
        return;
      GameObject gameObject = ((IEntityView) Owner).GameObject;
      if (gameObject == null)
        return;
      byte[] data = lipsync.Data;
      if (data == null)
        return;
      Lipsync component = gameObject.GetComponent<Lipsync>();
      if (component == null)
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
      Play(template, true);
    }

    public override void OnAdded()
    {
      ((IEntityView) Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
      base.OnAdded();
    }

    public override void OnRemoved()
    {
      ((IEntityView) Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
    }

    private void OnGameObjectChangedEvent()
    {
      if (!(((IEntityView) Owner).GameObject == null))
        return;
      Stop();
    }

    public void Stop()
    {
      if (audioState != null)
      {
        audioState.Complete = true;
        if (audioState.AudioSource != null)
          audioState.AudioSource.Stop();
        audioState = null;
      }
      ServiceLocator.GetService<SubtitlesService>().RemoveSubtitles(Owner);
    }

    private void AddSubtitles(string tag, AudioState state)
    {
      if ((EngineApplication.PlayerPosition with
          {
            y = 0.0f
          } - ((IEntityView) Owner).Position with
          {
            y = 0.0f
          }).magnitude > (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.EntitySubtitlesDistanceMax)
        return;
      ServiceLocator.GetService<SubtitlesService>().AddSubtitles(Owner, tag, state, ((IEntityView) Owner).GameObject);
    }
  }
}
