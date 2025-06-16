// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.TriggerEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
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
      this.enter = this.AddFlowOutput("Enter");
      this.stay = this.AddFlowOutput("Stay");
      this.exit = this.AddFlowOutput("Exit");
      this.AddValueOutput<GameObject>("Other", (ValueHandler<GameObject>) (() => this.other));
    }

    private void OnTriggerEnter(Collider other)
    {
      this.other = other.gameObject;
      this.enter.Call();
    }

    private void OnTriggerStay(Collider other)
    {
      this.other = other.gameObject;
      this.stay.Call();
    }

    private void OnTriggerExit(Collider other)
    {
      this.other = other.gameObject;
      this.exit.Call();
    }
  }
}
