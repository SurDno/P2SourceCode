// Decompiled with JetBrains decompiler
// Type: RootMotion.SolverManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      this.Initiate();
    }

    private void Start() => this.Initiate();

    private bool animatePhysics
    {
      get
      {
        if ((Object) this.animator != (Object) null)
          return this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
        return (Object) this.legacy != (Object) null && this.legacy.animatePhysics;
      }
    }

    private void Initiate()
    {
      if (this.componentInitiated)
        return;
      this.FindAnimatorRecursive(this.transform, true);
      this.InitiateSolver();
      this.componentInitiated = true;
    }

    private void Update()
    {
      if (this.skipSolverUpdate || this.animatePhysics || !this.fixTransforms)
        return;
      this.FixTransforms();
    }

    private void FindAnimatorRecursive(Transform t, bool findInChildren)
    {
      if (this.isAnimated)
        return;
      this.animator = t.GetComponent<Animator>();
      this.legacy = t.GetComponent<Animation>();
      if (this.isAnimated)
        return;
      if ((Object) this.animator == (Object) null & findInChildren)
        this.animator = t.GetComponentInChildren<Animator>();
      if ((Object) this.legacy == (Object) null & findInChildren)
        this.legacy = t.GetComponentInChildren<Animation>();
      if (this.isAnimated || !((Object) t.parent != (Object) null))
        return;
      this.FindAnimatorRecursive(t.parent, false);
    }

    private bool isAnimated
    {
      get => (Object) this.animator != (Object) null || (Object) this.legacy != (Object) null;
    }

    private void FixedUpdate()
    {
      if (this.skipSolverUpdate)
        this.skipSolverUpdate = false;
      this.updateFrame = true;
      if (!this.animatePhysics || !this.fixTransforms)
        return;
      this.FixTransforms();
    }

    private void LateUpdate()
    {
      if (this.skipSolverUpdate)
        return;
      if (!this.animatePhysics)
        this.updateFrame = true;
      if (!this.updateFrame)
        return;
      this.updateFrame = false;
      this.UpdateSolver();
    }

    public void UpdateSolverExternal()
    {
      if (!this.enabled)
        return;
      this.skipSolverUpdate = true;
      this.UpdateSolver();
    }
  }
}
