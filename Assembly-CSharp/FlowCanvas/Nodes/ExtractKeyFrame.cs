// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractKeyFrame
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractKeyFrame : ExtractorNode<Keyframe, float, float, float, float>
  {
    public override void Invoke(
      Keyframe key,
      out float inTangent,
      out float outTangent,
      out float time,
      out float value)
    {
      inTangent = key.inTangent;
      outTangent = key.outTangent;
      time = key.time;
      value = key.value;
    }
  }
}
