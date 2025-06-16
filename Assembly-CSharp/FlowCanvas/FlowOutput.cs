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
      if (pointer == null || parent.graph.isPaused)
        return;
      pointer();
    }

    public void BindTo(FlowInput target) => pointer = target.pointer;

    public void BindTo(FlowHandler call) => pointer = call;

    public void UnBind() => pointer = null;

    public void Append(FlowHandler action) => pointer += action;
  }
}
