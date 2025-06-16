using ParadoxNotion.Design;
using ParadoxNotion.Services;

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
      BlueprintManager.current.onApplicationQuit += ApplicationQuit;
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onApplicationQuit -= ApplicationQuit;
    }

    private void ApplicationQuit() => quit.Call();

    protected override void RegisterPorts() => quit = AddFlowOutput("Out");
  }
}
