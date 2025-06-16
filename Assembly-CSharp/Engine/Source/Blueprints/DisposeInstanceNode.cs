using System;
using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DisposeInstanceNode : FlowControlNode
  {
    private ValueInput<IObject> instanceValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ((IDisposable) instanceValue.value)?.Dispose();
        output.Call();
      });
      instanceValue = AddValueInput<IObject>("Instance");
    }
  }
}
