using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("OR")]
  [Category("Flow Controllers/Flow Merge")]
  [Description("Calls Out when either input is called")]
  public class ORMerge : FlowControlNode, IMultiPortNode
  {
    private FlowOutput fOut;
    [SerializeField]
    private int _portCount = 2;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    protected override void RegisterPorts()
    {
      fOut = AddFlowOutput("Out");
      for (int index = 0; index < portCount; ++index)
        AddFlowInput(index.ToString(), () => fOut.Call());
    }
  }
}
