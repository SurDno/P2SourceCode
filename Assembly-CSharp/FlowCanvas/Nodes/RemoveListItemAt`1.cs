using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Lists")]
public class RemoveListItemAt<T> : CallableFunctionNode<IList<T>, List<T>, int> {
	public override IList<T> Invoke(List<T> list, int index) {
		list.RemoveAt(index);
		return list;
	}
}