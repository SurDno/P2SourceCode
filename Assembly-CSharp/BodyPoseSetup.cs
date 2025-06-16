using Inspectors;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Animation))]
public class BodyPoseSetup : MonoBehaviour
{
  private Animation animation;
  [SerializeField]
  private AnimationClip[] clips;
  [SerializeField]
  [HideInInspector]
  private int currentClip;

  [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
  private int CurrentClip
  {
    get => this.currentClip;
    set
    {
      this.currentClip = value;
      this.Sample();
    }
  }

  private void Awake()
  {
    this.animation = this.GetComponent<Animation>();
    if (!((Object) this.animation == (Object) null))
      return;
    Debug.LogError((object) (typeof (BodyPoseSetup).Name + ":" + this.gameObject.name + " should contain Animation component"));
  }

  private void Start() => this.Sample();

  private void Sample()
  {
    if ((Object) this.animation == (Object) null || this.clips.Length < 0 || this.clips.Length <= this.currentClip)
      return;
    this.animation.AddClip(this.clips[this.currentClip], this.clips[this.currentClip].name);
    this.animation.clip = this.clips[this.currentClip];
    string name = this.animation.clip.name;
    this.animation[name].time = 0.0f;
    this.animation[name].weight = 1f;
    this.animation[name].enabled = true;
    this.animation.Sample();
    this.animation[name].enabled = false;
  }
}
