using ParadoxNotion.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Description("Create a collection of <T> objects")]
  public class CreateCollection<T> : VariableNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    public override void SetVariable(object o)
    {
    }

    protected override void RegisterPorts()
    {
      List<ValueInput<T>> ins = new List<ValueInput<T>>();
      for (int index = 0; index < this.portCount; ++index)
        ins.Add(this.AddValueInput<T>("Element" + index.ToString()));
      this.AddValueOutput<T[]>("Collection", (ValueHandler<T[]>) (() => ins.Select<ValueInput<T>, T>((Func<ValueInput<T>, T>) (p => p.value)).ToArray<T>()));
    }
  }
}
