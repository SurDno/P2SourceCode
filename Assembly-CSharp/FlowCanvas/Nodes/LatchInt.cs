using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Latch Integer")]
  [Category("Flow Controllers/Flow Convert")]
  [Description("Convert a Flow signal to an integer value")]
  [ContextDefinedInputs(typeof (int))]
  public class LatchInt : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;
    private int latched;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    protected override void RegisterPorts()
    {
      FlowOutput o = AddFlowOutput("Out");
      for (int index = 0; index < portCount; ++index)
      {
        int i = index;
        AddFlowInput(i.ToString(), () =>
        {
          latched = i;
          o.Call();
        });
      }
      AddValueOutput("Value", () => latched);
    }
  }
}
