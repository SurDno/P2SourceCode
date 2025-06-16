using System;
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
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("Fade", 0.0f)]
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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        LipSyncObject lipSync = lipSyncInput.value.Value;
        if (lipSync == null)
        {
          output.Call();
        }
        else
        {
          Transform target = targetInput.value;
          if (target == null)
          {
            output.Call();
          }
          else
          {
            GameObject prefab = prefabInput.value;
            if (prefab == null)
            {
              output.Call();
            }
            else
            {
              AudioMixerGroup mixer = mixerInput.value;
              if (mixer == null)
              {
                output.Call();
              }
              else
              {
                LipSyncInfo lipSyncInfo = LipSyncUtility.GetLipSyncInfo(lipSync);
                if (lipSyncInfo == null)
                {
                  output.Call();
                }
                else
                {
                  AudioClip clip = lipSyncInfo.Clip.Value;
                  if (clip == null)
                  {
                    output.Call();
                  }
                  else
                  {
                    AudioState state = SoundUtility.PlayAudioClip(prefab, target, clip, mixer, volumeInput.value, fadeTimeInput.value, true, "(blueprint) " + graph.agent.name, (Action) (() => output.Call()));
                    if (propagationInput.value)
                      state.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = target;
                    AddSubtitles(lipSyncInfo.Tag, state, target);
                  }
                }
              }
            }
          }
        }
      });
    }

    private void AddSubtitles(string tag, AudioState state, Transform target)
    {
      Vector3 playerPosition = EngineApplication.PlayerPosition with
      {
        y = 0.0f
      };
      Vector3 position = target.position with { y = 0.0f };
      if (!forcedSubtitles.value && (playerPosition - position).magnitude > (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.BleuprintSubtitlesDistanceMax)
        return;
      ServiceLocator.GetService<SubtitlesService>().AddSubtitles(null, tag, state, graphAgent);
    }
  }
}
