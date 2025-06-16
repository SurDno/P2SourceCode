using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Visibility")]
  [Category("Events/Object")]
  [Description("Calls events based on object's render visibility")]
  public class VisibilityEvents : EventNode<Transform>
  {
    private FlowOutput onVisible;
    private FlowOutput onInvisible;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[2]
      {
        "OnBecameVisible",
        "OnBecameInvisible"
      };
    }

    protected override void RegisterPorts()
    {
      this.onVisible = this.AddFlowOutput("Became Visible");
      this.onInvisible = this.AddFlowOutput("Became Invisible");
    }

    private void OnBecameVisible() => this.onVisible.Call();

    private void OnBecameInvisible() => this.onInvisible.Call();
  }
}
