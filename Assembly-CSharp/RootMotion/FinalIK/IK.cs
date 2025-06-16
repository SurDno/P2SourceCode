namespace RootMotion.FinalIK;

public abstract class IK : SolverManager {
	public abstract IKSolver GetIKSolver();

	protected override void UpdateSolver() {
		if (!GetIKSolver().initiated)
			InitiateSolver();
		if (!GetIKSolver().initiated)
			return;
		GetIKSolver().Update();
	}

	protected override void InitiateSolver() {
		if (GetIKSolver().initiated)
			return;
		GetIKSolver().Initiate(transform);
	}

	protected override void FixTransforms() {
		if (!GetIKSolver().initiated)
			return;
		GetIKSolver().FixTransforms();
	}

	protected abstract void OpenUserManual();

	protected abstract void OpenScriptReference();
}