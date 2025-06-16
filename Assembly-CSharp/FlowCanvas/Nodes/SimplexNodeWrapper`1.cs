using ParadoxNotion.Design;
using System;

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
        if ((object) this._simplexNode == null)
        {
          this._simplexNode = (T) Activator.CreateInstance(typeof (T));
          if ((object) this._simplexNode != null)
            this.GatherPorts();
        }
        return this._simplexNode;
      }
    }

    public override string name
    {
      get => (object) this.simplexNode != null ? this.simplexNode.name : "NULL";
    }

    public override string description
    {
      get => (object) this.simplexNode != null ? this.simplexNode.description : "NULL";
    }

    public override void OnGraphStarted()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphStarted();
    }

    public override void OnGraphPaused()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphPaused();
    }

    public override void OnGraphUnpaused()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphUnpaused();
    }

    public override void OnGraphStoped()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.OnGraphStoped();
    }

    protected override void RegisterPorts()
    {
      if ((object) this.simplexNode == null)
        return;
      this.simplexNode.RegisterPorts((FlowNode) this);
    }
  }
}
