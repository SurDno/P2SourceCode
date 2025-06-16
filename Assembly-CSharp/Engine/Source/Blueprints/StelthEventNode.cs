using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class StelthEventNode : EventNode<FlowScriptController>
  {
    private FlowOutput showOutput;
    private FlowOutput hideOutput;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged += new Action<bool>(this.OnVisibleChanged);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged -= new Action<bool>(this.OnVisibleChanged);
      base.OnGraphStoped();
    }

    private void OnVisibleChanged(bool visible)
    {
      if (visible)
        this.showOutput.Call();
      else
        this.hideOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.showOutput = this.AddFlowOutput("Show");
      this.hideOutput = this.AddFlowOutput("Hide");
    }
  }
}
