using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Description("Called when a custom event is received on target.\nTo send an event from code use:\n'FlowScriptController.SendEvent(string)'")]
  [Category("Events/Script")]
  public class CustomEvent : EventNode<FlowScriptController>
  {
    [RequiredField]
    public string eventName;
    private FlowOutput received;

    public override string name
    {
      get => base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", (object) this.eventName);
    }

    protected override string[] GetTargetMessageEvents()
    {
      return new string[1]{ "OnCustomEvent" };
    }

    protected override void RegisterPorts() => this.received = this.AddFlowOutput("Received");

    public void OnCustomEvent(EventData receivedEvent)
    {
      if (!(receivedEvent.name == this.eventName))
        return;
      this.received.Call();
    }
  }
}
