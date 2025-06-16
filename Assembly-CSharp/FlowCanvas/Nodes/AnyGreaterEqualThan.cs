using ParadoxNotion.Design;
using System;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Math Operators/Any")]
  [Name(">=")]
  public class AnyGreaterEqualThan : PureFunctionNode<bool, IComparable, IComparable>
  {
    public override bool Invoke(IComparable a, IComparable b)
    {
      return a.CompareTo((object) b) == 1 || object.Equals((object) a, (object) b);
    }
  }
}
