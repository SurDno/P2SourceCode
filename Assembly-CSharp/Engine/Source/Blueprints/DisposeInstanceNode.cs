using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DisposeInstanceNode : FlowControlNode
  {
    private ValueInput<IObject> instanceValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ((IDisposable) this.instanceValue.value)?.Dispose();
        output.Call();
      }));
      this.instanceValue = this.AddValueInput<IObject>("Instance");
    }
  }
}
