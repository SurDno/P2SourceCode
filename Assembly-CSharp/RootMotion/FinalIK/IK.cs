// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IK
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace RootMotion.FinalIK
{
  public abstract class IK : SolverManager
  {
    public abstract IKSolver GetIKSolver();

    protected override void UpdateSolver()
    {
      if (!this.GetIKSolver().initiated)
        this.InitiateSolver();
      if (!this.GetIKSolver().initiated)
        return;
      this.GetIKSolver().Update();
    }

    protected override void InitiateSolver()
    {
      if (this.GetIKSolver().initiated)
        return;
      this.GetIKSolver().Initiate(this.transform);
    }

    protected override void FixTransforms()
    {
      if (!this.GetIKSolver().initiated)
        return;
      this.GetIKSolver().FixTransforms();
    }

    protected abstract void OpenUserManual();

    protected abstract void OpenScriptReference();
  }
}
