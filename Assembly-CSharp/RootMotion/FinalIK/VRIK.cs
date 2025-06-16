// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.VRIK
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [AddComponentMenu("Scripts/RootMotion.FinalIK/IK/VR IK")]
  public class VRIK : IK
  {
    [ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
    [Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character.")]
    public VRIK.References references = new VRIK.References();
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
      VRIK.References.AutoDetectReferences(this.transform, out this.references);
    }

    [ContextMenu("Guess Hand Orientations")]
    public void GuessHandOrientations()
    {
      this.solver.GuessHandOrientations(this.references, false);
    }

    public override IKSolver GetIKSolver() => (IKSolver) this.solver;

    protected override void InitiateSolver()
    {
      if (this.references.isEmpty)
        this.AutoDetectReferences();
      if (this.references.isFilled)
        this.solver.SetToReferences(this.references);
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
          this.root,
          this.pelvis,
          this.spine,
          this.chest,
          this.neck,
          this.head,
          this.leftShoulder,
          this.leftUpperArm,
          this.leftForearm,
          this.leftHand,
          this.rightShoulder,
          this.rightUpperArm,
          this.rightForearm,
          this.rightHand,
          this.leftThigh,
          this.leftCalf,
          this.leftFoot,
          this.leftToes,
          this.rightThigh,
          this.rightCalf,
          this.rightFoot,
          this.rightToes
        };
      }

      public bool isFilled
      {
        get
        {
          return !((UnityEngine.Object) this.root == (UnityEngine.Object) null) && !((UnityEngine.Object) this.pelvis == (UnityEngine.Object) null) && !((UnityEngine.Object) this.spine == (UnityEngine.Object) null) && !((UnityEngine.Object) this.head == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftUpperArm == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftForearm == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftHand == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightUpperArm == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightForearm == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightHand == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftThigh == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftCalf == (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftFoot == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightThigh == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightCalf == (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightFoot == (UnityEngine.Object) null);
        }
      }

      public bool isEmpty
      {
        get
        {
          return !((UnityEngine.Object) this.root != (UnityEngine.Object) null) && !((UnityEngine.Object) this.pelvis != (UnityEngine.Object) null) && !((UnityEngine.Object) this.spine != (UnityEngine.Object) null) && !((UnityEngine.Object) this.chest != (UnityEngine.Object) null) && !((UnityEngine.Object) this.neck != (UnityEngine.Object) null) && !((UnityEngine.Object) this.head != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftShoulder != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftUpperArm != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftForearm != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftHand != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightShoulder != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightUpperArm != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightForearm != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightHand != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftThigh != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftCalf != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftFoot != (UnityEngine.Object) null) && !((UnityEngine.Object) this.leftToes != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightThigh != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightCalf != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightFoot != (UnityEngine.Object) null) && !((UnityEngine.Object) this.rightToes != (UnityEngine.Object) null);
        }
      }

      public static bool AutoDetectReferences(Transform root, out VRIK.References references)
      {
        references = new VRIK.References();
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
