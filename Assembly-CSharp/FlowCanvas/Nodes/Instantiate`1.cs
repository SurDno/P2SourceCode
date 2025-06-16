using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class Instantiate<T> : CallableFunctionNode<T, T, Vector3, Quaternion, Transform> where T : Object
  {
    public override T Invoke(T original, Vector3 position, Quaternion rotation, Transform parent)
    {
      return original == null ? default (T) : Object.Instantiate(original, position, rotation, parent);
    }
  }
}
