using Engine.Source.Audio;
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
  public class PlaySoundPrefabNode : FlowControlNode
  {
    [Port("Prefab")]
    private ValueInput<GameObject> prefabInput;
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", new object[] {1f})]
    private ValueInput<float> volumeInput;
    [Port("Fade", new object[] {0.0f})]
    private ValueInput<float> fadeTimeInput;
    [Port("Use Pause", new object[] {true})]
    private ValueInput<bool> usePause;
    [Port("Target")]
    private ValueInput<GameObject> targetInput;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        AudioClip clip = this.clipInput.value;
        if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
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
            GameObject gameObject = this.targetInput.value;
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
            {
              output.Call();
            }
            else
            {
              Transform transform = gameObject.transform;
              AudioState audioState = SoundUtility.PlayAudioClip(this.prefabInput.value, transform, clip, mixer, this.volumeInput.value, this.fadeTimeInput.value, this.usePause.value, "(blueprint) " + this.graph.agent.name, (Action) (() => output.Call()));
              if (!this.propagationInput.value)
                return;
              audioState.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = transform;
            }
          }
        }
      }));
    }
  }
}
