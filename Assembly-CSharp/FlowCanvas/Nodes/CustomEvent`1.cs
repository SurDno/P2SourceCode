// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CustomEvent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using ParadoxNotion.Design;

#nullable disable
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

    public override string name
    {
      get => base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", (object) this.eventName);
    }

    protected override string[] GetTargetMessageEvents()
    {
      return new string[1]{ "OnCustomEvent" };
    }

    protected override void RegisterPorts()
    {
      this.received = this.AddFlowOutput("Received");
      this.AddValueOutput<T>("Event Value", (ValueHandler<T>) (() => this.receivedValue));
    }

    public void OnCustomEvent(EventData receivedEvent)
    {
      if (!(receivedEvent.name == this.eventName))
        return;
      if (receivedEvent is EventData<T>)
        this.receivedValue = (receivedEvent as EventData<T>).value;
      this.received.Call();
    }
  }
}
