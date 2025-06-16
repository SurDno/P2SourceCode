using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Self")]
  [Description("Returns the Owner GameObject")]
  public class OwnerVariable : VariableNode
  {
    public override string name => "<size=20>SELF</size>";

    protected override void RegisterPorts()
    {
      this.AddValueOutput<GameObject>("Value", (ValueHandler<GameObject>) (() => (bool) (Object) this.graphAgent ? this.graphAgent.gameObject : (GameObject) null));
    }

    public override void SetVariable(object o)
    {
    }
  }
}
