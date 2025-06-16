using System;
using System.Collections.Generic;

namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public class GraphData
  {
    public List<Node> nodes = new List<Node>();
    public List<Connection> connections = new List<Connection>();
  }
}
