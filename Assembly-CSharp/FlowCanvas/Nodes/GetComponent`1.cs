using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Utility")]
  public class GetComponent<T> : PureFunctionNode<T, GameObject> where T : Component
  {
    private T _component;

    public override T Invoke(GameObject gameObject)
    {
      if ((Object) gameObject == (Object) null)
        return default (T);
      if ((Object) this._component == (Object) null || (Object) this._component.gameObject != (Object) gameObject)
        this._component = gameObject.GetComponent<T>();
      return this._component;
    }
  }
}
