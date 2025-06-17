using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on an enum value.\nThere are 2 ways to set the Enum type:\n1) Drag and Drop an enum connection on top of the node.\n2) Select the type after it has been added in the Prefered Types Editor Window.")]
  [ContextDefinedInputs(typeof (Enum))]
  public class SwitchEnum : FlowControlNode
  {
    [SerializeField]
    private Type _type = null;

    protected override void RegisterPorts()
    {
      if (_type == null)
        return;
      ValueInput e = AddValueInput(_type.Name, _type);
      List<FlowOutput> outs = [];
      foreach (string name in Enum.GetNames(_type))
        outs.Add(AddFlowOutput(name));
      AddFlowInput("In", () => outs[(int) Enum.Parse(e.value.GetType(), e.value.ToString())].Call());
    }
  }
}
