using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Delta Timed Float")]
  [Category("Functions/Utility")]
  public class DeltaTimed : PureFunctionNode<float, float, float>
  {
    public override float Invoke(float value, float multiplier)
    {
      return value * multiplier * Time.deltaTime;
    }
  }
}
