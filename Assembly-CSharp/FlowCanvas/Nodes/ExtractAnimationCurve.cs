// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractAnimationCurve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractAnimationCurve : 
    ExtractorNode<AnimationCurve, Keyframe[], float, WrapMode, WrapMode>
  {
    public override void Invoke(
      AnimationCurve curve,
      out Keyframe[] keys,
      out float length,
      out WrapMode postWrapMode,
      out WrapMode preWrapMode)
    {
      keys = curve.keys;
      length = (float) curve.length;
      postWrapMode = curve.postWrapMode;
      preWrapMode = curve.preWrapMode;
    }
  }
}
