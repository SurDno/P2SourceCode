using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class IntMappingFloatView : FloatViewBase
  {
    [SerializeField]
    private IntView intView;
    [SerializeField]
    private Step[] steps = new Step[0];
    [SerializeField]
    private int defaultValue;

    protected override void ApplyFloatValue()
    {
      if (intView == null)
        return;
      for (int index = 0; index < steps.Length; ++index)
      {
        if (steps[index].CheckCondition(FloatValue))
        {
          intView.IntValue = steps[index].ResultValue;
          return;
        }
      }
      intView.IntValue = defaultValue;
    }

    public override void SkipAnimation()
    {
      if (!(intView != null))
        return;
      intView.SkipAnimation();
    }

    [Serializable]
    public struct Step
    {
      public int ResultValue;
      public ConditionEnum Condition;
      public float ConditionValue;

      public bool CheckCondition(float value)
      {
        switch (Condition)
        {
          case ConditionEnum.Equal:
            return value == (double) ConditionValue;
          case ConditionEnum.NotEqual:
            return value != (double) ConditionValue;
          case ConditionEnum.Greater:
            return value > (double) ConditionValue;
          case ConditionEnum.Less:
            return value < (double) ConditionValue;
          case ConditionEnum.GreaterOrEqual:
            return value >= (double) ConditionValue;
          case ConditionEnum.LessOrEqual:
            return value <= (double) ConditionValue;
          default:
            return false;
        }
      }

      public enum ConditionEnum : byte
      {
        None,
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
      }
    }
  }
}
