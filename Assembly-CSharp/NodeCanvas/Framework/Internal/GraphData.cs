using System;
using System.Collections.Generic;

namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public class GraphData
  {
    public List<Node> nodes = [];
    public List<Connection> connections = [];
  }
}
