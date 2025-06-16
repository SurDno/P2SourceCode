using Engine.Source.Commons;

namespace Engine.Behaviours.Components
{
  public class PausableAnimator : MonoBehaviour
  {
    private Animator animator;

    private void OnPauseEvent()
    {
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        animator.SetFloat("Mecanim.Speed", 0.0f);
      else
        animator.SetFloat("Mecanim.Speed", 1f);
    }

    private void OnEnable()
    {
      animator = this.GetComponent<Animator>();
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
      OnPauseEvent();
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= OnPauseEvent;
    }
  }
}
