using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class Instantiate<T> : CallableFunctionNode<T, T, Vector3, Quaternion, Transform> where T : Object
  {
    public override T Invoke(T original, Vector3 position, Quaternion rotation, Transform parent)
    {
      return (Object) original == (Object) null ? default (T) : Object.Instantiate<T>(original, position, rotation, parent);
    }
  }
}
