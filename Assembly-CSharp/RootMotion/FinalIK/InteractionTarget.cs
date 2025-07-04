﻿using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=r5jiZnsDH3M")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction Target")]
  public class InteractionTarget : MonoBehaviour
  {
    [Tooltip("The type of the FBBIK effector.")]
    public FullBodyBipedEffector effectorType;
    [Tooltip("InteractionObject weight curve multipliers for this effector target.")]
    public Multiplier[] multipliers;
    [Tooltip("The interaction speed multiplier for this effector. This can be used to make interactions faster/slower for specific effectors.")]
    public float interactionSpeedMlp = 1f;
    [Tooltip("The pivot to twist/swing this interaction target about. For symmetric objects that can be interacted with from a certain angular range.")]
    public Transform pivot;
    [Tooltip("The axis of twisting the interaction target (blue line).")]
    public Vector3 twistAxis = Vector3.up;
    [Tooltip("The weight of twisting the interaction target towards the effector bone in the start of the interaction.")]
    public float twistWeight = 1f;
    [Tooltip("The weight of swinging the interaction target towards the effector bone in the start of the interaction. Swing is defined as a 3-DOF rotation around any axis, while twist is only around the twist axis.")]
    public float swingWeight;
    [Tooltip("If true, will twist/swing around the pivot only once at the start of the interaction. If false, will continue rotating throuout the whole interaction.")]
    public bool rotateOnce = true;
    private Quaternion defaultLocalRotation;
    private Transform lastPivot;

    [ContextMenu("TUTORIAL VIDEO (PART 1: BASICS)")]
    private void OpenTutorial1()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=r5jiZnsDH3M");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 2: PICKING UP...)")]
    private void OpenTutorial2()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=eP9-zycoHLk");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 3: ANIMATION)")]
    private void OpenTutorial3()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=sQfB2RcT1T4&index=14&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 4: TRIGGERS)")]
    private void OpenTutorial4()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
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

    public float GetValue(InteractionObject.WeightCurve.Type curveType)
    {
      for (int index = 0; index < multipliers.Length; ++index)
      {
        if (multipliers[index].curve == curveType)
          return multipliers[index].multiplier;
      }
      return 1f;
    }

    public void ResetRotation()
    {
      if (!(pivot != null))
        return;
      pivot.localRotation = defaultLocalRotation;
    }

    public void RotateTo(Vector3 position)
    {
      if (pivot == null)
        return;
      if (pivot != lastPivot)
      {
        defaultLocalRotation = pivot.localRotation;
        lastPivot = pivot;
      }
      pivot.localRotation = defaultLocalRotation;
      if (twistWeight > 0.0)
      {
        Vector3 tangent1 = transform.position - pivot.position;
        Vector3 axis = pivot.rotation * twistAxis;
        Vector3 normal1 = axis;
        Vector3.OrthoNormalize(ref normal1, ref tangent1);
        Vector3 normal2 = axis;
        Vector3 tangent2 = position - pivot.position;
        Vector3.OrthoNormalize(ref normal2, ref tangent2);
        pivot.rotation = Quaternion.Lerp(Quaternion.identity, QuaTools.FromToAroundAxis(tangent1, tangent2, axis), twistWeight) * pivot.rotation;
      }
      if (swingWeight <= 0.0)
        return;
      pivot.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(transform.position - pivot.position, position - pivot.position), swingWeight) * pivot.rotation;
    }

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page10.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_interaction_target.html");
    }

    [Serializable]
    public class Multiplier
    {
      [Tooltip("The curve type (InteractionObject.WeightCurve.Type).")]
      public InteractionObject.WeightCurve.Type curve;
      [Tooltip("Multiplier of the curve's value.")]
      public float multiplier;
    }
  }
}
