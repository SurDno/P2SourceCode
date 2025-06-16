using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayLipSyncAudio3dPrefabNode : FlowControlNode
  {
    [Port("LipSync", null)]
    private ValueInput<LipSyncObjectSerializable> lipSyncInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Prefab")]
    private ValueInput<GameObject> prefabInput;
    [Port("Volume", new object[] {1f})]
    private ValueInput<float> volumeInput;
    [Port("Fade", new object[] {0.0f})]
    private ValueInput<float> fadeTimeInput;
    [Port("Target")]
    private ValueInput<Transform> targetInput;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;
    [Port("ForcedSubtitles")]
    private ValueInput<bool> forcedSubtitles;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        LipSyncObject lipSync = this.lipSyncInput.value.Value;
        if (lipSync == null)
        {
          output.Call();
        }
        else
        {
          Transform target = this.targetInput.value;
          if ((UnityEngine.Object) target == (UnityEngine.Object) null)
          {
            output.Call();
          }
          else
          {
            GameObject prefab = this.prefabInput.value;
            if ((UnityEngine.Object) prefab == (UnityEngine.Object) null)
            {
              output.Call();
            }
            else
            {
              AudioMixerGroup mixer = this.mixerInput.value;
              if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
              {
                output.Call();
              }
              else
              {
                LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo((ILipSyncObject) lipSync);
                if (lipSyncInfo == null)
                {
                  output.Call();
                }
                else
                {
                  AudioClip clip = lipSyncInfo.Clip.Value;
                  if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
                  {
                    output.Call();
                  }
                  else
                  {
                    AudioState state = SoundUtility.PlayAudioClip(prefab, target, clip, mixer, this.volumeInput.value, this.fadeTimeInput.value, true, "(blueprint) " + this.graph.agent.name, (Action) (() => output.Call()));
                    if (this.propagationInput.value)
                      state.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = target;
                    this.AddSubtitles(lipSyncInfo.Tag, state, target);
                  }
                }
              }
            }
          }
        }
      }));
    }

    private void AddSubtitles(string tag, AudioState state, Transform target)
    {
      Vector3 playerPosition = EngineApplication.PlayerPosition with
      {
        y = 0.0f
      };
      Vector3 position = target.position with { y = 0.0f };
      if (!this.forcedSubtitles.value && (double) (playerPosition - position).magnitude > (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.BleuprintSubtitlesDistanceMax)
        return;
      ServiceLocator.GetService<SubtitlesService>().AddSubtitles((IEntity) null, tag, state, (UnityEngine.Object) this.graphAgent);
    }
  }
}
