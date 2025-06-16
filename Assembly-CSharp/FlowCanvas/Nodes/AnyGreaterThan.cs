using ParadoxNotion.Design;
using System;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name(">")]
  public class AnyGreaterThan : PureFunctionNode<bool, IComparable, IComparable>
  {
    public override bool Invoke(IComparable a, IComparable b) => a.CompareTo((object) b) == 1;
  }
}
