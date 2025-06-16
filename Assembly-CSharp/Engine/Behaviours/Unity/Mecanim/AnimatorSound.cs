using System;

namespace Engine.Behaviours.Unity.Mecanim
{
  [Serializable]
  public class AnimatorSound : StateMachineBehaviour
  {
    public AudioClip[] Clips;
    public float Volume = 1f;
    public float Delay;
    private GameObject audioGO;

    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      PivotHead component = animator.gameObject.GetComponent<PivotHead>();
      audioGO = new GameObject("Sound3D");
      audioGO.transform.SetParent(component.Head, false);
      AudioSource audioSource = audioGO.AddComponent<AudioSource>();
      audioSource.clip = Clips[UnityEngine.Random.Range(0, Clips.Length)];
      audioSource.volume = Volume * 0.5f;
      audioSource.spatialBlend = 1f;
      audioSource.minDistance = 0.4f;
      audioSource.PlayDelayed(Delay);
    }

    public override void OnStateExit(
      Animator animator,
      AnimatorStateInfo animatorStateInfo,
      int layerIndex)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) audioGO);
    }
  }
}
