﻿using UnityEngine;

namespace RootMotion.Dynamics
{
  public class JointBreakBroadcaster : MonoBehaviour
  {
    [SerializeField]
    [HideInInspector]
    public PuppetMaster puppetMaster;
    [SerializeField]
    [HideInInspector]
    public int muscleIndex;

    private void OnJointBreak()
    {
      if (!enabled)
        return;
      puppetMaster.RemoveMuscleRecursive(puppetMaster.muscles[muscleIndex].joint, true, true, MuscleRemoveMode.Numb);
    }
  }
}
