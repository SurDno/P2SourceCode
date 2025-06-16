using UnityEngine;

public class NGSS_ContactShadowsSource : MonoBehaviourInstance<NGSS_ContactShadowsSource> {
	public Light Light { get; private set; }

	protected override void Awake() {
		Light = GetComponent<Light>();
		base.Awake();
	}
}