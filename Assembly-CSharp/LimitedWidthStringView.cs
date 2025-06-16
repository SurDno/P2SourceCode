using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class LimitedWidthStringView : StringView {
	[SerializeField] private StringView view;
	[SerializeField] private LayoutElement layout;
	[SerializeField] private Text text;
	[SerializeField] private float maxWidth;

	public override void SkipAnimation() { }

	protected override void ApplyStringValue() {
		if (!(view != null))
			return;
		view.StringValue = StringValue;
		if (layout != null && text != null)
			layout.preferredWidth = text.preferredWidth > (double)maxWidth ? maxWidth : -1f;
	}
}