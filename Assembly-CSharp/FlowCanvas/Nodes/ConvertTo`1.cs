using System;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Converters")]
  public class ConvertTo<T> : PureFunctionNode<T, IConvertible> where T : IConvertible
  {
    public override T Invoke(IConvertible obj) => (T) Convert.ChangeType(obj, typeof (T));
  }
}
