using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Self")]
  [Description("Returns the Owner GameObject")]
  public class OwnerVariable : VariableNode
  {
    public override string name => "<size=20>SELF</size>";

    protected override void RegisterPorts()
    {
      AddValueOutput("Value", (ValueHandler<GameObject>) (() => (bool) (Object) graphAgent ? graphAgent.gameObject : (GameObject) null));
    }

    public override void SetVariable(object o)
    {
    }
  }
}
