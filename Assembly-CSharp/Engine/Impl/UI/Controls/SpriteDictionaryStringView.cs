using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class SpriteDictionaryStringView : StringView {
	[SerializeField] private SpriteView view;
	[SerializeField] private StringSpritePair[] dictionary;

	public override void SkipAnimation() { }

	protected override void ApplyStringValue() {
		if (dictionary == null || view == null)
			return;
		Sprite sprite = null;
		foreach (var stringSpritePair in dictionary)
			if (stringSpritePair.Key == StringValue) {
				sprite = stringSpritePair.Value;
				break;
			}

		view.SetValue(sprite, false);
	}
}