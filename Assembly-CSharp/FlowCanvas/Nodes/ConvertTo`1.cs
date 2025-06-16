using ParadoxNotion.Design;
using System;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Converters")]
  public class ConvertTo<T> : PureFunctionNode<T, IConvertible> where T : IConvertible
  {
    public override T Invoke(IConvertible obj) => (T) Convert.ChangeType((object) obj, typeof (T));
  }
}
