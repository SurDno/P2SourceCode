using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Utilities/Converters")]
public class ToList<T> : PureFunctionNode<List<T>, IList<T>> {
	public override List<T> Invoke(IList<T> list) {
		return list.ToList();
	}
}