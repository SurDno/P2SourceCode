using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class DeltaTimedVector3 : PureFunctionNode<Vector3, Vector3, float>
  {
    public override Vector3 Invoke(Vector3 value, float multiplier)
    {
      return value * multiplier * Time.deltaTime;
    }
  }
}
