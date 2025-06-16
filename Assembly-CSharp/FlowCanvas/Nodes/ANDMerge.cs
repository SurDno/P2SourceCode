using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("AND")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when all inputs are called together in the same frame")]
  public class ANDMerge : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 2;
    private FlowOutput fOut;
    private bool[] calls;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    protected override void RegisterPorts()
    {
      calls = new bool[portCount];
      fOut = AddFlowOutput("Out");
      for (int index = 0; index < portCount; ++index)
      {
        int i = index;
        AddFlowInput(i.ToString(), () =>
        {
          calls[i] = true;
          Check();
        });
      }
    }

    private void Check()
    {
      StartCoroutine(Reset());
      for (int index = 0; index < calls.Length; ++index)
      {
        if (!calls[index])
          return;
      }
      fOut.Call();
    }

    private IEnumerator Reset()
    {
      yield return null;
      for (int i = 0; i < calls.Length; ++i)
        calls[i] = false;
    }
  }
}
