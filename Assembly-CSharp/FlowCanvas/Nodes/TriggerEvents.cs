using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Trigger")]
  [Category("Events/Object")]
  [Description("Called when Trigger based event happen on target")]
  public class TriggerEvents : EventNode<Collider>
  {
    private GameObject other;
    private FlowOutput enter;
    private FlowOutput stay;
    private FlowOutput exit;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[3]
      {
        "OnTriggerEnter",
        "OnTriggerStay",
        "OnTriggerExit"
      };
    }

    protected override void RegisterPorts()
    {
      enter = AddFlowOutput("Enter");
      stay = AddFlowOutput("Stay");
      exit = AddFlowOutput("Exit");
      AddValueOutput("Other", () => other);
    }

    private void OnTriggerEnter(Collider other)
    {
      this.other = other.gameObject;
      enter.Call();
    }

    private void OnTriggerStay(Collider other)
    {
      this.other = other.gameObject;
      stay.Call();
    }

    private void OnTriggerExit(Collider other)
    {
      this.other = other.gameObject;
      exit.Call();
    }
  }
}
