// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.PlayLipSyncAudioNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayLipSyncAudioNode : FlowControlNode
  {
    [Port("LipSync", null)]
    private ValueInput<LipSyncObjectSerializable> lipSyncInput;
    [Port("Mixer", null)]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Volume", new object[] {1f})]
    private ValueInput<float> volumeInput;
    [Port("Fade", new object[] {0.0f})]
    private ValueInput<float> fadeTime;

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
                AudioState state = SoundUtility.PlayAudioClip2D(clip, mixer, this.volumeInput.value, this.fadeTime.value, true, "(blueprint) " + this.graph.agent.name, (Action) (() => output.Call()));
                ServiceLocator.GetService<SubtitlesService>().AddSubtitles((IEntity) null, lipSyncInfo.Tag, state, (UnityEngine.Object) this.graphAgent);
              }
            }
          }
        }
      }));
    }
  }
}
