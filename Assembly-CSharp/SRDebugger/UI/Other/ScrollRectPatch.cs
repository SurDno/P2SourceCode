using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other;

[RequireComponent(typeof(ScrollRect))]
[ExecuteInEditMode]
public class ScrollRectPatch : MonoBehaviour {
	public RectTransform Content;
	public Mask ReplaceMask;
	public RectTransform Viewport;

	private void Awake() {
		var component = GetComponent<ScrollRect>();
		component.content = Content;
		component.viewport = Viewport;
		if (!(ReplaceMask != null))
			return;
		var gameObject = ReplaceMask.gameObject;
		Destroy(gameObject.GetComponent<Graphic>());
		Destroy(gameObject.GetComponent<CanvasRenderer>());
		Destroy(ReplaceMask);
		gameObject.AddComponent<RectMask2D>();
	}
}