using System;
using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlaySoundNode : FlowControlNode
  {
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("Fade", 0.0f)]
    private ValueInput<float> fadeTime;
    [Port("Use Pause", true)]
    private ValueInput<bool> usePause;

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
            output.Call();
          else
            SoundUtility.PlayAudioClip2D(clip, mixer, volumeInput.value, fadeTime.value, usePause.value, "(blueprint) " + graph.agent.name, (Action) (() => output.Call()));
        }
      });
    }
  }
}
