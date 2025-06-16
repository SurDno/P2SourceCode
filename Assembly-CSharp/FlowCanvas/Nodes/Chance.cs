using ParadoxNotion.Design;

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
