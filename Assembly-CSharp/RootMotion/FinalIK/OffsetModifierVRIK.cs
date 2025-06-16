using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public abstract class OffsetModifierVRIK : MonoBehaviour
  {
    [Tooltip("The master weight")]
    public float weight = 1f;
    [Tooltip("Reference to the VRIK component")]
    public VRIK ik;
    private float lastTime;

    protected float deltaTime => Time.time - this.lastTime;

    protected abstract void OnModifyOffset();

    protected virtual void Start() => this.StartCoroutine(this.Initiate());

    private IEnumerator Initiate()
    {
      while ((Object) this.ik == (Object) null)
        yield return (object) null;
      IKSolverVR solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + new IKSolver.UpdateDelegate(this.ModifyOffset);
      this.lastTime = Time.time;
    }

    private void ModifyOffset()
    {
      if (!this.enabled || (double) this.weight <= 0.0 || (double) this.deltaTime <= 0.0 || (Object) this.ik == (Object) null)
        return;
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      this.OnModifyOffset();
      this.lastTime = Time.time;
    }

    protected virtual void OnDestroy()
    {
      if (!((Object) this.ik != (Object) null))
        return;
      IKSolverVR solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - new IKSolver.UpdateDelegate(this.ModifyOffset);
    }
  }
}
