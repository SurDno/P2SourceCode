using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TOD_AudioAtDay : MonoBehaviour {
	public float fadeTime = 1f;
	private float lerpTime;
	private AudioSource audioComponent;
	private float audioVolume;

	protected void Start() {
		audioComponent = GetComponent<AudioSource>();
		audioVolume = audioComponent.volume;
		audioComponent.enabled = TOD_Sky.Instance.IsDay;
	}

	protected void Update() {
		lerpTime = Mathf.Clamp01(lerpTime + (TOD_Sky.Instance.IsDay ? 1f : -1f) * Time.deltaTime / fadeTime);
		audioComponent.volume = Mathf.Lerp(0.0f, audioVolume, lerpTime);
		audioComponent.enabled = audioComponent.volume > 0.0;
	}
}