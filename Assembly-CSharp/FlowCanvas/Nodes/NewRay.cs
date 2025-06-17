using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Utilities/Constructors")]
  public class NewRay : PureFunctionNode<Ray, Vector3, Vector3>
  {
    public override Ray Invoke(Vector3 origin, Vector3 direction) => new(origin, direction);
  }
}
