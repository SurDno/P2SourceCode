// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Toggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Togglers")]
  [Description("When In is called, calls On or Off depending on the current toggle state. Whenever Toggle input is called the state changes.")]
  public class Toggle : FlowControlNode
  {
    public bool open = true;
    private bool original;

    public override string name => base.name + " " + (this.open ? "[ON]" : "[OFF]");

    public override void OnGraphStarted() => this.original = this.open;

    public override void OnGraphStoped() => this.open = this.original;

    protected override void RegisterPorts()
    {
      FlowOutput tOut = this.AddFlowOutput("On");
      FlowOutput fOut = this.AddFlowOutput("Off");
      this.AddFlowInput("In", (FlowHandler) (() => this.Call(this.open ? tOut : fOut)));
      this.AddFlowInput(nameof (Toggle), (FlowHandler) (() => this.open = !this.open));
    }
  }
}
