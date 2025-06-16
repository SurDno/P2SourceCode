using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Description("Create a collection of <T> objects")]
  public class CreateCollection<T> : VariableNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;

    public int portCount
    {
      get => _portCount;
      set => _portCount = value;
    }

    public override void SetVariable(object o)
    {
    }

    protected override void RegisterPorts()
    {
      List<ValueInput<T>> ins = new List<ValueInput<T>>();
      for (int index = 0; index < portCount; ++index)
        ins.Add(AddValueInput<T>("Element" + index));
      AddValueOutput("Collection", () => ins.Select(p => p.value).ToArray());
    }
  }
}
