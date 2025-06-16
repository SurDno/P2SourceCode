using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=7__IafZGwvI&index=1&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/IK/Full Body Biped IK")]
  public class FullBodyBipedIK : IK
  {
    public BipedReferences references = new BipedReferences();
    public IKSolverFullBodyBiped solver = new IKSolverFullBodyBiped();

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page6.html");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_full_body_biped_i_k.html");
    }

    [ContextMenu("TUTORIAL VIDEO (SETUP)")]
    private void OpenSetupTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=7__IafZGwvI");
    }

    [ContextMenu("TUTORIAL VIDEO (INSPECTOR)")]
    private void OpenInspectorTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=tgRMsTphjJo");
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

    public void SetReferences(BipedReferences references, Transform rootNode)
    {
      this.references = references;
      this.solver.SetToReferences(this.references, rootNode);
    }

    public override IKSolver GetIKSolver() => (IKSolver) this.solver;

    public bool ReferencesError(ref string errorMessage)
    {
      if (BipedReferences.SetupError(this.references, ref errorMessage))
        return true;
      if (this.references.spine.Length == 0)
      {
        errorMessage = "References has no spine bones assigned, can not initiate the solver.";
        return true;
      }
      if ((Object) this.solver.rootNode == (Object) null)
      {
        errorMessage = "Root Node bone is null, can not initiate the solver.";
        return true;
      }
      if ((Object) this.solver.rootNode != (Object) this.references.pelvis)
      {
        bool flag = false;
        for (int index = 0; index < this.references.spine.Length; ++index)
        {
          if ((Object) this.solver.rootNode == (Object) this.references.spine[index])
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          errorMessage = "The Root Node has to be one of the bones in the Spine or the Pelvis, can not initiate the solver.";
          return true;
        }
      }
      return false;
    }

    public bool ReferencesWarning(ref string warningMessage)
    {
      if (BipedReferences.SetupWarning(this.references, ref warningMessage))
        return true;
      if ((double) Vector3.Dot((this.references.rightUpperArm.position - this.references.leftUpperArm.position).normalized, (this.solver.rootNode.position - this.references.leftUpperArm.position).normalized) > 0.949999988079071)
      {
        warningMessage = "The root node, the left upper arm and the right upper arm bones should ideally form a triangle that is as close to equilateral as possible. Currently the root node bone seems to be very close to the line between the left upper arm and the right upper arm bones. This might cause unwanted behaviour like the spine turning upside down when pulled by a hand effector.Please set the root node bone to be one of the lower bones in the spine.";
        return true;
      }
      if ((double) Vector3.Dot((this.references.rightThigh.position - this.references.leftThigh.position).normalized, (this.solver.rootNode.position - this.references.leftThigh.position).normalized) <= 0.949999988079071)
        return false;
      warningMessage = "The root node, the left thigh and the right thigh bones should ideally form a triangle that is as close to equilateral as possible. Currently the root node bone seems to be very close to the line between the left thigh and the right thigh bones. This might cause unwanted behaviour like the hip turning upside down when pulled by an effector.Please set the root node bone to be one of the higher bones in the spine.";
      return true;
    }

    [ContextMenu("Reinitiate")]
    private void Reinitiate() => this.SetReferences(this.references, this.solver.rootNode);

    [ContextMenu("Auto-detect References")]
    private void AutoDetectReferences()
    {
      this.references = new BipedReferences();
      BipedReferences.AutoDetectReferences(ref this.references, this.transform, new BipedReferences.AutoDetectParams(true, false));
      this.solver.rootNode = IKSolverFullBodyBiped.DetectRootNodeBone(this.references);
      this.solver.SetToReferences(this.references, this.solver.rootNode);
    }
  }
}
