using System;
using System.Collections.Generic;

namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public class GraphData
  {
    public List<NodeCanvas.Framework.Node> nodes = new List<NodeCanvas.Framework.Node>();
    public List<Connection> connections = new List<Connection>();
  }
}
