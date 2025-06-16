// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.IntMappingFloatView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class IntMappingFloatView : FloatViewBase
  {
    [SerializeField]
    private IntView intView;
    [SerializeField]
    private IntMappingFloatView.Step[] steps = new IntMappingFloatView.Step[0];
    [SerializeField]
    private int defaultValue;

    protected override void ApplyFloatValue()
    {
      if ((UnityEngine.Object) this.intView == (UnityEngine.Object) null)
        return;
      for (int index = 0; index < this.steps.Length; ++index)
      {
        if (this.steps[index].CheckCondition(this.FloatValue))
        {
          this.intView.IntValue = this.steps[index].ResultValue;
          return;
        }
      }
      this.intView.IntValue = this.defaultValue;
    }

    public override void SkipAnimation()
    {
      if (!((UnityEngine.Object) this.intView != (UnityEngine.Object) null))
        return;
      this.intView.SkipAnimation();
    }

    [Serializable]
    public struct Step
    {
      public int ResultValue;
      public IntMappingFloatView.Step.ConditionEnum Condition;
      public float ConditionValue;

      public bool CheckCondition(float value)
      {
        switch (this.Condition)
        {
          case IntMappingFloatView.Step.ConditionEnum.Equal:
            return (double) value == (double) this.ConditionValue;
          case IntMappingFloatView.Step.ConditionEnum.NotEqual:
            return (double) value != (double) this.ConditionValue;
          case IntMappingFloatView.Step.ConditionEnum.Greater:
            return (double) value > (double) this.ConditionValue;
          case IntMappingFloatView.Step.ConditionEnum.Less:
            return (double) value < (double) this.ConditionValue;
          case IntMappingFloatView.Step.ConditionEnum.GreaterOrEqual:
            return (double) value >= (double) this.ConditionValue;
          case IntMappingFloatView.Step.ConditionEnum.LessOrEqual:
            return (double) value <= (double) this.ConditionValue;
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
