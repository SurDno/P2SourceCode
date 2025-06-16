using Engine.Source.Audio;
using Externals.LipSync;
using UnityEngine;

public class Lipsync : MonoBehaviour
{
  [SerializeField]
  private float smoothTime = 0.15f;
  [SerializeField]
  private float vowelsSmoothTime = 0.5f;
  [SerializeField]
  private float consonantSmoothTime = 0.15f;
  [Header("You can use ComputeNpc to fill this!")]
  [SerializeField]
  private Animator animator;
  private CustomAnnoReader annoReader;
  private AudioState audioState;
  private int lipsyncLayer0Index;
  private int lipsyncLayer1Index;
  private int lipsyncLayer2Index;
  private float layer0Weigth;
  private float layer1Weigth;
  private float layer2Weigth;

  public void Play(AudioState audioState, byte[] data)
  {
    this.Stop();
    this.annoReader = new CustomAnnoReader(data);
    this.audioState = audioState;
  }

  public void Stop()
  {
    if (this.annoReader == null)
      return;
    this.annoReader = (CustomAnnoReader) null;
  }

  private void Start()
  {
    this.lipsyncLayer0Index = this.animator.GetLayerIndex("Lipsync 0");
    this.lipsyncLayer1Index = this.animator.GetLayerIndex("Lipsync 1");
    this.lipsyncLayer2Index = this.animator.GetLayerIndex("Lipsync 2");
  }

  private void LateUpdate() => this.UpdateAnnoLipsync();

  private void UpdateLipsync(int msTime)
  {
    this.animator.SetFloat("Random", Random.value);
    PhonemeMixtureArticulationData lipsyncAtTime = this.annoReader.GetLipsyncAtTime(msTime);
    this.animator.SetInteger("LipSync/PhonemeIndex0", lipsyncAtTime.Constituents[0].Phoneme);
    this.layer0Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[0].Phoneme) ? Mathf.MoveTowards(this.layer0Weigth, lipsyncAtTime.Constituents[0].Weight, Time.deltaTime / this.consonantSmoothTime) : Mathf.MoveTowards(this.layer0Weigth, lipsyncAtTime.Constituents[0].Weight, Time.deltaTime / this.vowelsSmoothTime);
    this.animator.SetLayerWeight(this.lipsyncLayer0Index, this.layer0Weigth);
    if (lipsyncAtTime.Constituents.Length > 1)
    {
      this.animator.SetInteger("LipSync/PhonemeIndex1", lipsyncAtTime.Constituents[1].Phoneme);
      this.layer1Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[1].Phoneme) ? Mathf.MoveTowards(this.layer1Weigth, lipsyncAtTime.Constituents[1].Weight, Time.deltaTime / this.consonantSmoothTime) : Mathf.MoveTowards(this.layer1Weigth, lipsyncAtTime.Constituents[1].Weight, Time.deltaTime / this.vowelsSmoothTime);
      this.animator.SetLayerWeight(this.lipsyncLayer1Index, this.layer1Weigth);
    }
    else
    {
      this.animator.SetInteger("LipSync/PhonemeIndex1", 0);
      this.layer1Weigth = Mathf.MoveTowards(this.layer1Weigth, 0.0f, Time.deltaTime / this.smoothTime);
      this.animator.SetLayerWeight(this.lipsyncLayer1Index, this.layer1Weigth);
    }
    if (lipsyncAtTime.Constituents.Length > 2)
    {
      this.animator.SetInteger("LipSync/PhonemeIndex2", lipsyncAtTime.Constituents[2].Phoneme);
      this.layer2Weigth = !LipsyncUtility.Vowels.Contains(lipsyncAtTime.Constituents[2].Phoneme) ? Mathf.MoveTowards(this.layer2Weigth, lipsyncAtTime.Constituents[2].Weight, Time.deltaTime / this.consonantSmoothTime) : Mathf.MoveTowards(this.layer2Weigth, lipsyncAtTime.Constituents[2].Weight, Time.deltaTime / this.vowelsSmoothTime);
      this.animator.SetLayerWeight(this.lipsyncLayer2Index, this.layer2Weigth);
    }
    else
    {
      this.animator.SetInteger("LipSync/PhonemeIndex2", 0);
      this.layer2Weigth = Mathf.MoveTowards(this.layer2Weigth, 0.0f, Time.deltaTime / this.smoothTime);
      this.animator.SetLayerWeight(this.lipsyncLayer2Index, this.layer2Weigth);
    }
  }

  private void UpdateAnnoLipsync()
  {
    if (this.annoReader != null)
    {
      if (this.audioState != null && !this.audioState.Complete)
        this.UpdateLipsync((int) ((double) this.audioState.AudioSource.time * 1000.0));
      else
        this.Stop();
    }
    else if ((double) this.layer0Weigth != 0.0 || (double) this.layer1Weigth != 0.0 || (double) this.layer2Weigth != 0.0)
    {
      this.layer0Weigth = Mathf.MoveTowards(this.layer0Weigth, 0.0f, Time.deltaTime / this.vowelsSmoothTime);
      this.layer1Weigth = Mathf.MoveTowards(this.layer1Weigth, 0.0f, Time.deltaTime / this.vowelsSmoothTime);
      this.layer2Weigth = Mathf.MoveTowards(this.layer2Weigth, 0.0f, Time.deltaTime / this.vowelsSmoothTime);
      this.animator.SetLayerWeight(this.lipsyncLayer0Index, this.layer0Weigth);
      this.animator.SetLayerWeight(this.lipsyncLayer1Index, this.layer1Weigth);
      this.animator.SetLayerWeight(this.lipsyncLayer2Index, this.layer2Weigth);
    }
  }

  public struct MyPhoneme
  {
    public int Index;
    public float Weight;
  }
}
