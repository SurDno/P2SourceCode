using System;
using System.Collections.Generic;

namespace NodeCanvas.Framework.Internal;

[Serializable]
public class GraphData {
	public List<Node> nodes = new();
	public List<Connection> connections = new();
}