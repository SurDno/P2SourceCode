using System;
using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlaySound3dNode : FlowControlNode
  {
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("Spatialize")]
    private ValueInput<bool> spatialize;
    [Port("Fade", 0.0f)]
    private ValueInput<float> fadeTime;
    [Port("Use Pause", true)]
    private ValueInput<bool> usePause;
    [Port("Target")]
    private ValueInput<GameObject> targetInput;
    [Port("MinDistance", 1f)]
    private ValueInput<float> minDistance;
    [Port("MaxDistance", 10f)]
    private ValueInput<float> maxDistance;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        AudioClip clip = clipInput.value;
        if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
        {
          output.Call();
        }
        else
        {
          AudioMixerGroup mixer = mixerInput.value;
          if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
          {
            output.Call();
          }
          else
          {
            GameObject gameObject = targetInput.value;
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
            {
              output.Call();
            }
            else
            {
              Transform transform = gameObject.transform;
              AudioState audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, volumeInput.value, minDistance.value, maxDistance.value, spatialize.value, fadeTime.value, usePause.value, "(blueprint) " + graph.agent.name, (Action) (() => output.Call()));
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
