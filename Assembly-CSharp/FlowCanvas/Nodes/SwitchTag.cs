using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on the tag of a GameObject value")]
  [ContextDefinedInputs(typeof (GameObject))]
  public class SwitchTag : FlowControlNode
  {
    [SerializeField]
    private string[] _tagNames = null;

    protected override void RegisterPorts()
    {
      ValueInput<GameObject> go = AddValueInput<GameObject>("Value");
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < _tagNames.Length; ++index)
        outs.Add(AddFlowOutput(_tagNames[index], index.ToString()));
      AddFlowInput("In", () =>
      {
        for (int index = 0; index < _tagNames.Length; ++index)
        {
          if (_tagNames[index] == go.value.tag)
            outs[index].Call();
        }
      });
    }
  }
}
