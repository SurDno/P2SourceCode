using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using Inspectors;
using ParadoxNotion.Design;
using UnityEngine.Audio;

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
