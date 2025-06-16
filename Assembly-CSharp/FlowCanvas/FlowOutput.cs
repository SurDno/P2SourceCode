using System;

namespace FlowCanvas
{
  public class FlowOutput : Port
  {
    public FlowOutput(FlowNode parent, string name, string id)
      : base(parent, name, id)
    {
    }

    public FlowHandler pointer { get; set; }

    public override Type type => typeof (void);

    public void Call()
    {
      if (this.pointer == null || this.parent.graph.isPaused)
        return;
      this.pointer();
    }

    public void BindTo(FlowInput target) => this.pointer = target.pointer;

    public void BindTo(FlowHandler call) => this.pointer = call;

    public void UnBind() => this.pointer = (FlowHandler) null;

    public void Append(FlowHandler action) => this.pointer += action;
  }
}
