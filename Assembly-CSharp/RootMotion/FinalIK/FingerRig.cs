using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class FingerRig : SolverManager
  {
    [Tooltip("The master weight for all fingers.")]
    [Range(0.0f, 1f)]
    public float weight = 1f;
    public Finger[] fingers = new Finger[0];

    public bool initiated { get; private set; }

    public bool IsValid(ref string errorMessage)
    {
      foreach (Finger finger in this.fingers)
      {
        if (!finger.IsValid(ref errorMessage))
          return false;
      }
      return true;
    }

    [ContextMenu("Auto-detect")]
    public void AutoDetect()
    {
      this.fingers = new Finger[0];
      for (int index = 0; index < this.transform.childCount; ++index)
      {
        Transform[] array = new Transform[0];
        this.AddChildrenRecursive(this.transform.GetChild(index), ref array);
        if (array.Length == 3 || array.Length == 4)
        {
          Finger finger = new Finger();
          finger.bone1 = array[0];
          finger.bone2 = array[1];
          if (array.Length == 3)
          {
            finger.tip = array[2];
          }
          else
          {
            finger.bone3 = array[2];
            finger.tip = array[3];
          }
          finger.weight = 1f;
          Array.Resize<Finger>(ref this.fingers, this.fingers.Length + 1);
          this.fingers[this.fingers.Length - 1] = finger;
        }
      }
    }

    public void AddFinger(
      Transform bone1,
      Transform bone2,
      Transform bone3,
      Transform tip,
      Transform target = null)
    {
      Finger finger = new Finger();
      finger.bone1 = bone1;
      finger.bone2 = bone2;
      finger.bone3 = bone3;
      finger.tip = tip;
      finger.target = target;
      Array.Resize<Finger>(ref this.fingers, this.fingers.Length + 1);
      this.fingers[this.fingers.Length - 1] = finger;
      this.initiated = false;
      finger.Initiate(this.transform, this.fingers.Length - 1);
      if (!this.fingers[this.fingers.Length - 1].initiated)
        return;
      this.initiated = true;
    }

    public void RemoveFinger(int index)
    {
      if ((double) index < 0.0 || index >= this.fingers.Length)
        Warning.Log("RemoveFinger index out of bounds.", this.transform);
      else if (this.fingers.Length == 1)
      {
        this.fingers = new Finger[0];
      }
      else
      {
        Finger[] fingerArray = new Finger[this.fingers.Length - 1];
        int index1 = 0;
        for (int index2 = 0; index2 < this.fingers.Length; ++index2)
        {
          if (index2 != index)
          {
            fingerArray[index1] = this.fingers[index2];
            ++index1;
          }
        }
        this.fingers = fingerArray;
      }
    }

    private void AddChildrenRecursive(Transform parent, ref Transform[] array)
    {
      Array.Resize<Transform>(ref array, array.Length + 1);
      array[array.Length - 1] = parent;
      if (parent.childCount != 1)
        return;
      this.AddChildrenRecursive(parent.GetChild(0), ref array);
    }

    protected override void InitiateSolver()
    {
      this.initiated = true;
      for (int index = 0; index < this.fingers.Length; ++index)
      {
        this.fingers[index].Initiate(this.transform, index);
        if (!this.fingers[index].initiated)
          this.initiated = false;
      }
    }

    public void UpdateFingerSolvers()
    {
      if ((double) this.weight <= 0.0)
        return;
      foreach (Finger finger in this.fingers)
        finger.Update(this.weight);
    }

    public void FixFingerTransforms()
    {
      foreach (Finger finger in this.fingers)
        finger.FixTransforms();
    }

    protected override void UpdateSolver() => this.UpdateFingerSolvers();

    protected override void FixTransforms() => this.FixFingerTransforms();
  }
}
