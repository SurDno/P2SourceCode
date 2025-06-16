using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Lists")]
public class SetListItem<T> : CallableFunctionNode<IList<T>, IList<T>, int, T> {
	public override IList<T> Invoke(IList<T> list, int index, T item) {
		try {
			list[index] = item;
			return list;
		} catch {
			return null;
		}
	}
}