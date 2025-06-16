using System.Collections.Generic;
using UnityEngine;

namespace Pathologic.Prototype
{
  public class FightAnimation
  {
    private float _matchTargetTime;
    private float _speedScale;
    private float _unlockTime;
    public Vector3 AttackPosition;
    public Quaternion AttackRotation;
    public FightAnimation.ListWithScalableValues HitTimesA = new FightAnimation.ListWithScalableValues();
    public FightAnimation.ListWithScalableValues HitTimesB = new FightAnimation.ListWithScalableValues();

    public float SpeedScale
    {
      get => this._speedScale;
      set
      {
        this._speedScale = value;
        this.HitTimesA.Scale = this._speedScale;
        this.HitTimesB.Scale = this._speedScale;
      }
    }

    public float MatchTargetTime
    {
      get => this._matchTargetTime / this.SpeedScale;
      set => this._matchTargetTime = value;
    }

    public float UnlockTime
    {
      get => this._unlockTime / this.SpeedScale;
      set => this._unlockTime = value;
    }

    public class ListWithScalableValues
    {
      private List<float> _values = new List<float>();
      public float Scale;

      public int Count => this._values.Count;

      public float this[int index] => this._values[index] / this.Scale;

      public void Add(float value) => this._values.Add(value);
    }
  }
}
