using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a string value. The Default output is called if there is no other matching output same as the input value")]
  [ContextDefinedInputs(typeof (string))]
  public class SwitchString : FlowControlNode
  {
    public List<string> comparisonOutputs = [];

    protected override void RegisterPorts()
    {
      ValueInput<string> name = AddValueInput<string>("Value");
      List<FlowOutput> outs = [];
      for (int index = 0; index < comparisonOutputs.Count; ++index)
        outs.Add(AddFlowOutput(string.Format("\"{0}\"", comparisonOutputs[index]), index.ToString()));
      FlowOutput def = AddFlowOutput("Default");
      AddFlowInput("In", () =>
      {
        string str = name.value;
        if (str == null)
        {
          def.Call();
        }
        else
        {
          bool flag = false;
          for (int index = 0; index < comparisonOutputs.Count; ++index)
          {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(comparisonOutputs[index]))
            {
              outs[index].Call();
              flag = true;
            }
            else if (comparisonOutputs[index].Trim().ToLower() == str.Trim().ToLower())
            {
              outs[index].Call();
              flag = true;
            }
          }
          if (flag)
            return;
          def.Call();
        }
      });
    }
  }
}
