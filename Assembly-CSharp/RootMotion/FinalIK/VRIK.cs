using System;

namespace RootMotion.FinalIK
{
  [AddComponentMenu("Scripts/RootMotion.FinalIK/IK/VR IK")]
  public class VRIK : IK
  {
    [ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
    [Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character.")]
    public References references = new References();
    [Tooltip("The VRIK solver.")]
    public IKSolverVR solver = new IKSolverVR();

    [ContextMenu("User Manual")]
    protected override void OpenUserManual()
    {
      Debug.Log((object) "Sorry, VRIK User Manual is not finished yet.");
    }

    [ContextMenu("Scrpt Reference")]
    protected override void OpenScriptReference()
    {
      Debug.Log((object) "Sorry, VRIK Script reference is not finished yet.");
    }

    [ContextMenu("TUTORIAL VIDEO (STEAMVR SETUP)")]
    private void OpenSetupTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=6Pfx7lYQiIA&feature=youtu.be");
    }

    [ContextMenu("Auto-detect References")]
    public void AutoDetectReferences()
    {
      References.AutoDetectReferences(this.transform, out references);
    }

    [ContextMenu("Guess Hand Orientations")]
    public void GuessHandOrientations()
    {
      solver.GuessHandOrientations(references, false);
    }

    public override IKSolver GetIKSolver() => solver;

    protected override void InitiateSolver()
    {
      if (references.isEmpty)
        AutoDetectReferences();
      if (references.isFilled)
        solver.SetToReferences(references);
      base.InitiateSolver();
    }

    [Serializable]
    public class References
    {
      public Transform root;
      public Transform pelvis;
      public Transform spine;
      public Transform chest;
      public Transform neck;
      public Transform head;
      public Transform leftShoulder;
      public Transform leftUpperArm;
      public Transform leftForearm;
      public Transform leftHand;
      public Transform rightShoulder;
      public Transform rightUpperArm;
      public Transform rightForearm;
      public Transform rightHand;
      public Transform leftThigh;
      public Transform leftCalf;
      public Transform leftFoot;
      public Transform leftToes;
      public Transform rightThigh;
      public Transform rightCalf;
      public Transform rightFoot;
      public Transform rightToes;

      public Transform[] GetTransforms()
      {
        return new Transform[22]
        {
          root,
          pelvis,
          spine,
          chest,
          neck,
          head,
          leftShoulder,
          leftUpperArm,
          leftForearm,
          leftHand,
          rightShoulder,
          rightUpperArm,
          rightForearm,
          rightHand,
          leftThigh,
          leftCalf,
          leftFoot,
          leftToes,
          rightThigh,
          rightCalf,
          rightFoot,
          rightToes
        };
      }

      public bool isFilled
      {
        get
        {
          return !((UnityEngine.Object) root == (UnityEngine.Object) null) && !((UnityEngine.Object) pelvis == (UnityEngine.Object) null) && !((UnityEngine.Object) spine == (UnityEngine.Object) null) && !((UnityEngine.Object) head == (UnityEngine.Object) null) && !((UnityEngine.Object) leftUpperArm == (UnityEngine.Object) null) && !((UnityEngine.Object) leftForearm == (UnityEngine.Object) null) && !((UnityEngine.Object) leftHand == (UnityEngine.Object) null) && !((UnityEngine.Object) rightUpperArm == (UnityEngine.Object) null) && !((UnityEngine.Object) rightForearm == (UnityEngine.Object) null) && !((UnityEngine.Object) rightHand == (UnityEngine.Object) null) && !((UnityEngine.Object) leftThigh == (UnityEngine.Object) null) && !((UnityEngine.Object) leftCalf == (UnityEngine.Object) null) && !((UnityEngine.Object) leftFoot == (UnityEngine.Object) null) && !((UnityEngine.Object) rightThigh == (UnityEngine.Object) null) && !((UnityEngine.Object) rightCalf == (UnityEngine.Object) null) && !((UnityEngine.Object) rightFoot == (UnityEngine.Object) null);
        }
      }

      public bool isEmpty
      {
        get
        {
          return !((UnityEngine.Object) root != (UnityEngine.Object) null) && !((UnityEngine.Object) pelvis != (UnityEngine.Object) null) && !((UnityEngine.Object) spine != (UnityEngine.Object) null) && !((UnityEngine.Object) chest != (UnityEngine.Object) null) && !((UnityEngine.Object) neck != (UnityEngine.Object) null) && !((UnityEngine.Object) head != (UnityEngine.Object) null) && !((UnityEngine.Object) leftShoulder != (UnityEngine.Object) null) && !((UnityEngine.Object) leftUpperArm != (UnityEngine.Object) null) && !((UnityEngine.Object) leftForearm != (UnityEngine.Object) null) && !((UnityEngine.Object) leftHand != (UnityEngine.Object) null) && !((UnityEngine.Object) rightShoulder != (UnityEngine.Object) null) && !((UnityEngine.Object) rightUpperArm != (UnityEngine.Object) null) && !((UnityEngine.Object) rightForearm != (UnityEngine.Object) null) && !((UnityEngine.Object) rightHand != (UnityEngine.Object) null) && !((UnityEngine.Object) leftThigh != (UnityEngine.Object) null) && !((UnityEngine.Object) leftCalf != (UnityEngine.Object) null) && !((UnityEngine.Object) leftFoot != (UnityEngine.Object) null) && !((UnityEngine.Object) leftToes != (UnityEngine.Object) null) && !((UnityEngine.Object) rightThigh != (UnityEngine.Object) null) && !((UnityEngine.Object) rightCalf != (UnityEngine.Object) null) && !((UnityEngine.Object) rightFoot != (UnityEngine.Object) null) && !((UnityEngine.Object) rightToes != (UnityEngine.Object) null);
        }
      }

      public static bool AutoDetectReferences(Transform root, out References references)
      {
        references = new References();
        Animator componentInChildren = root.GetComponentInChildren<Animator>();
        if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null || !componentInChildren.isHuman)
        {
          Debug.LogWarning((object) "VRIK needs a Humanoid Animator to auto-detect biped references. Please assign references manually.");
          return false;
        }
        references.root = root;
        references.pelvis = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
        references.spine = componentInChildren.GetBoneTransform(HumanBodyBones.Spine);
        references.chest = componentInChildren.GetBoneTransform(HumanBodyBones.Chest);
        references.neck = componentInChildren.GetBoneTransform(HumanBodyBones.Neck);
        references.head = componentInChildren.GetBoneTransform(HumanBodyBones.Head);
        references.leftShoulder = componentInChildren.GetBoneTransform(HumanBodyBones.LeftShoulder);
        references.leftUpperArm = componentInChildren.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        references.leftForearm = componentInChildren.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        references.leftHand = componentInChildren.GetBoneTransform(HumanBodyBones.LeftHand);
        references.rightShoulder = componentInChildren.GetBoneTransform(HumanBodyBones.RightShoulder);
        references.rightUpperArm = componentInChildren.GetBoneTransform(HumanBodyBones.RightUpperArm);
        references.rightForearm = componentInChildren.GetBoneTransform(HumanBodyBones.RightLowerArm);
        references.rightHand = componentInChildren.GetBoneTransform(HumanBodyBones.RightHand);
        references.leftThigh = componentInChildren.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        references.leftCalf = componentInChildren.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        references.leftFoot = componentInChildren.GetBoneTransform(HumanBodyBones.LeftFoot);
        references.leftToes = componentInChildren.GetBoneTransform(HumanBodyBones.LeftToes);
        references.rightThigh = componentInChildren.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        references.rightCalf = componentInChildren.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        references.rightFoot = componentInChildren.GetBoneTransform(HumanBodyBones.RightFoot);
        references.rightToes = componentInChildren.GetBoneTransform(HumanBodyBones.RightToes);
        return true;
      }
    }
  }
}
