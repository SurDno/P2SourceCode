using ParadoxNotion.Design;
using System;
using System.Linq;
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
      get => this._sourceInputUID;
      set => this._sourceInputUID = value;
    }

    private RelayValueInput<T> sourceInput
    {
      get
      {
        if (this._sourceInput == null)
        {
          this._sourceInput = (object) this.graph.GetAllNodesOfType<RelayValueInput<T>>().FirstOrDefault<RelayValueInput<T>>((Func<RelayValueInput<T>, bool>) (i => i.UID == this.sourceInputUID));
          if (this._sourceInput == null)
            this._sourceInput = new object();
        }
        return this._sourceInput as RelayValueInput<T>;
      }
      set => this._sourceInput = (object) value;
    }

    public override string name
    {
      get
      {
        return string.Format("{0}", this.sourceInput != null ? (object) this.sourceInput.ToString() : (object) "@ NONE");
      }
    }

    protected override void RegisterPorts()
    {
      this.AddValueOutput<T>("Value", (ValueHandler<T>) (() => this.sourceInput != null ? this.sourceInput.port.value : default (T)));
    }
  }
}
