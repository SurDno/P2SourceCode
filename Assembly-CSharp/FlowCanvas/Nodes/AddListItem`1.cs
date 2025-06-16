using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Lists")]
public class AddListItem<T> : CallableFunctionNode<IList<T>, List<T>, T> {
	public override IList<T> Invoke(List<T> list, T item) {
		list.Add(item);
		return list;
	}
}