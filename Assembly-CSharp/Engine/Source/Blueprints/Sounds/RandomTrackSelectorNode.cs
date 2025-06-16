using System.Collections.Generic;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(typeof (AudioClip))]
  public class RandomTrackSelectorNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private List<ValueInput<AudioClip>> inputs = new List<ValueInput<AudioClip>>();

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      inputs.Clear();
      for (int index = 0; index < _portCount; ++index)
        inputs.Add(AddValueInput<AudioClip>((index + 1).ToString()));
    }

    [Port("Value")]
    private AudioClip Value() => inputs[UnityEngine.Random.Range(0, inputs.Count)].value;
  }
}
