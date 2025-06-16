using System.Collections.Generic;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  [ContextDefinedInputs(typeof (bool))]
  public class OrNode : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private List<ValueInput<bool>> inputs = new List<ValueInput<bool>>();

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
        inputs.Add(AddValueInput<bool>((index + 1).ToString()));
    }

    [Port("Value")]
    private bool Value()
    {
      bool flag = false;
      foreach (ValueInput<bool> input in inputs)
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
