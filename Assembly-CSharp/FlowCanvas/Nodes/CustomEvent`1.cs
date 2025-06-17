using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Description("Called when a custom value-based event is received on target.\nTo send an event from code use:\n'FlowScriptController.SendEvent<T>(string name, T value)'")]
  [Category("Events/Script")]
  public class CustomEvent<T> : EventNode<FlowScriptController>
  {
    [RequiredField]
    public string eventName;
    private FlowOutput received;
    private T receivedValue;

    public override string name => base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName);

    protected override string[] GetTargetMessageEvents()
    {
      return ["OnCustomEvent"];
    }

    protected override void RegisterPorts()
    {
      received = AddFlowOutput("Received");
      AddValueOutput("Event Value", () => receivedValue);
    }

    public void OnCustomEvent(EventData receivedEvent)
    {
      if (!(receivedEvent.name == eventName))
        return;
      if (receivedEvent is EventData<T>)
        receivedValue = (receivedEvent as EventData<T>).value;
      received.Call();
    }
  }
}
