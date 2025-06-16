namespace RootMotion
{
  public class SolverManager : MonoBehaviour
  {
    [Tooltip("If true, will fix all the Transforms used by the solver to their initial state in each Update. This prevents potential problems with unanimated bones and animator culling with a small cost of performance. Not recommended for CCD and FABRIK solvers.")]
    public bool fixTransforms = true;
    private Animator animator;
    private Animation legacy;
    private bool updateFrame;
    private bool componentInitiated;
    private bool skipSolverUpdate;

    public void Disable()
    {
      Debug.Log((object) "IK.Disable() is deprecated. Use enabled = false instead", (Object) this.transform);
      this.enabled = false;
    }

    protected virtual void InitiateSolver()
    {
    }

    protected virtual void UpdateSolver()
    {
    }

    protected virtual void FixTransforms()
    {
    }

    private void OnDisable()
    {
      if (!Application.isPlaying)
        return;
      Initiate();
    }

    private void Start() => Initiate();

    private bool animatePhysics
    {
      get
      {
        if ((Object) animator != (Object) null)
          return animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
        return (Object) legacy != (Object) null && legacy.animatePhysics;
      }
    }

    private void Initiate()
    {
      if (componentInitiated)
        return;
      FindAnimatorRecursive(this.transform, true);
      InitiateSolver();
      componentInitiated = true;
    }

    private void Update()
    {
      if (skipSolverUpdate || animatePhysics || !fixTransforms)
        return;
      FixTransforms();
    }

    private void FindAnimatorRecursive(Transform t, bool findInChildren)
    {
      if (isAnimated)
        return;
      animator = t.GetComponent<Animator>();
      legacy = t.GetComponent<Animation>();
      if (isAnimated)
        return;
      if ((Object) animator == (Object) null & findInChildren)
        animator = t.GetComponentInChildren<Animator>();
      if ((Object) legacy == (Object) null & findInChildren)
        legacy = t.GetComponentInChildren<Animation>();
      if (isAnimated || !((Object) t.parent != (Object) null))
        return;
      FindAnimatorRecursive(t.parent, false);
    }

    private bool isAnimated
    {
      get => (Object) animator != (Object) null || (Object) legacy != (Object) null;
    }

    private void FixedUpdate()
    {
      if (skipSolverUpdate)
        skipSolverUpdate = false;
      updateFrame = true;
      if (!animatePhysics || !fixTransforms)
        return;
      FixTransforms();
    }

    private void LateUpdate()
    {
      if (skipSolverUpdate)
        return;
      if (!animatePhysics)
        updateFrame = true;
      if (!updateFrame)
        return;
      updateFrame = false;
      UpdateSolver();
    }

    public void UpdateSolverExternal()
    {
      if (!this.enabled)
        return;
      skipSolverUpdate = true;
      UpdateSolver();
    }
  }
}
