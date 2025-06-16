using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(new System.Type[] {typeof (AudioClip)})]
  public class RandomTrackSelectorNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private List<ValueInput<AudioClip>> inputs = new List<ValueInput<AudioClip>>();

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.inputs.Clear();
      for (int index = 0; index < this._portCount; ++index)
        this.inputs.Add(this.AddValueInput<AudioClip>((index + 1).ToString()));
    }

    [Port("Value")]
    private AudioClip Value() => this.inputs[UnityEngine.Random.Range(0, this.inputs.Count)].value;
  }
}
