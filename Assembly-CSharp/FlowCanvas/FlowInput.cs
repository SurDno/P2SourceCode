using System;

namespace FlowCanvas
{
  public class FlowInput(FlowNode parent, string name, string id, FlowHandler pointer)
    : Port(parent, name, id) {
    public FlowHandler pointer { get; private set; } = pointer;

    public override Type type => typeof (void);
  }
}
