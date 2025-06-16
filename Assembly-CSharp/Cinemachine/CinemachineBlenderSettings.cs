// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineBlenderSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(10f, DocumentationSortingAttribute.Level.UserRef)]
  [Serializable]
  public sealed class CinemachineBlenderSettings : ScriptableObject
  {
    [Tooltip("The array containing explicitly defined blends between two Virtual Cameras")]
    public CinemachineBlenderSettings.CustomBlend[] m_CustomBlends = (CinemachineBlenderSettings.CustomBlend[]) null;
    public const string kBlendFromAnyCameraLabel = "**ANY CAMERA**";

    public AnimationCurve GetBlendCurveForVirtualCameras(
      string fromCameraName,
      string toCameraName,
      AnimationCurve defaultCurve)
    {
      AnimationCurve animationCurve1 = (AnimationCurve) null;
      AnimationCurve animationCurve2 = (AnimationCurve) null;
      if (this.m_CustomBlends != null)
      {
        for (int index = 0; index < this.m_CustomBlends.Length; ++index)
        {
          CinemachineBlenderSettings.CustomBlend customBlend = this.m_CustomBlends[index];
          if (customBlend.m_From == fromCameraName && customBlend.m_To == toCameraName)
            return customBlend.m_Blend.BlendCurve;
          if (customBlend.m_From == "**ANY CAMERA**")
          {
            if (!string.IsNullOrEmpty(toCameraName) && customBlend.m_To == toCameraName)
              animationCurve1 = customBlend.m_Blend.BlendCurve;
            else if (customBlend.m_To == "**ANY CAMERA**")
              defaultCurve = customBlend.m_Blend.BlendCurve;
          }
          else if (customBlend.m_To == "**ANY CAMERA**" && !string.IsNullOrEmpty(fromCameraName) && customBlend.m_From == fromCameraName)
            animationCurve2 = customBlend.m_Blend.BlendCurve;
        }
      }
      return animationCurve1 ?? animationCurve2 ?? defaultCurve;
    }

    [DocumentationSorting(10.1f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct CustomBlend
    {
      [Tooltip("When blending from this camera")]
      public string m_From;
      [Tooltip("When blending to this camera")]
      public string m_To;
      [Tooltip("Blend curve definition")]
      public CinemachineBlendDefinition m_Blend;
    }
  }
}
