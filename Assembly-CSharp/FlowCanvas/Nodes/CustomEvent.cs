// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CustomEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using ParadoxNotion.Design;

#nullable disable
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
