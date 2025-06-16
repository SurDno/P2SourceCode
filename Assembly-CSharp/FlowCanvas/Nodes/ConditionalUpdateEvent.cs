using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Conditional Event")]
  [Category("Events/Other")]
  [Description("Checks the condition boolean input per frame and calls outputs when the value has changed")]
  public class ConditionalUpdateEvent : EventNode, IUpdatable
  {
    private FlowOutput becameTrue;
    private FlowOutput becameFalse;
    private ValueInput<bool> condition;
    private bool lastState;

    protected override void RegisterPorts()
    {
      this.becameTrue = this.AddFlowOutput("Became True");
      this.becameFalse = this.AddFlowOutput("Became False");
      this.condition = this.AddValueInput<bool>("Condition");
    }

    public void Update()
    {
      if (!this.condition.value)
      {
        if (!this.lastState)
          return;
        this.becameFalse.Call();
        this.lastState = false;
      }
      else if (!this.lastState)
      {
        this.becameTrue.Call();
        this.lastState = true;
      }
    }
  }
}
