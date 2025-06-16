using System.Collections;

namespace RootMotion.FinalIK
{
  public abstract class OffsetModifierVRIK : MonoBehaviour
  {
    [Tooltip("The master weight")]
    public float weight = 1f;
    [Tooltip("Reference to the VRIK component")]
    public VRIK ik;
    private float lastTime;

    protected float deltaTime => Time.time - lastTime;

    protected abstract void OnModifyOffset();

    protected virtual void Start() => this.StartCoroutine(Initiate());

    private IEnumerator Initiate()
    {
      while ((Object) ik == (Object) null)
        yield return null;
      IKSolverVR solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + ModifyOffset;
      lastTime = Time.time;
    }

    private void ModifyOffset()
    {
      if (!this.enabled || weight <= 0.0 || deltaTime <= 0.0 || (Object) ik == (Object) null)
        return;
      weight = Mathf.Clamp(weight, 0.0f, 1f);
      OnModifyOffset();
      lastTime = Time.time;
    }

    protected virtual void OnDestroy()
    {
      if (!((Object) ik != (Object) null))
        return;
      IKSolverVR solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - ModifyOffset;
    }
  }
}
