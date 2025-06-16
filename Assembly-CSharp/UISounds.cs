using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISounds : MonoBehaviourInstance<UISounds> {
	[SerializeField] private AudioClip clickSound;
	private AudioSource audioSource;

	protected override void Awake() {
		base.Awake();
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayClickSound() {
		PlaySound(clickSound);
	}

	public void PlaySound(AudioClip sound) {
		if (sound == null)
			return;
		audioSource.PlayOneShot(sound);
	}
}