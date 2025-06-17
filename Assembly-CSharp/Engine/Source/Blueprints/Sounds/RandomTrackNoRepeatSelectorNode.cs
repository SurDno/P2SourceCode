using System.Collections.Generic;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(typeof (AudioClip))]
  public class RandomTrackNoRepeatSelectorNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    public int previousIndex;
    private List<ValueInput<AudioClip>> inputs = [];

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
    private AudioClip Value()
    {
      int count = inputs.Count;
      int index = (previousIndex + Random.Range(0, count - 1) + 1) % count;
      AudioClip audioClip = inputs[index].value;
      previousIndex = index;
      return audioClip;
    }
  }
}
