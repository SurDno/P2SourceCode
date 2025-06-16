using System.Collections.Generic;

namespace Pathologic.Prototype
{
  public class FightAnimation
  {
    private float _matchTargetTime;
    private float _speedScale;
    private float _unlockTime;
    public Vector3 AttackPosition;
    public Quaternion AttackRotation;
    public ListWithScalableValues HitTimesA = new ListWithScalableValues();
    public ListWithScalableValues HitTimesB = new ListWithScalableValues();

    public float SpeedScale
    {
      get => _speedScale;
      set
      {
        _speedScale = value;
        HitTimesA.Scale = _speedScale;
        HitTimesB.Scale = _speedScale;
      }
    }

    public float MatchTargetTime
    {
      get => _matchTargetTime / SpeedScale;
      set => _matchTargetTime = value;
    }

    public float UnlockTime
    {
      get => _unlockTime / SpeedScale;
      set => _unlockTime = value;
    }

    public class ListWithScalableValues
    {
      private List<float> _values = new List<float>();
      public float Scale;

      public int Count => _values.Count;

      public float this[int index] => _values[index] / Scale;

      public void Add(float value) => _values.Add(value);
    }
  }
}
