// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Chance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Filters")]
  [Description("Filter the Flow based on a chance of 0 to 1 for 0% - 100%")]
  [ContextDefinedInputs(new System.Type[] {typeof (float)})]
  public class Chance : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      ValueInput<float> c = this.AddValueInput<float>("Percentage");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) >= (double) c.value)
          return;
        o.Call();
      }));
    }
  }
}
