using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

namespace FlowCanvas.Nodes
{
  [Name("On Application Pause")]
  [Category("Events/Application")]
  [Description("Called when the Application is paused or resumed")]
  public class ApplicationPauseEvent : EventNode
  {
    private FlowOutput pause;
    private bool isPause;

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onApplicationPause += new Action<bool>(this.ApplicationPause);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onApplicationPause -= new Action<bool>(this.ApplicationPause);
    }

    private void ApplicationPause(bool isPause)
    {
      this.isPause = isPause;
      this.pause.Call();
    }

    protected override void RegisterPorts()
    {
      this.pause = this.AddFlowOutput("Out");
      this.AddValueOutput<bool>("Is Pause", (ValueHandler<bool>) (() => this.isPause));
    }
  }
}
