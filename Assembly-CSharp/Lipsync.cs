using Engine.Source.Audio;
using Externals.LipSync;
using UnityEngine;

public class Lipsync : MonoBehaviour {
	[SerializeField] private float smoothTime = 0.15f;
	[SerializeField] private float vowelsSmoothTime = 0.5f;
	[SerializeField] private float consonantSmoothTime = 0.15f;

	[Header("You can use ComputeNpc to fill this!")] [SerializeField]
	private Animator animator;

	private CustomAnnoReader annoReader;
	private AudioState audioState;
	private int lipsyncLayer0Index;
	private int lipsyncLayer1Index;
	private int lipsyncLayer2Index;
	private float layer0Weigth;
	private float layer1Weigth;
	private float layer2Weigth;

	public void Play(AudioState audioState, byte[] data) {
		Stop();
		annoReader = new CustomAnnoReader(data);
		this.audioState = audioState;
	}

	public void Stop() {
		if (annoReader == null)
			return;
		annoReader = null;
	}

	private void Start() {
		lipsyncLayer0Index = animator.GetLayerIndex("Lipsync 0");
		lipsyncLayer1Index = animator.GetLayerIndex("Lipsync 1");
		lipsyncLayer2Index = animator.GetLayerIndex("Lipsync 2");
	}

	private void LateUpdate() {
		UpdateAnnoLipsync();
	}

	private void UpdateLipsync(int msTime) {
		animator.SetFloat("Random", Random.value);
		var lipsyncAtTime = annoReader.GetLipsyncAtTime(msTime);
		animator.SetInteger("LipSync/PhonemeIndex0", lipsyncAtTime.Constituents[0].Phoneme);
		layer0Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[0].Phoneme)
			? Mathf.MoveTowards(layer0Weigth, lipsyncAtTime.Constituents[0].Weight,
				Time.deltaTime / consonantSmoothTime)
			: Mathf.MoveTowards(layer0Weigth, lipsyncAtTime.Constituents[0].Weight, Time.deltaTime / vowelsSmoothTime);
		animator.SetLayerWeight(lipsyncLayer0Index, layer0Weigth);
		if (lipsyncAtTime.Constituents.Length > 1) {
			animator.SetInteger("LipSync/PhonemeIndex1", lipsyncAtTime.Constituents[1].Phoneme);
			layer1Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[1].Phoneme)
				? Mathf.MoveTowards(layer1Weigth, lipsyncAtTime.Constituents[1].Weight,
					Time.deltaTime / consonantSmoothTime)
				: Mathf.MoveTowards(layer1Weigth, lipsyncAtTime.Constituents[1].Weight,
					Time.deltaTime / vowelsSmoothTime);
			animator.SetLayerWeight(lipsyncLayer1Index, layer1Weigth);
		} else {
			animator.SetInteger("LipSync/PhonemeIndex1", 0);
			layer1Weigth = Mathf.MoveTowards(layer1Weigth, 0.0f, Time.deltaTime / smoothTime);
			animator.SetLayerWeight(lipsyncLayer1Index, layer1Weigth);
		}

		if (lipsyncAtTime.Constituents.Length > 2) {
			animator.SetInteger("LipSync/PhonemeIndex2", lipsyncAtTime.Constituents[2].Phoneme);
			layer2Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[2].Phoneme)
				? Mathf.MoveTowards(layer2Weigth, lipsyncAtTime.Constituents[2].Weight,
					Time.deltaTime / consonantSmoothTime)
				: Mathf.MoveTowards(layer2Weigth, lipsyncAtTime.Constituents[2].Weight,
					Time.deltaTime / vowelsSmoothTime);
			animator.SetLayerWeight(lipsyncLayer2Index, layer2Weigth);
		} else {
			animator.SetInteger("LipSync/PhonemeIndex2", 0);
			layer2Weigth = Mathf.MoveTowards(layer2Weigth, 0.0f, Time.deltaTime / smoothTime);
			animator.SetLayerWeight(lipsyncLayer2Index, layer2Weigth);
		}
	}

	private void UpdateAnnoLipsync() {
		if (annoReader != null) {
			if (audioState != null && !audioState.Complete)
				UpdateLipsync((int)(audioState.AudioSource.time * 1000.0));
			else
				Stop();
		} else if (layer0Weigth != 0.0 || layer1Weigth != 0.0 || layer2Weigth != 0.0) {
			layer0Weigth = Mathf.MoveTowards(layer0Weigth, 0.0f, Time.deltaTime / vowelsSmoothTime);
			layer1Weigth = Mathf.MoveTowards(layer1Weigth, 0.0f, Time.deltaTime / vowelsSmoothTime);
			layer2Weigth = Mathf.MoveTowards(layer2Weigth, 0.0f, Time.deltaTime / vowelsSmoothTime);
			animator.SetLayerWeight(lipsyncLayer0Index, layer0Weigth);
			animator.SetLayerWeight(lipsyncLayer1Index, layer1Weigth);
			animator.SetLayerWeight(lipsyncLayer2Index, layer2Weigth);
		}
	}

	public struct MyPhoneme {
		public int Index;
		public float Weight;
	}
}