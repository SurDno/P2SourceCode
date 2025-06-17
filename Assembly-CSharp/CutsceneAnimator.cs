using System;
using Cofe.Utility;
using Inspectors;
using UnityEngine;

public class CutsceneAnimator : MonoBehaviour
{
  [SerializeField]
  private Item[] items = [];

  [Inspected]
  private void Run()
  {
    foreach (Item obj in items)
    {
      if (obj.Animator != null && !obj.Trigger.IsNullOrEmpty())
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
