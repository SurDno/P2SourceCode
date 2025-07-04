﻿using Engine.Behaviours.Components;
using UnityEngine;

public class AnimatorRandomStartTest : MonoBehaviour
{
  private void Start()
  {
    Pivot component = GetComponent<Pivot>();
    if (!(bool) (Object) component)
      return;
    Animator animator = component.GetAnimator();
    if ((bool) (Object) animator)
      animator.Play(0, 0, Random.Range(0, 1));
  }

  private void Update()
  {
  }
}
