using Engine.Source.Commons;
using System;
using UnityEngine;

namespace Engine.Behaviours.Components
{
  public class PausableAnimator : MonoBehaviour
  {
    private Animator animator;

    private void OnPauseEvent()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        this.animator.SetFloat("Mecanim.Speed", 0.0f);
      else
        this.animator.SetFloat("Mecanim.Speed", 1f);
    }

    private void OnEnable()
    {
      this.animator = this.GetComponent<Animator>();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.OnPauseEvent);
      this.OnPauseEvent();
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.OnPauseEvent);
    }
  }
}
