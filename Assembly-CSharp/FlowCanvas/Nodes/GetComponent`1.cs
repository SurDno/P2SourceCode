using ParadoxNotion.Design;

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
      if ((Object) _component == (Object) null || (Object) _component.gameObject != (Object) gameObject)
        _component = gameObject.GetComponent<T>();
      return _component;
    }
  }
}
