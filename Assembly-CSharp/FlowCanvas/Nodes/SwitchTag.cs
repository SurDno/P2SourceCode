using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on the tag of a GameObject value")]
  [ContextDefinedInputs(new System.Type[] {typeof (GameObject)})]
  public class SwitchTag : FlowControlNode
  {
    [SerializeField]
    private string[] _tagNames = (string[]) null;

    protected override void RegisterPorts()
    {
      ValueInput<GameObject> go = this.AddValueInput<GameObject>("Value");
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this._tagNames.Length; ++index)
        outs.Add(this.AddFlowOutput(this._tagNames[index], index.ToString()));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        for (int index = 0; index < this._tagNames.Length; ++index)
        {
          if (this._tagNames[index] == go.value.tag)
            outs[index].Call();
        }
      }));
    }
  }
}
