using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Category("Functions/Utility")]
public class GetComponent<T> : PureFunctionNode<T, GameObject> where T : Component {
	private T _component;

	public override T Invoke(GameObject gameObject) {
		if (gameObject == null)
			return default;
		if (_component == null || _component.gameObject != gameObject)
			_component = gameObject.GetComponent<T>();
		return _component;
	}
}