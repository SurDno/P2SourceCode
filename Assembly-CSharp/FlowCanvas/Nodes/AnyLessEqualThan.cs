using System;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name("<=")]
  public class AnyLessEqualThan : PureFunctionNode<bool, IComparable, IComparable>
  {
    public override bool Invoke(IComparable a, IComparable b)
    {
      return a.CompareTo(b) == -1 || Equals(a, b);
    }
  }
}
