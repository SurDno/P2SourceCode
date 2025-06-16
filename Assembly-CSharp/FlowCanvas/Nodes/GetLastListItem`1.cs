﻿using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes;

[Category("Functions/Lists")]
public class GetLastListItem<T> : PureFunctionNode<T, IList<T>> {
	public override T Invoke(IList<T> list) {
		return list.LastOrDefault();
	}
}