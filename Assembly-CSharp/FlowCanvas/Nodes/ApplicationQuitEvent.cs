using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

namespace FlowCanvas.Nodes
{
  [Name("On Application Quit")]
  [Category("Events/Application")]
  [Description("Called when the Application quit")]
  public class ApplicationQuitEvent : EventNode
  {
    private FlowOutput quit;

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onApplicationQuit += new Action(this.ApplicationQuit);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onApplicationQuit -= new Action(this.ApplicationQuit);
    }

    private void ApplicationQuit() => this.quit.Call();

    protected override void RegisterPorts() => this.quit = this.AddFlowOutput("Out");
  }
}
