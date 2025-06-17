using System;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [DoNotList]
  public class SimplexNodeWrapper<T> : FlowNode where T : SimplexNode
  {
    private T _simplexNode;

    private T simplexNode
    {
      get
      {
        if (_simplexNode == null)
        {
          _simplexNode = (T) Activator.CreateInstance(typeof (T));
          if (_simplexNode != null)
            GatherPorts();
        }
        return _simplexNode;
      }
    }

    public override string name => simplexNode != null ? simplexNode.name : "NULL";

    public override string description => simplexNode != null ? simplexNode.description : "NULL";

    public override void OnGraphStarted()
    {
      if (simplexNode == null)
        return;
      simplexNode.OnGraphStarted();
    }

    public override void OnGraphPaused()
    {
      if (simplexNode == null)
        return;
      simplexNode.OnGraphPaused();
    }

    public override void OnGraphUnpaused()
    {
      if (simplexNode == null)
        return;
      simplexNode.OnGraphUnpaused();
    }

    public override void OnGraphStoped()
    {
      if (simplexNode == null)
        return;
      simplexNode.OnGraphStoped();
    }

    protected override void RegisterPorts()
    {
      if (simplexNode == null)
        return;
      simplexNode.RegisterPorts(this);
    }
  }
}
