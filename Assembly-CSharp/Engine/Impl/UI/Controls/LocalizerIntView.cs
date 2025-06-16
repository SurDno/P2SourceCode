using Engine.Behaviours.Localization;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class LocalizerIntView : IntView {
	[SerializeField] private Localizer localizer;
	[SerializeField] private string signaturePrefix;
	[SerializeField] private string signatureSuffix;

	public override void SkipAnimation() { }

	protected override void ApplyIntValue() {
		if (localizer == null)
			return;
		localizer.Signature = signaturePrefix + IntValue + signatureSuffix;
	}
}