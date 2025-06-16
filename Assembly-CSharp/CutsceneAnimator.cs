using Cofe.Utility;
using Inspectors;
using System;
using UnityEngine;

public class CutsceneAnimator : MonoBehaviour
{
  [SerializeField]
  private CutsceneAnimator.Item[] items = new CutsceneAnimator.Item[0];

  [Inspected]
  private void Run()
  {
    foreach (CutsceneAnimator.Item obj in this.items)
    {
      if ((UnityEngine.Object) obj.Animator != (UnityEngine.Object) null && !obj.Trigger.IsNullOrEmpty())
        obj.Animator.SetTrigger(obj.Trigger);
    }
  }

  [Serializable]
  public class Item
  {
    public Animator Animator;
    public string Trigger;
  }
}
