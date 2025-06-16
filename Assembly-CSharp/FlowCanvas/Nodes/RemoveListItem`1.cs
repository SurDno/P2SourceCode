﻿using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Lists")]
public class RemoveListItem<T> : CallableFunctionNode<IList<T>, List<T>, T> {
	public override IList<T> Invoke(List<T> list, T item) {
		list.Remove(item);
		return list;
	}
}