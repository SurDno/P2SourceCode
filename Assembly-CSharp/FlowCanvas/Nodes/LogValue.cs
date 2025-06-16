using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class LogValue : CallableActionNode<object>
  {
    public override void Invoke(object obj) => Debug.Log(obj);
  }
}
