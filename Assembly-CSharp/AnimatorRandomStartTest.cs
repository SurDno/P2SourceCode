using Engine.Behaviours.Components;

public class AnimatorRandomStartTest : MonoBehaviour
{
  private void Start()
  {
    Pivot component = this.GetComponent<Pivot>();
    if (!(bool) (Object) component)
      return;
    Animator animator = component.GetAnimator();
    if ((bool) (Object) animator)
      animator.Play(0, 0, (float) Random.Range(0, 1));
  }

  private void Update()
  {
  }
}
