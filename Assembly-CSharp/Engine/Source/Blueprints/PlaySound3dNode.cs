// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.PlaySound3dNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlaySound3dNode : FlowControlNode
  {
    [Port("Clip", null)]
    private ValueInput<AudioClip> clipInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", new object[] {1f})]
    private ValueInput<float> volumeInput;
    [Port("Spatialize")]
    private ValueInput<bool> spatialize;
    [Port("Fade", new object[] {0.0f})]
    private ValueInput<float> fadeTime;
    [Port("Use Pause", new object[] {true})]
    private ValueInput<bool> usePause;
    [Port("Target")]
    private ValueInput<GameObject> targetInput;
    [Port("MinDistance", new object[] {1f})]
    private ValueInput<float> minDistance;
    [Port("MaxDistance", new object[] {10f})]
    private ValueInput<float> maxDistance;
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
              AudioState audioState = SoundUtility.PlayAudioClip3D(transform, clip, mixer, this.volumeInput.value, this.minDistance.value, this.maxDistance.value, this.spatialize.value, this.fadeTime.value, this.usePause.value, "(blueprint) " + this.graph.agent.name, (Action) (() => output.Call()));
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
