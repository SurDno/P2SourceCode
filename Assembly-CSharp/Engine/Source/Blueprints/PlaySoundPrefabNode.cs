using System;
using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlaySoundPrefabNode : FlowControlNode
  {
    [Port("Prefab")]
    private ValueInput<GameObject> prefabInput;
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("Fade", 0.0f)]
    private ValueInput<float> fadeTimeInput;
    [Port("Use Pause", true)]
    private ValueInput<bool> usePause;
    [Port("Target")]
    private ValueInput<GameObject> targetInput;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        AudioClip clip = clipInput.value;
        if (clip == null)
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
            GameObject gameObject = targetInput.value;
            if (gameObject == null)
            {
              output.Call();
            }
            else
            {
              Transform transform = gameObject.transform;
              AudioState audioState = SoundUtility.PlayAudioClip(prefabInput.value, transform, clip, mixer, volumeInput.value, fadeTimeInput.value, usePause.value, "(blueprint) " + graph.agent.name, (Action) (() => output.Call()));
              if (!propagationInput.value)
                return;
              audioState.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = transform;
            }
          }
        }
      });
    }
  }
}
