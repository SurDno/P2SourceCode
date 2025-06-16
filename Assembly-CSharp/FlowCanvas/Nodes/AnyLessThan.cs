using System;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name("<")]
  public class AnyLessThan : PureFunctionNode<bool, IComparable, IComparable>
  {
    public override bool Invoke(IComparable a, IComparable b) => a.CompareTo(b) == -1;
  }
}
