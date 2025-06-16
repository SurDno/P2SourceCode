using Engine.Source.Services.Profiles;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class SaveFileStringView : StringView {
	[SerializeField] private StringView view;
	[SerializeField] private string format;

	public override void SkipAnimation() {
		if (!(view != null))
			return;
		view.SkipAnimation();
	}

	protected override void ApplyStringValue() {
		if (!(view != null))
			return;
		view.StringValue = ProfilesUtility.ConvertSaveName(StringValue, format);
	}
}