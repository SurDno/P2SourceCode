using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Behaviours.Weather;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Light))]
public class Lightning : MonoBehaviour {
	private const float soundSpeed = 331f;
	private static int index;
	private Animator animator;

	[SerializeField] [FormerlySerializedAs("_Audios")]
	private AudioClip[] audios;

	private AudioSource audioSource;
	private float distance;
	private float elapsed;
	private int flashType;
	private bool isEnded;
	private Light light;

	private float[,] lightningFlash = new float[8, 30] {
		{
			0.0f,
			30f,
			190f,
			110f,
			75f,
			50f,
			30f,
			15f,
			200f,
			40f,
			185f,
			25f,
			45f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			60f,
			5f,
			0.0f,
			50f,
			10f,
			0.0f,
			0.0f,
			0.0f,
			byte.MaxValue,
			180f,
			105f,
			100f,
			115f,
			230f,
			120f,
			50f,
			220f,
			170f,
			0.0f,
			145f,
			0.0f,
			45f,
			60f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			25f,
			15f,
			10f,
			200f,
			110f,
			80f,
			65f,
			50f,
			70f,
			245f,
			135f,
			110f,
			245f,
			55f,
			80f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			10f,
			15f,
			20f,
			130f,
			55f,
			20f,
			180f,
			45f,
			75f,
			50f,
			45f,
			90f,
			200f,
			180f,
			110f,
			40f,
			80f,
			245f,
			20f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			byte.MaxValue,
			byte.MaxValue,
			40f,
			byte.MaxValue,
			245f,
			85f,
			35f,
			20f,
			35f,
			30f,
			0.0f,
			45f,
			55f,
			220f,
			85f,
			5f,
			0.0f,
			95f,
			40f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			45f,
			55f,
			220f,
			85f,
			5f,
			0.0f,
			95f,
			40f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			byte.MaxValue,
			byte.MaxValue,
			40f,
			byte.MaxValue,
			245f,
			85f,
			35f,
			20f,
			35f,
			30f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}, {
			0.0f,
			byte.MaxValue,
			180f,
			105f,
			100f,
			115f,
			230f,
			120f,
			50f,
			220f,
			170f,
			0.0f,
			145f,
			0.0f,
			45f,
			60f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f,
			0.0f
		}
	};

	public void Start() {
		animator = gameObject.GetComponent<Animator>();
		if (animator == null)
			throw null;
		light = gameObject.GetComponent<Light>();
		if (light == null)
			throw null;
		audioSource = gameObject.GetComponent<AudioSource>();
		if (audioSource == null)
			throw null;
		var main = UnityEngine.Camera.main;
		if (main == null)
			throw null;
		distance = (main.transform.position - transform.position).magnitude;
		var delay = distance / 331f;
		audioSource.clip = audios[Random.Range(0, audios.Length)];
		audioSource.PlayDelayed(delay);
		flashType = Random.Range(0, lightningFlash.GetLength(0));
		gameObject.name = "Lightning_" + index;
		++index;
	}

	private void Update() {
		if (isEnded) {
			gameObject.SetActive(false);
			Destroy(gameObject);
		} else {
			var flag1 = false;
			var num = 0.0333f;
			if (elapsed > (double)(lightningFlash.GetLength(1) * num)) {
				flag1 = true;
				light.intensity = 0.0f;
			} else
				light.intensity = lightningFlash[flashType, (int)(elapsed / (double)num)] / byte.MaxValue;

			var flag2 = animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("End");
			if (flag1 & flag2 && !audioSource.isPlaying)
				isEnded = true;
			elapsed += Time.deltaTime;
			var main = UnityEngine.Camera.main;
			if (main == null)
				return;
			gameObject.transform.LookAt(gameObject.transform.position + main.transform.rotation * Vector3.back,
				main.transform.rotation * Vector3.up);
		}
	}
}