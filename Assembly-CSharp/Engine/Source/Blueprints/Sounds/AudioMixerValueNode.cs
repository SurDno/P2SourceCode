// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.AudioMixerValueNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using Inspectors;
using ParadoxNotion.Design;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class AudioMixerValueNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Mixer")]
    private ValueInput<AudioMixerGroup> mixerInput;
    [Port("Name")]
    private ValueInput<string> nameInput;
    [Port("Min value")]
    private ValueInput<float> minValueInput;
    [Port("Max value")]
    private ValueInput<float> maxValueInput;

    [Inspected]
    public float Value => this.valueInput.value;

    [Inspected]
    public AudioMixerGroup Mixer => this.mixerInput.value;

    [Inspected]
    public string Name => this.nameInput.value;

    [Inspected]
    public float MinValue => this.minValueInput.value;

    [Inspected]
    public float MaxValue => this.maxValueInput.value;

    [Inspected]
    public FlowScriptController Agent => this.graph.agent;

    [Inspected]
    public bool Failed { get; set; }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<AudioMixerValueService>().AddNode(this);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<AudioMixerValueService>().RemoveNode(this);
      base.OnGraphStoped();
    }
  }
}
