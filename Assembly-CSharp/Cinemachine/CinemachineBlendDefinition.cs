// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineBlendDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(10.2f, DocumentationSortingAttribute.Level.UserRef)]
  [Serializable]
  public struct CinemachineBlendDefinition
  {
    [Tooltip("Shape of the blend curve")]
    public CinemachineBlendDefinition.Style m_Style;
    [Tooltip("Duration of the blend, in seconds")]
    public float m_Time;

    public CinemachineBlendDefinition(CinemachineBlendDefinition.Style style, float time)
    {
      this.m_Style = style;
      this.m_Time = time;
    }

    public AnimationCurve BlendCurve
    {
      get
      {
        float timeEnd = Mathf.Max(0.0f, this.m_Time);
        switch (this.m_Style)
        {
          case CinemachineBlendDefinition.Style.EaseInOut:
            return AnimationCurve.EaseInOut(0.0f, 0.0f, timeEnd, 1f);
          case CinemachineBlendDefinition.Style.EaseIn:
            AnimationCurve blendCurve1 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
            Keyframe[] keys1 = blendCurve1.keys;
            keys1[1].inTangent = 0.0f;
            blendCurve1.keys = keys1;
            return blendCurve1;
          case CinemachineBlendDefinition.Style.EaseOut:
            AnimationCurve blendCurve2 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
            Keyframe[] keys2 = blendCurve2.keys;
            keys2[0].outTangent = 0.0f;
            blendCurve2.keys = keys2;
            return blendCurve2;
          case CinemachineBlendDefinition.Style.HardIn:
            AnimationCurve blendCurve3 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
            Keyframe[] keys3 = blendCurve3.keys;
            keys3[0].outTangent = 0.0f;
            keys3[1].inTangent = 1.5708f;
            blendCurve3.keys = keys3;
            return blendCurve3;
          case CinemachineBlendDefinition.Style.HardOut:
            AnimationCurve blendCurve4 = AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
            Keyframe[] keys4 = blendCurve4.keys;
            keys4[0].outTangent = 1.5708f;
            keys4[1].inTangent = 0.0f;
            blendCurve4.keys = keys4;
            return blendCurve4;
          case CinemachineBlendDefinition.Style.Linear:
            return AnimationCurve.Linear(0.0f, 0.0f, timeEnd, 1f);
          default:
            return new AnimationCurve();
        }
      }
    }

    [DocumentationSorting(10.21f, DocumentationSortingAttribute.Level.UserRef)]
    public enum Style
    {
      Cut,
      EaseInOut,
      EaseIn,
      EaseOut,
      HardIn,
      HardOut,
      Linear,
    }
  }
}
