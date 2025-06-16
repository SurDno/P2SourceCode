// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.LegIK
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [AddComponentMenu("Scripts/RootMotion.FinalIK/IK/Leg IK")]
  public class LegIK : IK
  {
    public IKSolverLeg solver = new IKSolverLeg();

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Debug.Log((object) "No User Manual page for this component yet, sorry.");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Debug.Log((object) "No Script Reference for this component yet, sorry.");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public override IKSolver GetIKSolver() => (IKSolver) this.solver;
  }
}
