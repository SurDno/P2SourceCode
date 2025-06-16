using System;
using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("XOR")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when either A or B Input is called, but the other is not in the same frame.\n(A || B) && (A != B)")]
  [Obsolete]
  public class XORMerge : FlowControlNode
  {
    private bool a;
    private bool b;
    private FlowOutput fOut;

    protected override void RegisterPorts()
    {
      fOut = AddFlowOutput("Out");
      AddFlowInput("A", () =>
      {
        a = true;
        Check();
      });
      AddFlowInput("B", () =>
      {
        b = true;
        Check();
      });
    }

    private void Check()
    {
      if ((a || b) && a != b)
        fOut.Call();
      StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
      yield return null;
      a = false;
      b = false;
    }
  }
}
