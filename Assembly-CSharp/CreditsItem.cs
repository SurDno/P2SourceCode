using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class CreditsItem : MonoBehaviour {
	[SerializeField] private StringView[] lineViews;
	private int count;

	public void AddLine(string line) {
		line = line.Replace("\\n", "\n");
		var lineView = lineViews[count];
		lineView.StringValue = line;
		lineView.gameObject.SetActive(true);
		++count;
	}

	public float UpdateBottomEnd() {
		var transform = (RectTransform)this.transform;
		var preferredHeight = LayoutUtility.GetPreferredHeight(transform);
		var sizeDelta = transform.sizeDelta with {
			y = preferredHeight
		};
		transform.sizeDelta = sizeDelta;
		return preferredHeight - transform.anchoredPosition.y;
	}

	public bool IsFull() {
		return count >= lineViews.Length;
	}

	public void SetPosition(float value) {
		((RectTransform)transform).anchoredPosition = new Vector2(0.0f, -value);
	}
}