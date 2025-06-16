using System;
using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlaySound3dPrefabNode : FlowControlNode
  {
    [Port("Prefab")]
    private ValueInput<GameObject> sourcePrefab;
    [Port("Volume", 1f)]
    private ValueInput<float> volumeInput;
    [Port("FadeTime", 0.0f)]
    private ValueInput<float> fadeTime;
    [Port("Use Pause", true)]
    private ValueInput<bool> usePause;
    [Port("Target")]
    private ValueInput<GameObject> target;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Propagation")]
    private ValueInput<bool> propagationInput;
    private AudioSource source;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        AudioClip clip = clipInput.value;
        if (!((UnityEngine.Object) clip != (UnityEngine.Object) null))
          return;
        AudioMixerGroup mixer = mixerInput.value;
        if ((UnityEngine.Object) mixer != (UnityEngine.Object) null)
        {
          GameObject gameObject = target.value;
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
          {
            float unscaledTime = Time.unscaledTime;
            Transform transform = gameObject.transform;
            AudioState audioState = SoundUtility.PlayAudioClip(sourcePrefab.value, transform, clip, mixer, volumeInput.value, fadeTime.value, usePause.value, "(blueprint) " + graph.agent.name, (Action) (() => output.Call()));
            if (propagationInput.value && (UnityEngine.Object) transform != (UnityEngine.Object) null)
              audioState.AudioSource.gameObject.AddComponent<SPAudioSource>().Origin = transform;
          }
        }
      });
    }
  }
}
