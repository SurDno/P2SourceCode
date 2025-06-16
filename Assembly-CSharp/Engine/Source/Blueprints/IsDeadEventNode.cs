using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsDeadEventNode : EventNode<FlowScriptController>
  {
    private FlowOutput deadOutput;
    private FlowOutput resurrectOutput;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged += new Action<bool>(this.OnIsDeadChanged);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged -= new Action<bool>(this.OnIsDeadChanged);
      base.OnGraphStoped();
    }

    private void OnIsDeadChanged(bool visible)
    {
      if (visible)
        this.deadOutput.Call();
      else
        this.resurrectOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.deadOutput = this.AddFlowOutput("Dead");
      this.resurrectOutput = this.AddFlowOutput("Resurrect");
    }
  }
}
