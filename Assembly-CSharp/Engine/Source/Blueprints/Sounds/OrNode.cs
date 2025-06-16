using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(new System.Type[] {typeof (bool)})]
  public class OrNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private List<ValueInput<bool>> inputs = new List<ValueInput<bool>>();

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
        this.inputs.Add(this.AddValueInput<bool>((index + 1).ToString()));
    }

    [Port("Value")]
    private bool Value()
    {
      bool flag = false;
      foreach (ValueInput<bool> input in this.inputs)
      {
        if (input.isConnected && input.value)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }
  }
}
