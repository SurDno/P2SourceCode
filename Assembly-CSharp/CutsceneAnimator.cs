using System;
using Cofe.Utility;
using Inspectors;

public class CutsceneAnimator : MonoBehaviour
{
  [SerializeField]
  private Item[] items = new Item[0];

  [Inspected]
  private void Run()
  {
    foreach (Item obj in items)
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
