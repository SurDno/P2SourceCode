using System.Linq;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Description("Returns the chosen RelayInput source value.\nOnly RelayInputs of the same (T) type can be chosen.")]
  [Category("Flow Controllers/Relay")]
  public class RelayValueOutput<T> : FlowControlNode
  {
    [SerializeField]
    private string _sourceInputUID;
    private object _sourceInput;

    private string sourceInputUID
    {
      get => _sourceInputUID;
      set => _sourceInputUID = value;
    }

    private RelayValueInput<T> sourceInput
    {
      get
      {
        if (_sourceInput == null)
        {
          _sourceInput = graph.GetAllNodesOfType<RelayValueInput<T>>().FirstOrDefault(i => i.UID == sourceInputUID);
          if (_sourceInput == null)
            _sourceInput = new object();
        }
        return _sourceInput as RelayValueInput<T>;
      }
      set => _sourceInput = value;
    }

    public override string name => string.Format("{0}", sourceInput != null ? sourceInput.ToString() : (object) "@ NONE");

    protected override void RegisterPorts()
    {
      AddValueOutput("Value", () => sourceInput != null ? sourceInput.port.value : default (T));
    }
  }
}
